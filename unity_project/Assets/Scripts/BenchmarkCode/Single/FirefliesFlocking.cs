using System.Numerics;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace BenchmarkCode.Single
{
	internal struct Boid
	{
		public Vector position, velocity, acceleration;
	}

	[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
	public unsafe struct FirefliesFlockingBurst : IJob
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

			Boid* fireflies = (Boid*)UnsafeUtility.Malloc(boids * sizeof(Boid), 16, Allocator.Persistent);

			for (uint i = 0; i < boids; ++i)
			{
				fireflies[i].position = new Vector { x = Random(), y = Random(), z = Random() };
				fireflies[i].velocity = new Vector { x = Random(), y = Random(), z = Random() };
				fireflies[i].acceleration = new Vector { x = 0.0f, y = 0.0f, z = 0.0f };
			}

			for (uint i = 0; i < lifetime; ++i)
			{
				// Update
				for (uint boid = 0; boid < boids; ++boid)
				{
					Add(&fireflies[boid].velocity, &fireflies[boid].acceleration);

					float speed = Length(&fireflies[boid].velocity);

					if (speed > maxSpeed) {
						Divide(&fireflies[boid].velocity, speed);
						Multiply(&fireflies[boid].velocity, maxSpeed);
					}

					Add(&fireflies[boid].position, &fireflies[boid].velocity);
					Multiply(&fireflies[boid].acceleration, maxSpeed);
				}

				// Separation
				for (uint boid = 0; boid < boids; ++boid)
				{
					Vector separation = default(Vector);
					int count = 0;

					for (uint target = 0; target < boids; ++target)
					{
						Vector position = fireflies[boid].position;

						Subtract(&position, &fireflies[target].position);

						float distance = Length(&position);

						if (distance > 0.0f && distance < separationDistance)
						{
							Normalize(&position);
							Divide(&position, distance);

							separation = position;
							count++;
						}
					}

					if (count > 0)
					{
						Divide(&separation, (float)count);
						Normalize(&separation);
						Multiply(&separation, maxSpeed);
						Subtract(&separation, &fireflies[boid].velocity);

						float force = Length(&separation);

						if (force > maxForce)
						{
							Divide(&separation, force);
							Multiply(&separation, maxForce);
						}

						Multiply(&separation, 1.5f);
						Add(&fireflies[boid].acceleration, &separation);
					}
				}

				// Cohesion
				for (uint boid = 0; boid < boids; ++boid)
				{
					Vector cohesion = default(Vector);
					int count = 0;

					for (uint target = 0; target < boids; ++target)
					{
						Vector position = fireflies[boid].position;

						Subtract(&position, &fireflies[target].position);

						float distance = Length(&position);

						if (distance > 0.0f && distance < neighbourDistance)
						{
							cohesion = fireflies[boid].position;
							count++;
						}
					}

					if (count > 0)
					{
						Divide(&cohesion, (float)count);
						Subtract(&cohesion, &fireflies[boid].position);
						Normalize(&cohesion);
						Multiply(&cohesion, maxSpeed);
						Subtract(&cohesion, &fireflies[boid].velocity);

						float force = Length(&cohesion);

						if (force > maxForce)
						{
							Divide(&cohesion, force);
							Multiply(&cohesion, maxForce);
						}

						Add(&fireflies[boid].acceleration, &cohesion);
					}
				}
			}

			UnsafeUtility.Free(fireflies, Allocator.Persistent);

			return (float)parkMiller;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Add(Vector* left, Vector* right) {
			left->x += right->x;
			left->y += right->y;
			left->z += right->z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Subtract(Vector* left, Vector* right) {
			left->x -= right->x;
			left->y -= right->y;
			left->z -= right->z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Divide(Vector* vector, float value) {
			vector->x /= value;
			vector->y /= value;
			vector->z /= value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Multiply(Vector* vector, float value) {
			vector->x *= value;
			vector->y *= value;
			vector->z *= value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Normalize(Vector* vector) {
			float length = math.sqrt(vector->x * vector->x + vector->y * vector->y + vector->z * vector->z);

			vector->x /= length;
			vector->y /= length;
			vector->z /= length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private float Length(Vector* vector) {
			return math.sqrt(vector->x * vector->x + vector->y * vector->y + vector->z * vector->z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private float Random()
		{
			parkMiller = (uint)(((ulong)parkMiller * 48271u) % 0x7fffffff);

			return parkMiller / 10000000.0f;
		}
	}

	[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
	internal unsafe struct FirefliesFlockingGCC : IJob
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
