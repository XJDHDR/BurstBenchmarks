using System.Numerics;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;

namespace BenchmarkCode.Single
{
	public enum PixarRayHit
	{
		None = 0,
		Letter = 1,
		Wall = 2,
		Sun = 3
	}

	[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
	public unsafe struct PixarRaytracerBurst : IJob
	{
		public uint width;
		public uint height;
		public uint samples;
		public float result;

		public void Execute()
		{
			result = PixarRaytracer(width, height, samples);
		}

		private uint marsagliaZ, marsagliaW;

		private float PixarRaytracer(uint width, uint height, uint samples)
		{
			marsagliaZ = 666;
			marsagliaW = 999;

			Vector position = new Vector { x = -22.0f, y = 5.0f, z = 25.0f };
			Vector goal = new Vector { x = -3.0f, y = 4.0f, z = 0.0f };

			goal = Add(Inverse(goal), MultiplyFloat(position, -1.0f));

			Vector left = new Vector { x = goal.z, y = 0, z = goal.x };

			left = MultiplyFloat(Inverse(left), 1.0f / width);

			Vector up = Cross(goal, left);
			Vector color = default(Vector);
			Vector adjust = default(Vector);

			for (uint y = height; y > 0; y--)
			{
				for (uint x = width; x > 0; x--)
				{
					for (uint p = samples; p > 0; p--)
					{
						color = Add(color, Trace(position, Add(Inverse(MultiplyFloat(Add(goal, left), x - width / 2 + Random())), MultiplyFloat(up, y - height / 2 + Random()))));
					}

					color = MultiplyFloat(color, (1.0f / samples) + 14.0f / 241.0f);
					adjust = AddFloat(color, 1.0f);
					color = new Vector {
						x = color.x / adjust.x,
						y = color.y / adjust.y,
						z = color.z / adjust.z
					};

					color = MultiplyFloat(color, 255.0f);
				}
			}

			return color.x + color.y + color.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Vector Multiply(Vector left, Vector right) {
			left.x *= right.x;
			left.y *= right.y;
			left.z *= right.z;

			return left;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Vector MultiplyFloat(Vector vector, float value) {
			vector.x *= value;
			vector.y *= value;
			vector.z *= value;

			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private float Modulus(Vector left, Vector right) {
			return left.x * right.x + left.y * right.y + left.z * right.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private float ModulusSelf(Vector vector) {
			return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Vector Inverse(Vector vector) {
			return MultiplyFloat(vector, 1 / math.sqrt(ModulusSelf(vector)));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Vector Add(Vector left, Vector right) {
			left.x += right.x;
			left.y += right.y;
			left.z += right.z;

			return left;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Vector AddFloat(Vector vector, float value) {
			vector.x += value;
			vector.y += value;
			vector.z += value;

			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Vector Cross(Vector to, Vector from) {
			Vector vector = default(Vector);

			vector.x = to.y * from.z - to.z * from.y;
			vector.y = to.z * from.x - to.x * from.z;
			vector.z = to.x * from.y - to.y * from.x;

			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private float Min(float left, float right) {
			return left < right ? left : right;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private float BoxTest(Vector position, Vector lowerLeft, Vector upperRight)
		{
			lowerLeft = MultiplyFloat(Add(position, lowerLeft), -1);
			upperRight = MultiplyFloat(Add(upperRight, position), -1);

			return -Min(Min(Min(lowerLeft.x, upperRight.x), Min(lowerLeft.y, upperRight.y)), Min(lowerLeft.z, upperRight.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private float Random()
		{
			marsagliaZ = 36969 * (marsagliaZ & 65535) + (marsagliaZ >> 16);
			marsagliaW = 18000 * (marsagliaW & 65535) + (marsagliaW >> 16);

			return ((marsagliaZ << 16) + marsagliaW) * 2.0f / 10000000000.0f;
		}

		private float Sample(Vector position, int* hitType)
		{
			const int size = 60;

			float distance = 1e9f;
			Vector f = position;
			byte* letters = stackalloc byte[size];

			// P              // I              // X              // A              // R
			letters[0]  = 53; letters[12] = 65; letters[24] = 73; letters[32] = 85; letters[44] = 97; letters[56] = 99;
			letters[1]  = 79; letters[13] = 79; letters[25] = 79; letters[33] = 79; letters[45] = 79; letters[57] = 87;
			letters[2]  = 53; letters[14] = 69; letters[26] = 81; letters[34] = 89; letters[46] = 97; letters[58] = 105;
			letters[3]  = 95; letters[15] = 79; letters[27] = 95; letters[35] = 95; letters[47] = 95; letters[59] = 79;
			letters[4]  = 53; letters[16] = 67; letters[28] = 73; letters[36] = 89; letters[48] = 97;
			letters[5]  = 87; letters[17] = 79; letters[29] = 95; letters[37] = 95; letters[49] = 87;
			letters[6]  = 57; letters[18] = 67; letters[30] = 81; letters[38] = 93; letters[50] = 101;
			letters[7]  = 87; letters[19] = 95; letters[31] = 79; letters[39] = 79; letters[51] = 87;
			letters[8]  = 53; letters[20] = 65;                   letters[40] = 87; letters[52] = 97;
			letters[9]  = 95; letters[21] = 95;                   letters[41] = 87; letters[53] = 95;
			letters[10] = 57; letters[22] = 69;                   letters[42] = 91; letters[54] = 101;
			letters[11] = 95; letters[23] = 95;                   letters[43] = 87; letters[55] = 95;

			f.z = 0.0f;

			for (int i = 0; i < size; i += 4)
			{
				Vector begin = MultiplyFloat(new Vector { x = letters[i] - 79.0f, y = letters[i + 1] - 79.0f, z = 0.0f }, 0.5f);
				Vector e = Add(MultiplyFloat(new Vector { x = letters[i + 2] - 79.0f, y = letters[i + 3] - 79.0f, z = 0.0f }, 0.5f), MultiplyFloat(begin, -1.0f));
				Vector o = MultiplyFloat(Add(f, MultiplyFloat(Add(begin, e), Min(-Min(Modulus(MultiplyFloat(Add(begin, f), -1.0f), e) / ModulusSelf(e), 0.0f), 1.0f))), -1.0f);

				distance = Min(distance, ModulusSelf(o));
			}

			distance = math.sqrt(distance);

			Vector* curves = stackalloc Vector[2];

			curves[0] = new Vector { x = -11.0f, y = 6.0f, z = 0.0f };
			curves[1] = new Vector { x = 11.0f, y = 6.0f, z = 0.0f };

			for (int i = 1; i >= 0; i--)
			{
				Vector o = Add(f, MultiplyFloat(curves[i], -1.0f));
				float m = 0.0f;

				if (o.x > 0.0f)
				{
					m = math.abs(math.sqrt(ModulusSelf(o)) - 2.0f);
				}
				else
				{
					if (o.y > 0.0f)
						o.y += -2.0f;
					else
						o.y += 2.0f;

					o.y += math.sqrt(ModulusSelf(o));
				}

				distance = Min(distance, m);
			}

			distance = math.pow(math.pow(distance, 8.0f) + math.pow(position.z, 8.0f), 0.125f) - 0.5f;
			*hitType = (int)PixarRayHit.Letter;

			float roomDistance = Min(-Min(BoxTest(position, new Vector { x = -30.0f, y = -0.5f, z = -30.0f }, new Vector { x = 30.0f, y = 18.0f, z = 30.0f }), BoxTest(position, new Vector { x = -25.0f, y = -17.5f, z = -25.0f }, new Vector { x = 25.0f, y = 20.0f, z = 25.0f })), BoxTest( new Vector { x = math.fmod(math.abs(position.x), 8), y = position.y, z = position.z }, new Vector { x = 1.5f, y = 18.5f, z = -25.0f }, new Vector { x = 6.5f, y = 20.0f, z = 25.0f }));

			if (roomDistance < distance)
			{
				distance = roomDistance;
				*hitType = (int)PixarRayHit.Wall;
			}

			float sun = 19.9f - position.y;

			if (sun < distance)
			{
				distance = sun;
				*hitType = (int)PixarRayHit.Sun;
			}

			return distance;
		}

		private int RayMarching(Vector origin, Vector direction, Vector* hitPosition, Vector* hitNormal)
		{
			int hitType = (int)PixarRayHit.None;
			int noHitCount = 0;
			float distance = 0.0f;

			for (float i = 0; i < 100; i += distance)
			{
				*hitPosition = MultiplyFloat(Add(origin, direction), i);
				distance = Sample(*hitPosition, &hitType);

				if (distance < 0.01f || ++noHitCount > 99)
				{
					*hitNormal = Inverse(new Vector { x = Sample(Add(*hitPosition, new Vector { x = 0.01f, y = 0.0f, z = 0.0f }), &noHitCount) - distance, y = Sample(Add(*hitPosition, new Vector { x = 0.0f, y = 0.01f, z = 0.0f }), &noHitCount) - distance, z = Sample(Add(*hitPosition, new Vector { x = 0.0f, y = 0.0f, z = 0.01f }), &noHitCount) - distance });

					return hitType;
				}
			}

			return (int)PixarRayHit.None;
		}

		private Vector Trace(Vector origin, Vector direction) {
			Vector
				sampledPosition = new Vector { x = 1.0f, y = 1.0f, z = 1.0f },
				normal = new Vector { x = 1.0f, y = 1.0f, z = 1.0f },
				color = new Vector { x = 1.0f, y = 1.0f, z = 1.0f },
				attenuation = new Vector { x = 1.0f, y = 1.0f, z = 1.0f },
				lightDirection = Inverse(new Vector { x = 0.6f, y = 0.6f, z = 1.0f });

			for (int bounce = 3; bounce > 0; bounce--)
			{
				PixarRayHit hitType = (PixarRayHit)RayMarching(origin, direction, &sampledPosition, &normal);

				switch (hitType)
				{
					case PixarRayHit.None:
						break;

					case PixarRayHit.Letter:
					{
						direction = MultiplyFloat(Add(direction, normal), Modulus(normal, direction) * -2.0f);
						origin = MultiplyFloat(Add(sampledPosition, direction), 0.1f);
						attenuation = MultiplyFloat(attenuation, 0.2f);

						break;
					}

					case PixarRayHit.Wall:
					{
						float
							incidence = Modulus(normal, lightDirection),
							p = 6.283185f * Random(),
							c = Random(),
							s = math.sqrt(1.0f - c),
							g = normal.z < 0 ? -1.0f : 1.0f,
							u = -1.0f / (g + normal.z),
							v = normal.x * normal.y * u;

						direction = Add(Add(new Vector { x = v, y = g + normal.y * normal.y * u, z = -normal.y * (math.cos(p) * s) }, new Vector { x = 1.0f + g * normal.x * normal.x * u, y = g * v, z = -g * normal.x }), MultiplyFloat(normal, math.sqrt(c)));
						origin = MultiplyFloat(Add(sampledPosition, direction), 0.1f);
						attenuation = MultiplyFloat(attenuation, 0.2f);

						if (incidence > 0 && RayMarching(MultiplyFloat(Add(sampledPosition, normal), 0.1f), lightDirection, &sampledPosition, &normal) == (int)PixarRayHit.Sun)
							color = MultiplyFloat(Multiply(Add(color, attenuation), new Vector { x = 500.0f, y = 400.0f, z = 100.0f }), incidence);

						break;
					}

					case PixarRayHit.Sun:
					{
						color = Multiply(Add(color, attenuation), new Vector { x = 50.0f, y = 80.0f, z = 100.0f });

						goto escape;
					}
				}
			}

			escape:

			return color;
		}
	}

	[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
	internal unsafe struct PixarRaytracerGCC : IJob
	{
		public uint width;
		public uint height;
		public uint samples;
		public float result;

		public void Execute()
		{
			result = NativeBindings.benchmark_pixar_raytracer(width, height, samples);
		}
	}
}
