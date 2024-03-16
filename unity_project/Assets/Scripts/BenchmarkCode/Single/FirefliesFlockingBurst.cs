using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace BenchmarkCode.Single
{
	internal struct BoidBurst
	{
		public float3 position, velocity, acceleration;
	}

	[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
	public struct FirefliesFlockingBurst : IJob
	{
		public int boids;
		public uint lifetime;
		public float result;

		public void Execute()
		{
			result = FirefliesFlocking(boids, lifetime);
		}

		private uint parkMiller;
		private float maxSpeed;
		private float maxForce;
		private float separationDistance;
		private float neighbourDistance;

		private float FirefliesFlocking(int boids, uint lifetime)
		{
			parkMiller = 666;
			maxSpeed = 1.0f;
			maxForce = 0.03f;
			separationDistance = 15.0f;
			neighbourDistance = 30.0f;

			NativeArray<BoidBurst> fireflies = new NativeArray<BoidBurst>(boids, Allocator.Persistent);

			for (int i = 0; i < boids; ++i)
			{
				BoidBurst newFirefly = new();
				newFirefly.position = new float3 { x = Random(), y = Random(), z = Random() };
				newFirefly.velocity = new float3 { x = Random(), y = Random(), z = Random() };
				newFirefly.acceleration = new float3 { x = 0.0f, y = 0.0f, z = 0.0f };
				
				fireflies[i] = newFirefly;
			}

			for (uint i = 0; i < lifetime; ++i)
			{
				// Update
				for (int boid = 0; boid < boids; ++boid)
				{
					BoidBurst firefly = fireflies[boid];
					
					firefly.velocity += fireflies[boid].acceleration;

					float speed = math.length(fireflies[boid].velocity);

					if (speed > maxSpeed)
					{
						firefly.velocity /= speed;
						firefly.velocity *= maxSpeed;
					}

					firefly.position += fireflies[boid].velocity;
					firefly.acceleration *= maxSpeed;

					fireflies[boid] = firefly;
				}

				// Separation
				for (int boid = 0; boid < boids; ++boid)
				{
					BoidBurst currentFirefly = fireflies[boid];
					
					float3 separation = default(float3);
					int count = 0;

					for (int target = 0; target < boids; ++target)
					{
						float3 position = currentFirefly.position;

						position -= fireflies[target].position;

						float distance = math.length(position);

						if (distance > 0.0f && distance < separationDistance)
						{
							position = math.normalize(position);
							position /= distance;

							separation = position;
							count++;
						}
					}

					if (count > 0)
					{
						separation /= count;
						separation = math.normalize(separation);
						separation *= maxSpeed;
						separation -= currentFirefly.velocity;

						float force = math.length(separation);

						if (force > maxForce)
						{
							separation /= force;
							separation *= maxForce;
						}

						separation /= 1.5f;
						currentFirefly.acceleration += separation;
					}

					fireflies[boid] = currentFirefly;
				}

				// Cohesion
				for (int boid = 0; boid < boids; ++boid)
				{
					BoidBurst currentFirefly = fireflies[boid];
					
					float3 cohesion = default(float3);
					int count = 0;

					for (int target = 0; target < boids; ++target)
					{
						float3 position = currentFirefly.position;

						position -= fireflies[target].position;

						float distance = math.length(position);

						if (distance > 0.0f && distance < neighbourDistance)
						{
							cohesion = currentFirefly.position;
							count++;
						}
					}

					if (count > 0)
					{
						cohesion /= count;
						cohesion -= currentFirefly.position;
						cohesion = math.normalize(cohesion);
						cohesion *= maxSpeed;
						cohesion -= currentFirefly.velocity;

						float force = math.length(cohesion);

						if (force > maxForce)
						{
							cohesion /= force;
							cohesion *= maxForce;
						}

						currentFirefly.acceleration += cohesion;
					}

					fireflies[boid] = currentFirefly;
				}
			}

			fireflies.Dispose();

			return parkMiller;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private float Random()
		{
			parkMiller = (uint)(((ulong)parkMiller * 48271u) % 0x7fffffff);

			return parkMiller / 10000000.0f;
		}
	}
}
