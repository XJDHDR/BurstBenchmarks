using Unity.Burst;
using Unity.Jobs;

namespace BenchmarkCode.Single
{
	[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
	public unsafe struct PolynomialsBurst : IJob
	{
		public uint iterations;
		public float result;

		public void Execute()
		{
			result = Polynomials(iterations);
		}

		private float Polynomials(uint iterations)
		{
			const float x = 0.2f;

			float pu = 0.0f;
			float* poly = stackalloc float[100];

			for (uint i = 0; i < iterations; i++)
			{
				float mu = 10.0f;
				float s;
				int j;

				for (j = 0; j < 100; j++)
				{
					poly[j] = mu = (mu + 2.0f) / 2.0f;
				}

				s = 0.0f;

				for (j = 0; j < 100; j++)
				{
					s = x * s + poly[j];
				}

				pu += s;
			}

			return pu;
		}
	}

	[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
	internal unsafe struct PolynomialsGCC : IJob
	{
		public uint iterations;
		public float result;

		public void Execute()
		{
			result = NativeBindings.benchmark_polynomials(iterations);
		}
	}
}
