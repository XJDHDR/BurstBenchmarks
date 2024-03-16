using System.Runtime.CompilerServices;
using System.Text;

namespace NET_project.Benchmarks;

public unsafe struct RadixNET : IJob
{
	public uint iterations;
	public int result;

	public void Run()
	{
		result = Radix(iterations);
	}

	private uint classicRandom;

	private int Radix(uint iterations)
	{
		classicRandom = 7525;

		const int arrayLength = 128;

		int[] array = new int[arrayLength];

		for (uint a = 0; a < iterations; a++)
		{
			for (int b = 0; b < arrayLength; b++)
			{
				array[b] = Random();
			}

			Sort(array, arrayLength, (a == 0));
		}

		int head = array[0];

		return head;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int Random()
	{
		classicRandom = (6253729 * classicRandom + 4396403);

		return (int)(classicRandom % 32767);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int FindLargest(int[] array, int length)
	{
		int i;
		int largest = -1;

		for (i = 0; i < length; i++)
		{
			if (array[i] > largest)
				largest = array[i];
		}

		return largest;
	}

	private void Sort(int[] array, int length, bool printDone)
	{
		int i;
		Span<int> semiSorted = stackalloc int[length];
		int significantDigit = 1;
		int largest = FindLargest(array, length);

		while (largest / significantDigit > 0)
		{
			significantDigit = sortIteration(semiSorted);
		}

		int sortIteration(Span<int> semiSorted)
		{
			Span<int> bucket = stackalloc int[10];

			for (i = 0; i < length; i++)
			{
				bucket[(array[i] / significantDigit) % 10]++;
			}

			for (i = 1; i < 10; i++)
			{
				bucket[i] += bucket[i - 1];
			}

			for (i = length - 1; i >= 0; i--)
			{
				semiSorted[--bucket[(array[i] / significantDigit) % 10]] = array[i];
			}

			for (i = 0; i < length; i++)
			{
				array[i] = semiSorted[i];
			}

			significantDigit *= 10;
			return significantDigit;
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
