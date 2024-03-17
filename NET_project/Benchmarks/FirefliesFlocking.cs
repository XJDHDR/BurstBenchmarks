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

	private static uint parkMiller;
	private static float maxSpeed;
	private static float maxForce;
	private static float separationDistance;
	private static float neighbourDistance;

	private static float FirefliesFlocking(uint boids, uint lifetime) {
		parkMiller = 666;
		maxSpeed = 1.0f;
		maxForce = 0.03f;
		separationDistance = 15.0f;
		neighbourDistance = 30.0f;

		Boid[] fireflies = new Boid[boids];

		for (uint i = 0; i < fireflies.Length; i++) {
			ref Boid firefly = ref fireflies[i];

			firefly.position = new Vector3(Random(), Random(), Random());
			firefly.velocity = new Vector3(Random(), Random(), Random());
			firefly.acceleration = Vector3.Zero;
		}

		for (uint i = 0; i < lifetime; i++)
		{
			// Update
			for (uint boid = 0; boid < fireflies.Length; boid++)
			{
				ref Boid firefly = ref fireflies[boid];
				firefly.velocity += firefly.acceleration;

				float speed = firefly.velocity.Length();

				if (speed > maxSpeed)
				{
					firefly.velocity /= speed;
					firefly.velocity *= maxSpeed;
				}

				firefly.position += firefly.velocity;
				firefly.acceleration *= maxSpeed;
			}

			// Separation
			for (uint boid = 0; boid < fireflies.Length; boid++)
			{
				ref Boid firefly = ref fireflies[boid];

				Vector3 separation = Vector3.Zero;
				int count = 0;

				for (uint target = 0; target < fireflies.Length; target++) 
				{
					Vector3 position = firefly.position;

					position -= fireflies[target].position;

					float distance = position.Length();

					if (distance > 0.0f && distance < separationDistance)
					{
						position = Vector3.Normalize(position);
						position /= distance;

						separation = position;
						count++;
					}
				}

				if (count > 0)
				{
					separation /= count;
					separation = Vector3.Normalize(separation);
					separation *= maxSpeed;
					separation -= firefly.velocity;

					float force = separation.Length();

					if (force > maxForce)
					{
						separation /= force;
						separation *= maxForce;
					}

					separation *= 1.5f;
					firefly.acceleration += separation;
				}
			}

			// Cohesion
			for (uint boid = 0; boid < fireflies.Length; boid++)
			{
				ref Boid firefly = ref fireflies[boid];

				Vector3 cohesion = Vector3.Zero;
				int count = 0;

				for (uint target = 0; target < fireflies.Length; target++)
				{
					Vector3 position = firefly.position;

					position -= fireflies[target].position;

					float distance = position.Length();

					if (distance > 0.0f && distance < neighbourDistance)
					{
						cohesion = firefly.position;
						count++;
					}
				}

				if (count > 0)
				{
					cohesion /= count;
					cohesion -= firefly.position;
					cohesion = Vector3.Normalize(cohesion);
					cohesion *= maxSpeed;
					cohesion -= firefly.velocity;

					float force = cohesion.Length();

					if (force > maxForce)
					{
						cohesion /= force;
						cohesion *= maxForce;
					}

					firefly.acceleration += cohesion;
				}
			}
		}

		return parkMiller;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static float Random() {
		parkMiller = (uint)((ulong)parkMiller * 48271u % 0x7fffffff);

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
