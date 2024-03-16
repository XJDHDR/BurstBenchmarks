using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;

namespace BenchmarkCode.Double
{
	internal struct NBody
	{
		public double x, y, z, vx, vy, vz, mass;
	}

	[BurstCompile(CompileSynchronously = true)]
	public unsafe struct NBodyBurst : IJob
	{
		public uint advancements;
		public double result;

		public void Execute()
		{
			result = NBody(advancements);
		}

		private double NBody(uint advancements)
		{
			Span<NBody> sun = stackalloc NBody[5];

			InitializeBodies(sun);
			Energy(sun);

			while (--advancements > 0)
			{
				Advance(sun, 0.01);
			}

			Energy(sun);

			return sun[0].x + sun[0].y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void InitializeBodies(Span<NBody> sun)
		{
			const double pi = 3.141592653589793;
			const double solarMass = 4 * pi * pi;
			const double daysPerYear = 365.24;

			unchecked
			{
				sun[1] = new NBody { // Jupiter
					x = 4.84143144246472090e+00,
					y = -1.16032004402742839e+00,
					z = -1.03622044471123109e-01,
					vx = 1.66007664274403694e-03 * daysPerYear,
					vy = 7.69901118419740425e-03 * daysPerYear,
					vz = -6.90460016972063023e-05 * daysPerYear,
					mass = 9.54791938424326609e-04 * solarMass
				};

				sun[2] = new NBody { // Saturn
					x = 8.34336671824457987e+00,
					y = 4.12479856412430479e+00,
					z = -4.03523417114321381e-01,
					vx = -2.76742510726862411e-03 * daysPerYear,
					vy = 4.99852801234917238e-03 * daysPerYear,
					vz = 2.30417297573763929e-05 * daysPerYear,
					mass = 2.85885980666130812e-04 * solarMass
				};

				sun[3] = new NBody { // Uranus
					x = 1.28943695621391310e+01,
					y = -1.51111514016986312e+01,
					z = -2.23307578892655734e-01,
					vx = 2.96460137564761618e-03 * daysPerYear,
					vy = 2.37847173959480950e-03 * daysPerYear,
					vz = -2.96589568540237556e-05 * daysPerYear,
					mass = 4.36624404335156298e-05 * solarMass
				};

				sun[4] = new NBody { // Neptune
					x = 1.53796971148509165e+01,
					y = -2.59193146099879641e+01,
					z = 1.79258772950371181e-01,
					vx = 2.68067772490389322e-03 * daysPerYear,
					vy = 1.62824170038242295e-03 * daysPerYear,
					vz = -9.51592254519715870e-05 * daysPerYear,
					mass = 5.15138902046611451e-05 * solarMass
				};

				double vx = 0, vy = 0, vz = 0;

				for (int i = 1; i <= sun.Length; ++i)
				{
					double mass = sun[i].mass;

					vx += sun[i].vx * mass;
					vy += sun[i].vy * mass;
					vz += sun[i].vz * mass;
				}

				sun[0].mass = solarMass;
				sun[0].vx = vx / -solarMass;
				sun[0].vy = vy / -solarMass;
				sun[0].vz = vz / -solarMass;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Energy(Span<NBody> sun)
		{
			unchecked
			{
				double e = 0.0;

				for (int bi = 0; bi < sun.Length; ++bi)
				{
					double
						imass = sun[bi].mass,
						ix = sun[bi].x,
						iy = sun[bi].y,
						iz = sun[bi].z,
						ivx = sun[bi].vx,
						ivy = sun[bi].vy,
						ivz = sun[bi].vz;

					e += 0.5 * imass * (ivx * ivx + ivy * ivy + ivz * ivz);

					for (int bj = bi + 1; bj < sun.Length; ++bj)
					{
						double
							jmass = sun[bj].mass,
							dx = ix - sun[bj].x,
							dy = iy - sun[bj].y,
							dz = iz - sun[bj].z;

						e -= imass * jmass / math.sqrt(dx * dx + dy * dy + dz * dz);
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private double GetD2(double dx, double dy, double dz)
		{
			double d2 = dx * dx + dy * dy + dz * dz;

			return d2 * math.sqrt(d2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Advance(Span<NBody> sun, double distance)
		{
			unchecked
			{
				for (int bi = 0; bi < (sun.Length - 1); ++bi)
				{
					double
						ix = sun[bi].x,
						iy = sun[bi].y,
						iz = sun[bi].z,
						ivx = sun[bi].vx,
						ivy = sun[bi].vy,
						ivz = sun[bi].vz,
						imass = sun[bi].mass;

					for (int bj = bi + 1; bj < sun.Length; ++bj)
					{
						double
							dx = sun[bj].x - ix,
							dy = sun[bj].y - iy,
							dz = sun[bj].z - iz,
							jmass = sun[bj].mass,
							mag = distance / GetD2(dx, dy, dz);

						sun[bj].vx -= dx * imass * mag;
						sun[bj].vy -= dy * imass * mag;
						sun[bj].vz -= dz * imass * mag;
						ivx += dx * jmass * mag;
						ivy += dy * jmass * mag;
						ivz += dz * jmass * mag;
					}

					sun[bi].vx = ivx;
					sun[bi].vy = ivy;
					sun[bi].vz = ivz;
					sun[bi].x = ix + ivx * distance;
					sun[bi].y = iy + ivy * distance;
					sun[bi].z = iz + ivz * distance;
				}

				sun[4].x += sun[4].vx * distance;
				sun[4].y += sun[4].vy * distance;
				sun[4].z += sun[4].vz * distance;
			}
		}
	}

	[BurstCompile(CompileSynchronously = true)]
	internal unsafe struct NBodyGCC : IJob
	{
		public uint advancements;
		public double result;

		public void Execute()
		{
			result = NativeBindings.benchmark_nbody(advancements);
		}
	}
}
