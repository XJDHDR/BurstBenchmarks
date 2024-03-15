using Unity.Burst;
using Unity.Jobs;

namespace BenchmarkCode.Integer
{
	[BurstCompile(CompileSynchronously = true)]
	public unsafe struct SieveOfEratosthenesBurst : IJob {
		public uint iterations;
		public uint result;

		public void Execute()
		{
			result = SieveOfEratosthenes(iterations);
		}

		private uint SieveOfEratosthenes(uint iterations)
		{
			const int size = 1024;

			byte* flags = stackalloc byte[size];
			uint a, b, c, prime, count = 0;

			for (a = 1; a <= iterations; a++)
			{
				count = 0;

				for (b = 0; b < size; b++)
				{
					flags[b] = 1; // True
				}

				for (b = 0; b < size; b++)
				{
					if (flags[b] == 1)
					{
						prime = b + b + 3;
						c = b + prime;

						while (c < size)
						{
							flags[c] = 0; // False
							c += prime;
						}

						count++;
					}
				}
			}

			return count;
		}
	}

	[BurstCompile(CompileSynchronously = true)]
	internal unsafe struct SieveOfEratosthenesGCC : IJob
	{
		public uint iterations;
		public uint result;

		public void Execute()
		{
			result = NativeBindings.benchmark_sieve_of_eratosthenes(iterations);
		}
	}
}
