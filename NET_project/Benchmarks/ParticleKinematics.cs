using System.Numerics;

namespace NET_project.Benchmarks;

internal struct Particle {
	public Vector3 i;
	public Vector3 v;
}

public unsafe struct ParticleKinematicsNET : IJob
{
	public uint quantity;
	public uint iterations;
	public float result;

	public void Run() {
		result = ParticleKinematics(quantity, iterations);
	}

	private static float ParticleKinematics(uint quantity, uint iterations) {
		Particle[] particles = new Particle[quantity];

		for (int i = 0; i < particles.Length; i++) {
			particles[i].i = new Vector3(i) + new Vector3(0, 1, 2);
			particles[i].v = new Vector3(1, 2, 3);
		}

		for (uint a = 0; a < iterations; a++) {
			for (int b = 0; b < particles.Length; b++) {
				particles[b].i += particles[b].v;
			}
		}

		if (particles.Length == 0)
		{
			// Help the JIT by testing for quantity == 0
			return default;
		}
		return particles[0].i.X + particles[0].i.Y + particles[0].i.Z;
	}
}

internal unsafe struct ParticleKinematicsGCC : IJob
{
	public uint quantity;
	public uint iterations;
	public float result;
	public string libName;

	public void Run()
	{
		switch (libName)
		{
			case "GCC":
				result = GccNativeBindings.benchmark_particle_kinematics(quantity, iterations);
				break;

			case "Clang":
				result = ClangNativeBindings.benchmark_particle_kinematics(quantity, iterations);
				break;

			case "MS":
				result = MsNativeBindings.benchmark_particle_kinematics(quantity, iterations);
				break;

			case "Rust":
				result = RustNativeBindings.benchmark_particle_kinematics(quantity, iterations);
				break;
		}
	}
}
