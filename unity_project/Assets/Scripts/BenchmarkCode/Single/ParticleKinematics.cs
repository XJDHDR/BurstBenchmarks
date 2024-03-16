using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace BenchmarkCode.Single
{
	internal struct Particle
	{
		public float x, y, z, vx, vy, vz;
	}

	public unsafe struct ParticleKinematicsUnity : IJob
	{
		public uint quantity;
		public uint iterations;
		public float result;

		public void Execute()
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
					Particle p = particles[b];

					p.x += p.vx;
					p.y += p.vy;
					p.z += p.vz;
				}
			}

			Particle particle = new Particle { x = particles[0].x, y = particles[0].y, z = particles[0].z };

			return particle.x + particle.y + particle.z;
		}
	}

	internal struct ParticleKinematicsGCC : IJob
	{
		public uint quantity;
		public uint iterations;
		public float result;

		public void Execute()
		{
			result = NativeBindings.benchmark_particle_kinematics(quantity, iterations);
		}
	}
}
