using System.Runtime.CompilerServices;

namespace NET_project.Benchmarks;

internal struct NBody {
	public double x, y, z, vx, vy, vz, mass;
}

public unsafe struct NBodyNET : IJob {
	public uint advancements;
	public double result;

	public void Run() {
		result = NBody(advancements);
	}

	private double NBody(uint advancements) {
		NBody* sun = stackalloc NBody[5];
		NBody* end = sun + 4;

		InitializeBodies(sun, end);
		Energy(sun, end);

		while (--advancements > 0) {
			Advance(sun, end, 0.01);
		}

		Energy(sun, end);

		return sun[0].x + sun[0].y;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void InitializeBodies(NBody* sun, NBody* end) {
		const double solarMass = 4 * double.Pi * double.Pi;
		const double daysPerYear = 365.24;

		unchecked {
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

			for (NBody* planet = sun + 1; planet <= end; planet++) {
				double mass = planet->mass;

				vx += planet->vx * mass;
				vy += planet->vy * mass;
				vz += planet->vz * mass;
			}

			sun->mass = solarMass;
			sun->vx = vx / -solarMass;
			sun->vy = vy / -solarMass;
			sun->vz = vz / -solarMass;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Energy(NBody* sun, NBody* end) {
		double e = 0.0;

		for (NBody* bi = sun; bi <= end; bi++) {
			double imass = bi->mass;
			double ix = bi->x;
			double iy = bi->y;
			double iz = bi->z;
			double ivx = bi->vx;
			double ivy = bi->vy;
			double ivz = bi->vz;

			e += 0.5 * imass * ((ivx * ivx) + (ivy * ivy) + (ivz * ivz));

			for (NBody* bj = bi + 1; bj <= end; bj++) {
				double jmass = bj->mass;
				double dx = ix - bj->x;
				double dy = iy - bj->y;
				double dz = iz - bj->z;

				e -= imass * jmass / double.Sqrt((dx * dx) + (dy * dy) + (dz * dz));
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static double GetD2(double dx, double dy, double dz) {
		double d2 = (dx * dx) + (dy * dy) + (dz * dz);

		return d2 * double.Sqrt(d2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Advance(NBody* sun, NBody* end, double distance) {
		for (NBody* bi = sun; bi < end; bi++) {
			double ix = bi->x;
			double iy = bi->y;
			double iz = bi->z;
			double ivx = bi->vx;
			double ivy = bi->vy;
			double ivz = bi->vz;
			double imass = bi->mass;

			for (NBody* bj = bi + 1; bj <= end; bj++) {
				double dx = bj->x - ix;
				double dy = bj->y - iy;
				double dz = bj->z - iz;
				double jmass = bj->mass;
				double mag = distance / GetD2(dx, dy, dz);

				bj->vx = bj->vx - (dx * imass * mag);
				bj->vy = bj->vy - (dy * imass * mag);
				bj->vz = bj->vz - (dz * imass * mag);
				ivx += dx * jmass * mag;
				ivy += dy * jmass * mag;
				ivz += dz * jmass * mag;
			}

			bi->vx = ivx;
			bi->vy = ivy;
			bi->vz = ivz;
			bi->x = ix + (ivx * distance);
			bi->y = iy + (ivy * distance);
			bi->z = iz + (ivz * distance);
		}

		end->x = end->x + (end->vx * distance);
		end->y = end->y + (end->vy * distance);
		end->z = end->z + (end->vz * distance);
	}
}

internal unsafe struct NBodyGCC : IJob {
	public uint advancements;
	public double result;
	public string libName;

	public void Run() {
		switch (libName)
		{
			case "GCC":
				result = GccNativeBindings.benchmark_nbody(advancements);
				break;
			
			case "Clang":
				result = ClangNativeBindings.benchmark_nbody(advancements);
				break;
			
			case "MS":
				result = MsNativeBindings.benchmark_nbody(advancements);
				break;
			
			case "Rust":
				result = RustNativeBindings.benchmark_nbody(advancements);
				break;
		}
	}
}
