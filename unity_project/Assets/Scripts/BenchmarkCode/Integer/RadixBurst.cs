using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace BenchmarkCode.Integer
{
	[BurstCompile(CompileSynchronously = true)]
	public unsafe struct RadixBurst : IJob
	{
		public uint iterations;
		public int result;

		public void Execute()
		{
			result = Radix(iterations);
		}

		private uint classicRandom;

		private int Radix(uint iterations)
		{
			classicRandom = 7525;

			const int arrayLength = 128;

			NativeArray<int> array = new NativeArray<int>(arrayLength, Allocator.Persistent);

			for (uint a = 0; a < iterations; a++) {
				for (int b = 0; b < arrayLength; b++) {
					array[b] = Random();
				}

				Sort(array, arrayLength);
			}

			int head = array[0];

			array.Dispose();

			return head;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int Random()
		{
			classicRandom = (6253729 * classicRandom + 4396403);

			return (int)(classicRandom % 32767);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int FindLargest(NativeArray<int> array, int length)
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

		private void Sort(NativeArray<int> array, int length)
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
}
