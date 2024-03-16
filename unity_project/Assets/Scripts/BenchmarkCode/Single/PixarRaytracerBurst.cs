using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;

namespace BenchmarkCode.Single
{
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

			float3 position = new float3 { x = -22.0f, y = 5.0f, z = 25.0f };
			float3 goal = new float3 { x = -3.0f, y = 4.0f, z = 0.0f };

			goal = math.normalize(goal) + (position * -1.0f);

			float3 left = new float3 { x = goal.z, y = 0, z = goal.x };

			left = math.normalize(left) * (1.0f / width);

			float3 up = math.cross(goal, left);
			float3 color = default(float3);
			float3 adjust = default(float3);

			for (uint y = height; y > 0; y--)
			{
				for (uint x = width; x > 0; x--)
				{
					for (uint p = samples; p > 0; p--)
					{
						color += Trace(
							position,
							math.normalize(
								((goal + left) * (x - (float)width / 2 + Random()))
							) + (up * (y - (float)height / 2 + Random()))
						);
					}

					color *= ((1.0f / samples) + 14.0f / 241.0f);
					adjust = color + 1.0f;
					color = new float3
					{
						x = color.x / adjust.x,
						y = color.y / adjust.y,
						z = color.z / adjust.z
					};

					color *= 255.0f;
				}
			}

			return color.x + color.y + color.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private float BoxTest(float3 position, float3 lowerLeft, float3 upperRight)
		{
			lowerLeft = (position + lowerLeft) * -1;
			upperRight = (upperRight + position) * -1;

			return -math.min(math.min(math.min(lowerLeft.x, upperRight.x), math.min(lowerLeft.y, upperRight.y)),
				math.min(lowerLeft.z, upperRight.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private float Random()
		{
			marsagliaZ = 36969 * (marsagliaZ & 65535) + (marsagliaZ >> 16);
			marsagliaW = 18000 * (marsagliaW & 65535) + (marsagliaW >> 16);

			return ((marsagliaZ << 16) + marsagliaW) * 2.0f / 10000000000.0f;
		}

		private float Sample(float3 position, ref int hitType)
		{
			const int size = 60;

			float distance = 1e9f;
			float3 f = position;
			Span<byte> letters = stackalloc byte[size];

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
				float3 begin = new float3 { x = letters[i] - 79.0f, y = letters[i + 1] - 79.0f, z = 0.0f } * 0.5f;
				float3 e = (new float3 { x = letters[i + 2] - 79.0f, y = letters[i + 3] - 79.0f, z = 0.0f } * 0.5f) +
				            (begin * -1.0f);
				float3 o = (f + (
					(begin + e) * math.min(
						-math.min(
							math.dot(
								((begin + f) * -1.0f),
								e
							) / math.dot(e, e),
							0.0f
						),
						1.0f
					)
				) * -1.0f);

				distance = math.min(distance, math.dot(o, o));
			}

			distance = math.sqrt(distance);

			Span<float3> curves = stackalloc float3[2];

			curves[0] = new float3 { x = -11.0f, y = 6.0f, z = 0.0f };
			curves[1] = new float3 { x = 11.0f, y = 6.0f, z = 0.0f };

			for (int i = 1; i >= 0; i--)
			{
				float3 o = f + (curves[i] * -1.0f);
				float m = 0.0f;

				if (o.x > 0.0f)
				{
					m = math.abs(math.sqrt(math.dot(o, o)) - 2.0f);
				}
				else
				{
					if (o.y > 0.0f)
						o.y += -2.0f;
					else
						o.y += 2.0f;

					o.y += math.sqrt(math.dot(o, o));
				}

				distance = math.min(distance, m);
			}

			distance = math.pow(math.pow(distance, 8.0f) + math.pow(position.z, 8.0f), 0.125f) - 0.5f;
			hitType = (int)PixarRayHit.Letter;

			float roomDistance = math.min(
				-math.min(
					BoxTest(position, new float3 { x = -30.0f, y = -0.5f, z = -30.0f },
						new float3 { x = 30.0f, y = 18.0f, z = 30.0f }),
					BoxTest(position, new float3 { x = -25.0f, y = -17.5f, z = -25.0f },
						new float3 { x = 25.0f, y = 20.0f, z = 25.0f })
				),
				BoxTest(
					new float3 { x = math.abs(position.x) % 8, y = position.y, z = position.z },
					new float3 { x = 1.5f, y = 18.5f, z = -25.0f },
					new float3 { x = 6.5f, y = 20.0f, z = 25.0f }
				)
			);

			if (roomDistance < distance)
			{
				distance = roomDistance;
				hitType = (int)PixarRayHit.Wall;
			}

			float sun = 19.9f - position.y;

			if (sun < distance)
			{
				distance = sun;
				hitType = (int)PixarRayHit.Sun;
			}

			return distance;
		}

		private int RayMarching(float3 origin, float3 direction, ref float3 hitPosition, ref float3 hitNormal)
		{
			int hitType = (int)PixarRayHit.None;
			int noHitCount = 0;
			float distance = 0.0f;

			for (float i = 0; i < 100; i += distance)
			{
				hitPosition = (origin + direction) * i;
				distance = Sample(hitPosition, ref hitType);

				if (distance < 0.01f || ++noHitCount > 99)
				{
					hitNormal = math.normalize(
						new float3
						{
							x = Sample((hitPosition + new float3 { x = 0.01f, y = 0.0f, z = 0.0f }), ref noHitCount) -
							    distance,
							y = Sample((hitPosition + new float3 { x = 0.0f, y = 0.01f, z = 0.0f }), ref noHitCount) -
							    distance,
							z = Sample((hitPosition + new float3 { x = 0.0f, y = 0.0f, z = 0.01f }), ref noHitCount) -
							    distance
						}
					);

					return hitType;
				}
			}

			return (int)PixarRayHit.None;
		}

		private float3 Trace(float3 origin, float3 direction)
		{
			float3
				sampledPosition = new() { x = 1.0f, y = 1.0f, z = 1.0f },
				normal = new() { x = 1.0f, y = 1.0f, z = 1.0f },
				color = new() { x = 1.0f, y = 1.0f, z = 1.0f },
				attenuation = new() { x = 1.0f, y = 1.0f, z = 1.0f },
				lightDirection = math.normalize(new float3 { x = 0.6f, y = 0.6f, z = 1.0f });

			for (int bounce = 3; bounce > 0; bounce--)
			{
				PixarRayHit hitType = (PixarRayHit)RayMarching(origin, direction, ref sampledPosition, ref normal);

				switch (hitType)
				{
					case PixarRayHit.None:
						break;

					case PixarRayHit.Letter:
					{
						direction = (direction + normal) * (math.dot(normal, direction) * -2.0f);
						origin = (sampledPosition + direction) * 0.1f;
						attenuation *= 0.2f;

						break;
					}

					case PixarRayHit.Wall:
					{
						float
							incidence = math.dot(normal, lightDirection),
							p = 6.283185f * Random(),
							c = Random(),
							s = math.sqrt(1.0f - c),
							g = normal.z < 0 ? -1.0f : 1.0f,
							u = -1.0f / (g + normal.z),
							v = normal.x * normal.y * u;

						direction = (
							(
								new float3
								{
									x = v, y = g + normal.y * normal.y * u, z = -normal.y * (math.cos(p) * s)
								} +
								new float3 { x = 1.0f + g * normal.x * normal.x * u, y = g * v, z = -g * normal.x }
							) + (normal * math.sqrt(c))
						);
						origin = (sampledPosition + direction) * 0.1f;
						attenuation *= 0.2f;

						if (
							incidence > 0 &&
							RayMarching(
								(sampledPosition + normal) * 0.1f,
								lightDirection,
								ref sampledPosition,
								ref normal
							) == (int)PixarRayHit.Sun
						)
						{
							color = ((color + attenuation) * new float3 { x = 500.0f, y = 400.0f, z = 100.0f }) * incidence;
						}

						break;
					}

					case PixarRayHit.Sun:
					{
						color = (color + attenuation) * new float3 { x = 50.0f, y = 80.0f, z = 100.0f };

						goto escape;
					}
				}
			}

			escape:

			return color;
		}
	}
}
