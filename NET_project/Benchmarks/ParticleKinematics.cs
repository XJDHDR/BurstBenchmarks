namespace NET_project.Benchmarks;

internal struct Particle {
	public float x, y, z, vx, vy, vz;
}

public unsafe struct ParticleKinematicsNET : IJob
{
	public uint quantity;
	public uint iterations;
	public float result;

	public void Run()
	{
		result = ParticleKinematics(quantity, iterations);
	}

	private float ParticleKinematics(uint quantity, uint iterations)
	{
		Particle[] particles = new Particle[quantity];

		for (uint i = 0; i < quantity; ++i)
		{
			particles[i].x = i;
			particles[i].y = (i + 1);
			particles[i].z = (i + 2);
			particles[i].vx = 1.0f;
			particles[i].vy = 2.0f;
			particles[i].vz = 3.0f;
		}

		for (uint a = 0; a < iterations; ++a)
		{
			for (uint b = 0, c = quantity; b < c; ++b)
			{
				particles[b].x += particles[b].vx;
				particles[b].y += particles[b].vy;
				particles[b].z += particles[b].vz;
			}
		}

		Particle particle = new Particle { x = particles[0].x, y = particles[0].y, z = particles[0].z };

		return particle.x + particle.y + particle.z;
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
