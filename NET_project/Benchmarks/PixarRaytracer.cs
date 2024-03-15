using System.Numerics;
using System.Runtime.CompilerServices;

namespace NET_project.Benchmarks;

public enum PixarRayHit {
	None = 0,
	Letter = 1,
	Wall = 2,
	Sun = 3
}

public unsafe struct PixarRaytracerNET : IJob
{
	public uint width;
	public uint height;
	public uint samples;
	public float result;

	public void Run()
	{
		result = PixarRaytracer(width, height, samples);
	}

	private uint marsagliaZ, marsagliaW;

	private float PixarRaytracer(uint width, uint height, uint samples)
	{
		marsagliaZ = 666;
		marsagliaW = 999;

		Vector3 position = new Vector3 { X = -22.0f, Y = 5.0f, Z = 25.0f };
		Vector3 goal = new Vector3 { X = -3.0f, Y = 4.0f, Z = 0.0f };

		goal = Vector3.Add(Inverse(goal), Vector3.Multiply(position, -1.0f));

		Vector3 left = new Vector3 { X = goal.Z, Y = 0, Z = goal.X };

		left = Vector3.Multiply(Inverse(left), 1.0f / width);

		Vector3 up = Vector3.Cross(goal, left);
		Vector3 color = default(Vector3);
		Vector3 adjust = default(Vector3);

		for (uint y = height; y > 0; y--)
		{
			for (uint x = width; x > 0; x--)
			{
				for (uint p = samples; p > 0; p--)
				{
					color = Vector3.Add(color, Trace(position, Vector3.Add(Inverse(Vector3.Multiply(Vector3.Add(goal, left), x - width / 2 + Random())), Vector3.Multiply(up, y - height / 2 + Random()))));
				}

				color = Vector3.Multiply(color, (1.0f / samples) + 14.0f / 241.0f);
				adjust = AddFloat(color, 1.0f);
				color = new Vector3 {
					X = color.X / adjust.X,
					Y = color.Y / adjust.Y,
					Z = color.Z / adjust.Z
				};

				color = Vector3.Multiply(color, 255.0f);
			}
		}

		return color.X + color.Y + color.Z;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Vector3 Inverse(Vector3 vector) {
		return Vector3.Multiply(vector, 1 / (float)Math.Sqrt(Vector3.Dot(vector, vector)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Vector3 AddFloat(Vector3 vector, float value) {
		vector.X += value;
		vector.Y += value;
		vector.Z += value;

		return vector;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private float Min(float left, float right) {
		return left < right ? left : right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private float BoxTest(Vector3 position, Vector3 lowerLeft, Vector3 upperRight) {
		lowerLeft = Vector3.Multiply(Vector3.Add(position, lowerLeft), -1);
		upperRight = Vector3.Multiply(Vector3.Add(upperRight, position), -1);

		return -Min(Min(Min(lowerLeft.X, upperRight.X), Min(lowerLeft.Y, upperRight.Y)), Min(lowerLeft.Z, upperRight.Z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private float Random() {
		marsagliaZ = 36969 * (marsagliaZ & 65535) + (marsagliaZ >> 16);
		marsagliaW = 18000 * (marsagliaW & 65535) + (marsagliaW >> 16);

		return ((marsagliaZ << 16) + marsagliaW) * 2.0f / 10000000000.0f;
	}

	private float Sample(Vector3 position, int* hitType) {
		const int size = 60;

		float distance = 1e9f;
		Vector3 f = position;
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

		f.Z = 0.0f;

		for (int i = 0; i < size; i += 4) {
			Vector3 begin = Vector3.Multiply(new Vector3 { X = letters[i] - 79.0f, Y = letters[i + 1] - 79.0f, Z = 0.0f }, 0.5f);
			Vector3 e = Vector3.Add(Vector3.Multiply(new Vector3 { X = letters[i + 2] - 79.0f, Y = letters[i + 3] - 79.0f, Z = 0.0f }, 0.5f), Vector3.Multiply(begin, -1.0f));
			Vector3 o = Vector3.Multiply(Vector3.Add(f, Vector3.Multiply(Vector3.Add(begin, e), Min(-Min(Vector3.Dot(Vector3.Multiply(Vector3.Add(begin, f), -1.0f), e) / Vector3.Dot(e, e), 0.0f), 1.0f))), -1.0f);

			distance = Min(distance, Vector3.Dot(o, o));
		}

		distance = (float)Math.Sqrt(distance);

		Span<Vector3> curves = stackalloc Vector3[2];

		curves[0] = new Vector3 { X = -11.0f, Y = 6.0f, Z = 0.0f };
		curves[1] = new Vector3 { X = 11.0f, Y = 6.0f, Z = 0.0f };

		for (int i = 1; i >= 0; i--) {
			Vector3 o = Vector3.Add(f, Vector3.Multiply(curves[i], -1.0f));
			float m = 0.0f;

			if (o.X > 0.0f) {
				m = (float)Math.Abs(Math.Sqrt(Vector3.Dot(o, o)) - 2.0f);
			} else {
				if (o.Y > 0.0f)
					o.Y += -2.0f;
				else
					o.Y += 2.0f;

				o.Y += (float)Math.Sqrt(Vector3.Dot(o, o));
			}

			distance = Min(distance, m);
		}

		distance = (float)Math.Pow(Math.Pow(distance, 8.0f) + Math.Pow(position.Z, 8.0f), 0.125f) - 0.5f;
		*hitType = (int)PixarRayHit.Letter;

		float roomDistance = Min(-Min(BoxTest(position, new Vector3 { X = -30.0f, Y = -0.5f, Z = -30.0f }, new Vector3 { X = 30.0f, Y = 18.0f, Z = 30.0f }), BoxTest(position, new Vector3 { X = -25.0f, Y = -17.5f, Z = -25.0f }, new Vector3 { X = 25.0f, Y = 20.0f, Z = 25.0f })), BoxTest( new Vector3 { X = Math.Abs(position.X) % 8, Y = position.Y, Z = position.Z }, new Vector3 { X = 1.5f, Y = 18.5f, Z = -25.0f }, new Vector3 { X = 6.5f, Y = 20.0f, Z = 25.0f }));

		if (roomDistance < distance) {
			distance = roomDistance;
			*hitType = (int)PixarRayHit.Wall;
		}

		float sun = 19.9f - position.Y;

		if (sun < distance) {
			distance = sun;
			*hitType = (int)PixarRayHit.Sun;
		}

		return distance;
	}

	private int RayMarching(Vector3 origin, Vector3 direction, Vector3* hitPosition, Vector3* hitNormal) {
		int hitType = (int)PixarRayHit.None;
		int noHitCount = 0;
		float distance = 0.0f;

		for (float i = 0; i < 100; i += distance) {
			*hitPosition = Vector3.Multiply(Vector3.Add(origin, direction), i);
			distance = Sample(*hitPosition, &hitType);

			if (distance < 0.01f || ++noHitCount > 99) {
				*hitNormal = Inverse(new Vector3 { X = Sample(Vector3.Add(*hitPosition, new Vector3 { X = 0.01f, Y = 0.0f, Z = 0.0f }), &noHitCount) - distance, Y = Sample(Vector3.Add(*hitPosition, new Vector3 { X = 0.0f, Y = 0.01f, Z = 0.0f }), &noHitCount) - distance, Z = Sample(Vector3.Add(*hitPosition, new Vector3 { X = 0.0f, Y = 0.0f, Z = 0.01f }), &noHitCount) - distance });

				return hitType;
			}
		}

		return (int)PixarRayHit.None;
	}

	private Vector3 Trace(Vector3 origin, Vector3 direction) {
		Vector3
			sampledPosition = new Vector3 { X = 1.0f, Y = 1.0f, Z = 1.0f },
			normal = new Vector3 { X = 1.0f, Y = 1.0f, Z = 1.0f },
			color = new Vector3 { X = 1.0f, Y = 1.0f, Z = 1.0f },
			attenuation = new Vector3 { X = 1.0f, Y = 1.0f, Z = 1.0f },
			lightDirection = Inverse(new Vector3 { X = 0.6f, Y = 0.6f, Z = 1.0f });

		for (int bounce = 3; bounce > 0; bounce--)
		{
			PixarRayHit hitType = (PixarRayHit)RayMarching(origin, direction, &sampledPosition, &normal);

			switch (hitType) {
				case PixarRayHit.None:
					break;

				case PixarRayHit.Letter:
				{
					direction = Vector3.Multiply(Vector3.Add(direction, normal), Vector3.Dot(normal, direction) * -2.0f);
					origin = Vector3.Multiply(Vector3.Add(sampledPosition, direction), 0.1f);
					attenuation = Vector3.Multiply(attenuation, 0.2f);

					break;
				}

				case PixarRayHit.Wall:
				{
					float
						incidence = Vector3.Dot(normal, lightDirection),
						p = 6.283185f * Random(),
						c = Random(),
						s = (float)Math.Sqrt(1.0f - c),
						g = normal.Z < 0 ? -1.0f : 1.0f,
						u = -1.0f / (g + normal.Z),
						v = normal.X * normal.Y * u;

					direction = Vector3.Add(Vector3.Add(new Vector3 { X = v, Y = g + normal.Y * normal.Y * u, Z = -normal.Y * ((float)Math.Cos(p) * s) }, new Vector3 { X = 1.0f + g * normal.X * normal.X * u, Y = g * v, Z = -g * normal.X }), Vector3.Multiply(normal, (float)Math.Sqrt(c)));
					origin = Vector3.Multiply(Vector3.Add(sampledPosition, direction), 0.1f);
					attenuation = Vector3.Multiply(attenuation, 0.2f);

					if (incidence > 0 && RayMarching(Vector3.Multiply(Vector3.Add(sampledPosition, normal), 0.1f), lightDirection, &sampledPosition, &normal) == (int)PixarRayHit.Sun)
						color = Vector3.Multiply(Vector3.Multiply(Vector3.Add(color, attenuation), new Vector3 { X = 500.0f, Y = 400.0f, Z = 100.0f }), incidence);

					break;
				}

				case PixarRayHit.Sun:
				{
					color = Vector3.Multiply(Vector3.Add(color, attenuation), new Vector3 { X = 50.0f, Y = 80.0f, Z = 100.0f });

					goto escape;
				}
			}
		}

		escape:

		return color;
	}
}

internal unsafe struct PixarRaytracerGCC : IJob {
	public uint width;
	public uint height;
	public uint samples;
	public float result;
	public string libName;

	public void Run() {
		switch (libName)
		{
			case "GCC":
				result = GccNativeBindings.benchmark_pixar_raytracer(width, height, samples);
				break;

			case "Clang":
				result = ClangNativeBindings.benchmark_pixar_raytracer(width, height, samples);
				break;

			case "MS":
				result = MsNativeBindings.benchmark_pixar_raytracer(width, height, samples);
				break;

			case "Rust":
				result = RustNativeBindings.benchmark_pixar_raytracer(width, height, samples);
				break;
		}
	}
}
