using System.Numerics;
using System.Runtime.CompilerServices;

namespace NET_project.Benchmarks;

internal struct Boid {
	public Vector3 position, velocity, acceleration;
}

public unsafe struct FirefliesFlockingNET : IJob {
	public uint boids;
	public uint lifetime;
	public float result;

	public void Run() {
		result = FirefliesFlocking(boids, lifetime);
	}

	private uint parkMiller;
	private float maxSpeed;
	private float maxForce;
	private float separationDistance;
	private float neighbourDistance;

	private float FirefliesFlocking(uint boids, uint lifetime) {
		parkMiller = 666;
		maxSpeed = 1.0f;
		maxForce = 0.03f;
		separationDistance = 15.0f;
		neighbourDistance = 30.0f;

		Boid[] fireflies = new Boid[boids];

		for (uint i = 0; i < boids; ++i) {
			fireflies[i].position = new Vector3 { X = Random(), Y = Random(), Z = Random() };
			fireflies[i].velocity = new Vector3 { X = Random(), Y = Random(), Z = Random() };
			fireflies[i].acceleration = new Vector3 { X = 0.0f, Y = 0.0f, Z = 0.0f };
		}

		for (uint i = 0; i < lifetime; ++i)
		{
			// Update
			for (uint boid = 0; boid < boids; ++boid)
			{
				fireflies[boid].velocity = Vector3.Add(fireflies[boid].velocity, fireflies[boid].acceleration);

				float speed = fireflies[boid].velocity.Length();

				if (speed > maxSpeed)
				{
					fireflies[boid].velocity = Vector3.Divide(fireflies[boid].velocity, speed);
					fireflies[boid].velocity = Vector3.Multiply(fireflies[boid].velocity, maxSpeed);
				}

				fireflies[boid].position = Vector3.Add(fireflies[boid].position, fireflies[boid].velocity);
				fireflies[boid].acceleration = Vector3.Multiply(fireflies[boid].acceleration, maxSpeed);
			}

			// Separation
			for (uint boid = 0; boid < boids; ++boid)
			{
				Vector3 separation = default(Vector3);
				int count = 0;

				for (uint target = 0; target < boids; ++target) 
				{
					Vector3 position = fireflies[boid].position;

					position = Vector3.Subtract(position, fireflies[target].position);

					float distance = position.Length();

					if (distance > 0.0f && distance < separationDistance)
					{
						position = Vector3.Normalize(position);
						position = Vector3.Divide(position, distance);

						separation = position;
						count++;
					}
				}

				if (count > 0)
				{
					separation = Vector3.Divide(separation, count);
					separation = Vector3.Normalize(separation);
					separation = Vector3.Multiply(separation, maxSpeed);
					separation = Vector3.Subtract(separation, fireflies[boid].velocity);

					float force = separation.Length();

					if (force > maxForce)
					{
						separation = Vector3.Divide(separation, force);
						separation = Vector3.Multiply(separation, maxForce);
					}

					separation = Vector3.Multiply(separation, 1.5f);
					fireflies[boid].acceleration = Vector3.Add(fireflies[boid].acceleration, separation);
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

					position = Vector3.Subtract(position, fireflies[target].position);

					float distance = position.Length();

					if (distance > 0.0f && distance < neighbourDistance)
					{
						cohesion = fireflies[boid].position;
						count++;
					}
				}

				if (count > 0)
				{
					cohesion = Vector3.Divide(cohesion, count);
					cohesion = Vector3.Subtract(cohesion, fireflies[boid].position);
					cohesion = Vector3.Normalize(cohesion);
					cohesion = Vector3.Multiply(cohesion, maxSpeed);
					cohesion = Vector3.Subtract(cohesion, fireflies[boid].velocity);

					float force = cohesion.Length();

					if (force > maxForce)
					{
						cohesion = Vector3.Divide(cohesion, force);
						cohesion = Vector3.Multiply(cohesion, maxForce);
					}

					fireflies[boid].acceleration = Vector3.Add(fireflies[boid].acceleration, cohesion);
				}
			}
		}

		return (float)parkMiller;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private float Random() {
		parkMiller = (uint)(((ulong)parkMiller * 48271u) % 0x7fffffff);

		return parkMiller / 10000000.0f;
	}
}

internal unsafe struct FirefliesFlockingGCC : IJob {
	public uint boids;
	public uint lifetime;
	public float result;
	public string libName;

	public void Run() {
		switch (libName)
		{
			case "GCC":
				result = GccNativeBindings.benchmark_fireflies_flocking(boids, lifetime);
				break;

			case "Clang":
				result = ClangNativeBindings.benchmark_fireflies_flocking(boids, lifetime);
				break;

			case "MS":
				result = MsNativeBindings.benchmark_fireflies_flocking(boids, lifetime);
				break;

			case "Rust":
				result = RustNativeBindings.benchmark_fireflies_flocking(boids, lifetime);
				break;
		}
	}
}
