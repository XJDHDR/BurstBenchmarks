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
				Particle newParticle = new Particle()
				{
					x = i,
					y = (i + 1),
					z = (i + 2),
					vx = 1.0f,
					vy = 2.0f,
					vz = 3.0f,
				};
				
				particles[i] = newParticle;
			}

			for (uint a = 0; a < iterations; ++a)
			{
				for (uint b = 0, c = quantity; b < c; ++b)
				{
					Particle p = particles[b];
					
					p.x += p.vx;
					p.y += p.vy;
					p.z += p.vz;
					
					particles[b] = p;
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
