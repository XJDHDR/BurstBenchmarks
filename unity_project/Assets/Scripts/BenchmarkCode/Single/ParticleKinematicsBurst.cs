using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace BenchmarkCode.Single
{
	[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
	public unsafe struct ParticleKinematicsBurst : IJob
	{
		public int quantity;
		public uint iterations;
		public float result;

		public void Execute()
		{
			result = ParticleKinematics(quantity, iterations);
		}

		private float ParticleKinematics(int quantity, uint iterations)
		{
			NativeArray<Particle> particles = new NativeArray<Particle>(quantity, Allocator.Persistent);

			for (int i = 0; i < quantity; ++i)
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
				for (int b = 0, c = quantity; b < c; ++b)
				{
					Particle p = particles[b];

					p.x += p.vx;
					p.y += p.vy;
					p.z += p.vz;
				}
			}

			Particle particle = new Particle { x = particles[0].x, y = particles[0].y, z = particles[0].z };

			particles.Dispose();

			return particle.x + particle.y + particle.z;
		}
	}
}
