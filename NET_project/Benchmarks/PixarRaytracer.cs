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

	public void Run() {
		result = PixarRaytracer(width, height, samples);
	}

	private static uint marsagliaZ;
	private static uint marsagliaW;

	private static float PixarRaytracer(uint width, uint height, uint samples)
	{
		marsagliaZ = 666;
		marsagliaW = 999;

		Vector3 position = new Vector3(-22.0f, 5.0f, 25.0f);
		Vector3 goal = new Vector3(-3.0f, 4.0f, 0.0f);

		goal = Vector3.Normalize(goal) - position;

		Vector3 left = new Vector3(goal.Z, 0, goal.X);

		left = Vector3.Normalize(left) / width;

		Vector3 up = Vector3.Cross(goal, left);
		Vector3 color = Vector3.Zero;
		Vector3 adjust;

		for (uint y = height; y > 0; y--) {
			for (uint x = width; x > 0; x--) {
				for (uint p = samples; p > 0; p--) {
					color += Trace(position, Vector3.Normalize((goal + left) * (x - (width / 2) + Random())) + (up * (y - (height / 2) + Random())));
				}

				color *= (1.0f / samples) + (14.0f / 241.0f);
				adjust = color + Vector3.One;
				color /= adjust;

				color *= 255.0f;
			}
		}

		return color.X + color.Y + color.Z;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static float Min(float left, float right) {
		return left < right ? left : right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static float BoxTest(Vector3 position, Vector3 lowerLeft, Vector3 upperRight) {
		lowerLeft = -(position + lowerLeft);
		upperRight = -(upperRight + position);

		Vector3 vmin = Vector3.Min(lowerLeft, upperRight);
		return -Min(Min(vmin.X, vmin.Y), vmin.Z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static float Random() {
		marsagliaZ = (36969 * (marsagliaZ & 65535)) + (marsagliaZ >> 16);
		marsagliaW = (18000 * (marsagliaW & 65535)) + (marsagliaW >> 16);

		return ((marsagliaZ << 16) + marsagliaW) * 2.0f / 10000000000.0f;
	}

	private static ReadOnlySpan<float> letters => [
		/* P */ 53, 79, 53, 95, 53, 87, 57, 87, 53, 95, 57, 95,
		/* I */ 65, 79, 69, 79, 67, 79, 67, 95, 65, 95, 69, 95,
		/* X */ 73, 79, 81, 95, 73, 95, 81, 79,
		/* A */ 85, 79, 89, 95, 89, 95, 93, 79, 87, 87, 91, 87,
		/* R */ 97, 79, 97, 95, 97, 87, 101, 87, 97, 95, 101, 95, 99, 87, 105, 79
	];

	private static float Sample(Vector3 position, int* hitType) {
		float distance = 1e9f;
		Vector3 f = position;

		f.Z = 0.0f;

		for (int i = 0; i < letters.Length; i += 4) {
			// The JIT isn't quite smart enough to elide the bounds
			// checks and so we'll help it out a little bit instead.
			ref float letter = ref Unsafe.AsRef(in letters[i]);

			Vector3 begin = new Vector3((Unsafe.As<float, Vector2>(ref letter) - new Vector2(79.0f)) * 0.5f, 0.0f);
			Vector3 e = new Vector3((Unsafe.As<float, Vector2>(ref Unsafe.Add(ref letter, 2)) - new Vector2(79.0f)) * 0.5f, 0.0f) - begin;
			Vector3 o = -(f + ((begin + e) * Min(-Min(Vector3.Dot(-(begin + f), e) / e.LengthSquared(), 0.0f), 1.0f)));

			distance = Min(distance, o.LengthSquared());
		}

		distance = float.Sqrt(distance);

		Span<Vector3> curves = [
			new Vector3(11.0f, 6.0f, 0.0f),
			new Vector3(-11.0f, 6.0f, 0.0f),
		];

		for (int i = 0; i < curves.Length; i++) {
			Vector3 o = f - curves[i];
			float m = 0.0f;

			if (o.X > 0.0f) {
				m = float.Abs(o.Length() - 2.0f);
			} else {
				if (o.Y > 0.0f)
					o.Y += -2.0f;
				else
					o.Y += 2.0f;

				o.Y += o.Length();
			}

			distance = Min(distance, m);
		}

		distance = float.Pow(float.Pow(distance, 8.0f) + float.Pow(position.Z, 8.0f), 0.125f) - 0.5f;
		*hitType = (int)PixarRayHit.Letter;

		float roomDistance = Min(-Min(BoxTest(position, new Vector3(-30.0f, -0.5f, -30.0f), new Vector3(30.0f, 18.0f, 30.0f)), BoxTest(position, new Vector3(-25.0f, -17.5f, -25.0f), new Vector3(25.0f, 20.0f, 25.0f))), BoxTest(new Vector3(float.Abs(position.X) % 8, position.Y, position.Z), new Vector3(1.5f, 18.5f, -25.0f), new Vector3(6.5f, 20.0f, 25.0f)));

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

	private static int RayMarching(Vector3 origin, Vector3 direction, Vector3* hitPosition, Vector3* hitNormal) {
		int hitType = (int)PixarRayHit.None;
		int noHitCount = 0;
		float distance = 0.0f;

		for (float i = 0; i < 100; i += distance) {
			*hitPosition = (origin + direction) * i;
			distance = Sample(*hitPosition, &hitType);

			if (distance < 0.01f || ++noHitCount > 99) {
				*hitNormal = Vector3.Normalize(new Vector3(Sample(*hitPosition + new Vector3(0.01f, 0.0f, 0.0f), &noHitCount) - distance, Sample(*hitPosition + new Vector3(0.0f, 0.01f, 0.0f), &noHitCount) - distance, Sample(*hitPosition + new Vector3(0.0f, 0.0f, 0.01f), &noHitCount) - distance));

				return hitType;
			}
		}

		return (int)PixarRayHit.None;
	}

	private static Vector3 Trace(Vector3 origin, Vector3 direction) {
		Vector3 sampledPosition = Vector3.One;
		Vector3 normal = Vector3.One;
		Vector3 color = Vector3.One;
		Vector3 attenuation = Vector3.One;
		Vector3 lightDirection = Vector3.Normalize(new Vector3(0.6f, 0.6f, 1.0f));

		for (int bounce = 3; bounce > 0; bounce--) {
			PixarRayHit hitType = (PixarRayHit)RayMarching(origin, direction, &sampledPosition, &normal);

			switch (hitType) {
				case PixarRayHit.None:
					break;

				case PixarRayHit.Letter:
				{
					direction = (direction + normal) * (Vector3.Dot(normal, direction) * -2.0f);
					origin = (sampledPosition + direction) * 0.1f;
					attenuation *= 0.2f;

					break;
				}

				case PixarRayHit.Wall:
				{
					float incidence = Vector3.Dot(normal, lightDirection);
					float p = 6.283185f * Random();
					float c = Random();
					float s = float.Sqrt(1.0f - c);
					float g = (normal.Z < 0) ? -1.0f : 1.0f;
					float u = -1.0f / (g + normal.Z);
					float v = normal.X * normal.Y * u;

					direction = new Vector3(v, g + (normal.Y * normal.Y * u), -normal.Y * (float.Cos(p) * s)) + new Vector3(1.0f + (g * normal.X * normal.X * u), g * v, -g * normal.X) + (normal * float.Sqrt(c));
					origin = (sampledPosition + direction) * 0.1f;
					attenuation *= 0.2f;

					if (incidence > 0 && RayMarching((sampledPosition + normal) * 0.1f, lightDirection, &sampledPosition, &normal) == (int)PixarRayHit.Sun)
						color = (color + attenuation) * new Vector3(500.0f, 400.0f, 100.0f) * incidence;

					break;
				}

				case PixarRayHit.Sun:
				{
					color = (color + attenuation) * new Vector3(50.0f, 80.0f, 100.0f);
					return color;
				}
			}
		}

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
