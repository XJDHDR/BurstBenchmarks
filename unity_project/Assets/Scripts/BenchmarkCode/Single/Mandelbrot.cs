using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;

namespace BenchmarkCode.Single
{
	[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
	public struct MandelbrotBurst : IJob
	{
		public uint width;
		public uint height;
		public uint iterations;
		public float result;

		public void Execute()
		{
			result = Mandelbrot(width, height, iterations);
		}

		private float Mandelbrot(uint width, uint height, uint iterations)
		{
			float data = 0.0f;

			for (uint i = 0; i < iterations; i++)
			{
				float
					left = -2.1f,
					right = 1.0f,
					top = -1.3f,
					bottom = 1.3f,
					deltaX = (right - left) / width,
					deltaY = (bottom - top) / height,
					coordinateX = left;

				for (uint x = 0; x < width; x++)
				{
					float coordinateY = top;

					for (uint y = 0; y < height; y++)
					{
						float workX = 0;
						float workY = 0;
						int counter = 0;

						while (counter < 255 && math.sqrt((workX * workX) + (workY * workY)) < 2.0f)
						{
							counter++;

							float newX = (workX * workX) - (workY * workY) + coordinateX;

							workY = 2 * workX * workY + coordinateY;
							workX = newX;
						}

						data = workX + workY;
						coordinateY += deltaY;
					}

					coordinateX += deltaX;
				}
			}

			return data;
		}
	}

	[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
	internal struct MandelbrotGCC : IJob
	{
		public uint width;
		public uint height;
		public uint iterations;
		public float result;

		public void Execute()
		{
			result = NativeBindings.benchmark_mandelbrot(width, height, iterations);
		}
	}
}
