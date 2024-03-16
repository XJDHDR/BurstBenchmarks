using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;

namespace BenchmarkCode.Single
{
	internal struct BoidUnity
	{
		public Vector3 position, velocity, acceleration;
	}

	public unsafe struct FirefliesFlockingUnity : IJob
	{
		public uint boids;
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

		private float FirefliesFlocking(uint boids, uint lifetime)
		{
			parkMiller = 666;
			maxSpeed = 1.0f;
			maxForce = 0.03f;
			separationDistance = 15.0f;
			neighbourDistance = 30.0f;

			BoidUnity[] fireflies = new BoidUnity[boids];

			for (uint i = 0; i < boids; ++i)
			{
				fireflies[i].position = new Vector3 { x = Random(), y = Random(), z = Random() };
				fireflies[i].velocity = new Vector3 { x = Random(), y = Random(), z = Random() };
				fireflies[i].acceleration = new Vector3 { x = 0.0f, y = 0.0f, z = 0.0f };
			}

			for (uint i = 0; i < lifetime; ++i)
			{
				// Update
				for (uint boid = 0; boid < boids; ++boid)
				{
					fireflies[boid].velocity += fireflies[boid].acceleration;

					float speed = fireflies[boid].velocity.magnitude;

					if (speed > maxSpeed) {
						fireflies[boid].velocity /= speed;
						fireflies[boid].velocity *= maxSpeed;
					}

					fireflies[boid].position += fireflies[boid].velocity;
					fireflies[boid].acceleration *= maxSpeed;
				}

				// Separation
				for (uint boid = 0; boid < boids; ++boid)
				{
					Vector3 separation = default(Vector3);
					int count = 0;

					for (uint target = 0; target < boids; ++target)
					{
						Vector3 position = fireflies[boid].position;

						position -= fireflies[target].position;

						float distance = position.magnitude;

						if (distance > 0.0f && distance < separationDistance)
						{
							position.Normalize();
							position /= distance;

							separation = position;
							count++;
						}
					}

					if (count > 0)
					{
						separation /= count;
						separation.Normalize();
						separation *= maxSpeed;
						separation -= fireflies[boid].velocity;

						float force = separation.magnitude;

						if (force > maxForce)
						{
							separation /= force;
							separation *= maxForce;
						}

						separation *= 1.5f;
						fireflies[boid].acceleration += separation;
					}
				}

				// Cohesion
				for (uint boid = 0; boid < boids; ++boid)
				{
					Vector3 cohesion = default(Vector3);
					int count = 0;

					for (uint target = 0; target < boids; ++target)
					{
						Vector3 position = fireflies[boid].position;

						position -= fireflies[target].position;

						float distance = position.magnitude;

						if (distance > 0.0f && distance < neighbourDistance)
						{
							cohesion = fireflies[boid].position;
							count++;
						}
					}

					if (count > 0)
					{
						cohesion /= count;
						cohesion -= fireflies[boid].position;
						cohesion.Normalize();
						cohesion *= maxSpeed;
						cohesion -= fireflies[boid].velocity;

						float force = cohesion.magnitude;

						if (force > maxForce)
						{
							cohesion /= force;
							cohesion *= maxForce;
						}

						fireflies[boid].acceleration += cohesion;
					}
				}
			}

			return parkMiller;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private float Random()
		{
			parkMiller = (uint)(((ulong)parkMiller * 48271u) % 0x7fffffff);

			return parkMiller / 10000000.0f;
		}
	}

	internal struct FirefliesFlockingGCC : IJob
	{
		public uint boids;
		public uint lifetime;
		public float result;

		public void Execute()
		{
			result = NativeBindings.benchmark_fireflies_flocking(boids, lifetime);
		}
	}
}
