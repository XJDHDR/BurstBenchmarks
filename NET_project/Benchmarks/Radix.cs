using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NET_project.Benchmarks;

[SkipLocalsInit]
public unsafe struct RadixNET : IJob
{
	public uint iterations;
	public int result;

	public void Run() {
		result = Radix(iterations);
	}

	private static uint classicRandom;

	private static int Radix(uint iterations) {
		classicRandom = 7525;
		Span<int> span = stackalloc int[128];

		for (uint a = 0; a < iterations; a++) {
			for (int b = 0; b < span.Length; b++) {
				span[b] = Random();
			}

			Sort(span);
		}

		return span[0];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int Random() {
		classicRandom = (6253729 * classicRandom) + 4396403;

		return (int)(classicRandom % 32767);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int FindLargest(Span<int> span) {
		int largest = -1;

		for (int i = 0; i < span.Length; i++)
		{
			if (span[i] > largest)
				largest = span[i];
		}

		return largest;
	}

	private static void Sort(Span<int> span) {
		const int bucketLength = 10;

		// Neither of these can be Span<int> as the JIT cannot elide
		// the bounds checks due to the contents of `span[i]` being unknown.
		//
		// We could use `Unsafe.Add` as is done for one of the accesses below,
		// but just using pointers is faster and easier to read/maintain

		int* semiSorted = stackalloc int[span.Length];
		int* bucket = stackalloc int[bucketLength];

		int significantDigit = 1;

		for (int largest = FindLargest(span); largest / significantDigit > 0; significantDigit *= 10) {
			new Span<int>(bucket, bucketLength).Clear();

			for (int i = 0; i < span.Length; i++) {
				bucket[span[i] / significantDigit % 10]++;
			}

			for (int i = 1; i < bucketLength; i++) {
				bucket[i] += bucket[i - 1];
			}

			for (int i = span.Length - 1; i >= 0; i--) {
				// The JIT can't currently elide bounds checks for reverse iterated
				// loops so help it out by manually offsetting the reference.
				int value = Unsafe.Add(ref MemoryMarshal.GetReference(span), i);
				semiSorted[--bucket[value / significantDigit % 10]] = value;
			}

			for (int i = 0; i < span.Length; i++) {
				span[i] = semiSorted[i];
			}
		}
	}
}

internal unsafe struct RadixGCC : IJob {
	public uint iterations;
	public int result;
	public string libName;

	public void Run() {
		switch (libName)
		{
			case "GCC":
				result = GccNativeBindings.benchmark_radix(iterations);
				break;

			case "Clang":
				result = ClangNativeBindings.benchmark_radix(iterations);
				break;

			case "MS":
				result = MsNativeBindings.benchmark_radix(iterations);
				break;

			case "Rust":
				result = RustNativeBindings.benchmark_radix(iterations);
				break;
		}
	}
}
