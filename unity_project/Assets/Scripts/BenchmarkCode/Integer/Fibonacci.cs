using Unity.Burst;
using Unity.Jobs;

namespace BenchmarkCode.Integer
{
	[BurstCompile(CompileSynchronously = true)]
	public struct FibonacciBurst : IJob {
		public uint number;
		public uint result;

		public void Execute()
		{
			result = Fibonacci(number);
		}

		private uint Fibonacci(uint number)
		{
			if (number <= 1)
			{
				return 1;
			}

			return Fibonacci(number - 1) + Fibonacci(number - 2);
		}
	}

	[BurstCompile(CompileSynchronously = true)]
	internal struct FibonacciGCC : IJob
	{
		public uint number;
		public uint result;

		public void Execute()
		{
			result = NativeBindings.benchmark_fibonacci(number);
		}
	}
}
