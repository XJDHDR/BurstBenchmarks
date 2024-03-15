using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using NET_project;
// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace
// ReSharper disable ForCanBeConvertedToForeach
// ReSharper disable ConvertSwitchStatementToSwitchExpression

public static class Benchmarks {
	// Options
	
	// Comment out libs that won't be used.
	private static readonly string[] nativeLibNames = {
		"GCC",
		"Clang",
		"MS",
		"Rust",
	};

	private const bool
		netRyuJitEnabled = true,
		nativeLibEnabled = true;

	private const bool
		fibonacciEnabled = false,
		mandelbrotEnabled = false,
		nbodyEnabled = false,
		sieveOfEratosthenesEnabled = false,
		pixarRaytracerEnabled = false,
		firefliesFlockingEnabled = false,
		polynomialsEnabled = false,
		particleKinematicsEnabled = false,
		arcfourEnabled = false,
		seahashEnabled = false,
		radixEnabled = true;

	private const uint
		fibonacciNumber = 46,
		mandelbrotIterations = 8,
		nbodyAdvancements = 100000000,
		sieveOfEratosthenesIterations = 1000000,
		pixarRaytracerSamples = 16,
		firefliesFlockingLifetime = 1000,
		polynomialsIterations = 10000000,
		particleKinematicsIterations = 10000000,
		arcfourIterations = 10000000,
		seahashIterations = 1000000,
		radixIterations = 1000000;

	
	private static unsafe void* Malloc(int size, int alignment, out void* pointer) {
		IntPtr aligned = IntPtr.Zero;

		if (alignment > 8) {
			pointer = (void*)Marshal.AllocHGlobal(size + (alignment - 8));
			aligned = new IntPtr(alignment * (((long)pointer + (alignment - 1)) / alignment));
		} else {
			aligned = Marshal.AllocHGlobal(size);
			pointer = (void*)aligned;
		}

		return (void*)aligned;
	}

	private static unsafe void Free(void* pointer) {
		Marshal.FreeHGlobal((IntPtr)pointer);
	}
	
	private struct Vector {
		public float x, y, z;
	}


	// Fireflies Flocking

	private struct Boid {
		public Vector position, velocity, acceleration;
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

			Boid* fireflies = (Boid*)Malloc((int)(boids * sizeof(Boid)), 16, out void* firefliesPointer);

			for (uint i = 0; i < boids; ++i) {
				fireflies[i].position = new Vector { x = Random(), y = Random(), z = Random() };
				fireflies[i].velocity = new Vector { x = Random(), y = Random(), z = Random() };
				fireflies[i].acceleration = new Vector { x = 0.0f, y = 0.0f, z = 0.0f };
			}

			for (uint i = 0; i < lifetime; ++i) {
				// Update
				for (uint boid = 0; boid < boids; ++boid) {
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
				for (uint boid = 0; boid < boids; ++boid) {
					Vector separation = default(Vector);
					int count = 0;

					for (uint target = 0; target < boids; ++target) {
						Vector position = fireflies[boid].position;

						Subtract(&position, &fireflies[target].position);

						float distance = Length(&position);

						if (distance > 0.0f && distance < separationDistance) {
							Normalize(&position);
							Divide(&position, distance);

							separation = position;
							count++;
						}
					}

					if (count > 0) {
						Divide(&separation, (float)count);
						Normalize(&separation);
						Multiply(&separation, maxSpeed);
						Subtract(&separation, &fireflies[boid].velocity);

						float force = Length(&separation);

						if (force > maxForce) {
							Divide(&separation, force);
							Multiply(&separation, maxForce);
						}

						Multiply(&separation, 1.5f);
						Add(&fireflies[boid].acceleration, &separation);
					}
				}

				// Cohesion
				for (uint boid = 0; boid < boids; ++boid) {
					Vector cohesion = default(Vector);
					int count = 0;

					for (uint target = 0; target < boids; ++target) {
						Vector position = fireflies[boid].position;

						Subtract(&position, &fireflies[target].position);

						float distance = Length(&position);

						if (distance > 0.0f && distance < neighbourDistance) {
							cohesion = fireflies[boid].position;
							count++;
						}
					}

					if (count > 0) {
						Divide(&cohesion, (float)count);
						Subtract(&cohesion, &fireflies[boid].position);
						Normalize(&cohesion);
						Multiply(&cohesion, maxSpeed);
						Subtract(&cohesion, &fireflies[boid].velocity);

						float force = Length(&cohesion);

						if (force > maxForce) {
							Divide(&cohesion, force);
							Multiply(&cohesion, maxForce);
						}

						Add(&fireflies[boid].acceleration, &cohesion);
					}
				}
			}

			Free(firefliesPointer);

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
			float length = (float)Math.Sqrt(vector->x * vector->x + vector->y * vector->y + vector->z * vector->z);

			vector->x /= length;
			vector->y /= length;
			vector->z /= length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private float Length(Vector* vector) {
			return (float)Math.Sqrt(vector->x * vector->x + vector->y * vector->y + vector->z * vector->z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private float Random() {
			parkMiller = (uint)(((ulong)parkMiller * 48271u) % 0x7fffffff);

			return parkMiller / 10000000.0f;
		}
	}

	private unsafe struct FirefliesFlockingGCC : IJob {
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

	// Polynomials

	public unsafe struct PolynomialsNET : IJob {
		public uint iterations;
		public float result;

		public void Run() {
			result = Polynomials(iterations);
		}

		private float Polynomials(uint iterations) {
			const float x = 0.2f;

			float pu = 0.0f;
			float* poly = stackalloc float[100];

			for (uint i = 0; i < iterations; i++) {
				float mu = 10.0f;
				float s;
				int j;

				for (j = 0; j < 100; j++) {
					poly[j] = mu = (mu + 2.0f) / 2.0f;
				}

				s = 0.0f;

				for (j = 0; j < 100; j++) {
					s = x * s + poly[j];
				}

				pu += s;
			}

			return pu;
		}
	}

	private unsafe struct PolynomialsGCC : IJob {
		public uint iterations;
		public float result;
		public string libName;

		public void Run() {
			switch (libName)
			{
				case "GCC":
					result = GccNativeBindings.benchmark_polynomials(iterations);
					break;

				case "Clang":
					result = ClangNativeBindings.benchmark_polynomials(iterations);
					break;

				case "MS":
					result = MsNativeBindings.benchmark_polynomials(iterations);
					break;

				case "Rust":
					result = RustNativeBindings.benchmark_polynomials(iterations);
					break;
			}
		}
	}

	// Particle Kinematics

	private struct Particle {
		public float x, y, z, vx, vy, vz;
	}

	public unsafe struct ParticleKinematicsNET : IJob {
		public uint quantity;
		public uint iterations;
		public float result;

		public void Run() {
			result = ParticleKinematics(quantity, iterations);
		}

		private float ParticleKinematics(uint quantity, uint iterations) {
			Particle* particles = (Particle*)Malloc((int)(quantity * sizeof(Particle)), 16, out void* particlesPointer);

			for (uint i = 0; i < quantity; ++i) {
				particles[i].x = (float)i;
				particles[i].y = (float)(i + 1);
				particles[i].z = (float)(i + 2);
				particles[i].vx = 1.0f;
				particles[i].vy = 2.0f;
				particles[i].vz = 3.0f;
			}

			for (uint a = 0; a < iterations; ++a) {
				for (uint b = 0, c = quantity; b < c; ++b) {
					Particle* p = &particles[b];

					p->x += p->vx;
					p->y += p->vy;
					p->z += p->vz;
				}
			}

			Particle particle = new Particle { x = particles[0].x, y = particles[0].y, z = particles[0].z };

			Free(particlesPointer);

			return particle.x + particle.y + particle.z;
		}
	}

	private unsafe struct ParticleKinematicsGCC : IJob {
		public uint quantity;
		public uint iterations;
		public float result;
		public string libName;

		public void Run() {
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

	// Arcfour

	public unsafe struct ArcfourNET : IJob {
		public uint iterations;
		public int result;

		public void Run() {
			result = Arcfour(iterations);
		}

		private int Arcfour(uint iterations) {
			const int keyLength = 5;
			const int streamLength = 10;

			byte* state = (byte*)Malloc(256, 8, out void* statePointer);
			byte* buffer = (byte*)Malloc(64, 8, out void* bufferPointer);
			byte* key = stackalloc byte[5];
			byte* stream = stackalloc byte[10];

			key[0] = 0xDB;
			key[1] = 0xB7;
			key[2] = 0x60;
			key[3] = 0xD4;
			key[4] = 0x56;

			stream[0] = 0xEB;
			stream[1] = 0x9F;
			stream[2] = 0x77;
			stream[3] = 0x81;
			stream[4] = 0xB7;
			stream[5] = 0x34;
			stream[6] = 0xCA;
			stream[7] = 0x72;
			stream[8] = 0xA7;
			stream[9] = 0x19;

			int idx = 0;

			for (uint i = 0; i < iterations; i++) {
				idx = KeySetup(state, key, keyLength);
				idx = GenerateStream(state, buffer, streamLength);
			}

			Free(statePointer);
			Free(bufferPointer);

			return idx;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int KeySetup(byte* state, byte* key, int length) {
			int i, j;
			byte t;

			for (i = 0; i < 256; ++i) {
				state[i] = (byte)i;
			}

			for (i = 0, j = 0; i < 256; ++i) {
				j = (j + state[i] + key[i % length]) % 256;
				t = state[i];
				state[i] = state[j];
				state[j] = t;
			}

			return i;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int GenerateStream(byte* state, byte* buffer, int length) {
			int i, j;
			int idx;
			byte t;

			for (idx = 0, i = 0, j = 0; idx < length; ++idx) {
				i = (i + 1) % 256;
				j = (j + state[i]) % 256;
				t = state[i];
				state[i] = state[j];
				state[j] = t;
				buffer[idx] = state[(state[i] + state[j]) % 256];
			}

			return i;
		}
	}

	private unsafe struct ArcfourGCC : IJob {
		public uint iterations;
		public int result;
		public string libName;

		public void Run() {
			switch (libName)
			{
				case "GCC":
					result = GccNativeBindings.benchmark_arcfour(iterations);
					break;

				case "Clang":
					result = ClangNativeBindings.benchmark_arcfour(iterations);
					break;

				case "MS":
					result = MsNativeBindings.benchmark_arcfour(iterations);
					break;

				case "Rust":
					result = RustNativeBindings.benchmark_arcfour(iterations);
					break;
			}
		}
	}

	// Seahash

	public unsafe struct SeahashNET : IJob {
		public uint iterations;
		public ulong result;

		public void Run() {
			result = Seahash(iterations);
		}

		private ulong Seahash(uint Iterations) {
			const int bufferLength = 1024 * 128;

			byte* buffer = (byte*)Malloc(bufferLength, 8, out void* bufferPointer);

			for (int i = 0; i < bufferLength; i++) {
				buffer[i] = (byte)(i % 256);
			}

			ulong hash = 0;

			for (uint i = 0; i < Iterations; i++) {
				hash = Compute(buffer, bufferLength, 0x16F11FE89B0D677C, 0xB480A793D8E6C86C, 0x6FE2E5AAF078EBC9, 0x14F994A4C5259381);
			}

			Free(bufferPointer);

			return hash;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private ulong Read(byte* pointer) {
			return *(ulong*)pointer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private ulong Diffuse(ulong value) {
			value *= 0x6EED0E9DA4D94A4F;
			value ^= ((value >> 32) >> (int)(value >> 60));
			value *= 0x6EED0E9DA4D94A4F;

			return value;
		}

		private ulong Compute(byte* buffer, ulong length, ulong a, ulong b, ulong c, ulong d) {
			const uint blockSize = 32;

			ulong end = length & ~(blockSize - 1);

			for (uint i = 0; i < end; i += blockSize) {
				a ^= Read(buffer + i);
				b ^= Read(buffer + i + 8);
				c ^= Read(buffer + i + 16);
				d ^= Read(buffer + i + 24);

				a = Diffuse(a);
				b = Diffuse(b);
				c = Diffuse(c);
				d = Diffuse(d);
			}

			ulong excessive = length - end;
			byte* bufferEnd = buffer + end;

			if (excessive > 0) {
				a ^= Read(bufferEnd);

				if (excessive > 8) {
					b ^= Read(bufferEnd);

					if (excessive > 16) {
						c ^= Read(bufferEnd);

						if (excessive > 24) {
							d ^= Read(bufferEnd);
							d = Diffuse(d);
						}

						c = Diffuse(c);
					}

					b = Diffuse(b);
				}

				a = Diffuse(a);
			}

			a ^= b;
			c ^= d;
			a ^= c;
			a ^= length;

			return Diffuse(a);
		}
	}

	private unsafe struct SeahashGCC : IJob {
		public uint iterations;
		public ulong result;
		public string libName;

		public void Run() {
			switch (libName)
			{
				case "GCC":
					result = GccNativeBindings.benchmark_seahash(iterations);
					break;

				case "Clang":
					result = ClangNativeBindings.benchmark_seahash(iterations);
					break;

				case "MS":
					result = MsNativeBindings.benchmark_seahash(iterations);
					break;

				case "Rust":
					result = RustNativeBindings.benchmark_seahash(iterations);
					break;
			}
		}
	}

	// Radix

	public unsafe struct RadixNET : IJob {
		public uint iterations;
		public int result;

		public void Run() {
			result = Radix(iterations);
		}

		private uint classicRandom;

		private int Radix(uint iterations) {
			classicRandom = 7525;

			const int arrayLength = 128;

			int* array = (int*)Malloc(arrayLength * sizeof(int), 16, out void* arrayPointer);

			for (uint a = 0; a < iterations; a++) {
				for (int b = 0; b < arrayLength; b++) {
					array[b] = Random();
				}

				Sort(array, arrayLength, (a == 0));
			}

			int head = array[0];

			Free(arrayPointer);

			return head;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int Random() {
			classicRandom = (6253729 * classicRandom + 4396403);

			return (int)(classicRandom % 32767);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int FindLargest(int* array, int length) {
			int i;
			int largest = -1;

			for (i = 0; i < length; i++) {
				if (array[i] > largest)
					largest = array[i];
			}

			return largest;
		}

		private void Sort(int* array, int length, bool printDone) {
			int i;
			Span<int> semiSorted = stackalloc int[length];
			int significantDigit = 1;
			int largest = FindLargest(array, length);

			Span<int> bucket = stackalloc int[10];
			int loops = 0;
			while (largest / significantDigit > 0) {
				loops++;
				for (i = 0; i < length; i++) {
					bucket[(array[i] / significantDigit) % 10]++;
				}

				for (i = 1; i < 10; i++) {
					bucket[i] += bucket[i - 1];
				}

				StringBuilder values = new();
				for (i = length - 1; i >= 0; i--)
				{
					int bucketIndex = (array[i] / significantDigit) % 10;
					int semiSortedIndex = (--bucket[bucketIndex]) % length;
					semiSorted[semiSortedIndex] = array[i];
					
					if (printDone && loops <= 2)
					{
						values.AppendLine($"Loop {loops}, Index {i} = {semiSorted[semiSortedIndex]}");
					}
				}

				/*
				if (printDone && loops <= 2)
				{
					File.AppendAllText("./benchmark_output_cs.txt", values.ToString());
				}*/

				for (i = 0; i < length; i++) {
					array[i] = semiSorted[i];
					
					if (printDone && loops <= 1)
					{
						values.AppendLine($"Loop {loops}, Index {i} = {array[i]}");
					}
				}

				if (printDone && loops <= 1)
				{
					File.AppendAllText("./benchmark_output_cs.txt", values.ToString());
				}

				significantDigit *= 10;
			}
		}
	}

	private unsafe struct RadixGCC : IJob {
		public uint iterations;
		public int result;
		public string libName;

		public void Run() {
			switch (libName)
			{
				case "GCC":
					result = GccNativeBindings.benchmark_radix(iterations);
					break;

				case "Clang":
					result = ClangNativeBindings.benchmark_radix(iterations);
					break;

				case "MS":
					result = MsNativeBindings.benchmark_radix(iterations);
					break;

				case "Rust":
					result = RustNativeBindings.benchmark_radix(iterations);
					break;
			}
		}
	}

	[STAThread]
	private static void Main() {
		Process thisProcess = Process.GetCurrentProcess();
		thisProcess.PriorityClass = ProcessPriorityClass.High;
		
		var stopwatch = new Stopwatch();
		long time;
		using (StreamWriter benchmarkResultsStreamWriter = new($"{Directory.GetCurrentDirectory()}/Results.txt", true))
		{
			benchmarkResultsStreamWriter.WriteLine("\r\nBenchmark Results:");

			// Benchmarks

			if (netRyuJitEnabled && fibonacciEnabled) {
				var benchmark = new FibonacciNET {
					number = fibonacciNumber
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();
				stopwatch.Stop();

				time = stopwatch.ElapsedTicks;

				Console.WriteLine($"(RyuJIT) Fibonacci: {time} ticks. Result: {benchmark.result}");
				benchmarkResultsStreamWriter.WriteLine($"(RyuJIT) Fibonacci: {time} ticks. Result: {benchmark.result}");
			}

			if (nativeLibEnabled && fibonacciEnabled) {
				for (int i = 0; i < nativeLibNames.Length; i++)
				{
					var benchmark = new FibonacciGCC {
						number = fibonacciNumber,
						libName = nativeLibNames[i]
					};

					stopwatch.Stop();
					benchmark.Run();

					stopwatch.Restart();
					benchmark.Run();
					stopwatch.Stop();

					time = stopwatch.ElapsedTicks;

					Console.WriteLine($"({nativeLibNames[i]}) Fibonacci: {time} ticks. Result: {benchmark.result}");
					benchmarkResultsStreamWriter.WriteLine($"({nativeLibNames[i]}) Fibonacci: {time} ticks. Result: {benchmark.result}");
				}
			}

			if (netRyuJitEnabled && mandelbrotEnabled) {
				var benchmark = new MandelbrotNET {
					width = 1920,
					height = 1080,
					iterations = mandelbrotIterations
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();
				stopwatch.Stop();

				time = stopwatch.ElapsedTicks;

				Console.WriteLine($"(RyuJIT) Mandelbrot: {time} ticks. Result: {benchmark.result}");
				benchmarkResultsStreamWriter.WriteLine($"(RyuJIT) Mandelbrot: {time} ticks. Result: {benchmark.result}");
			}

			if (nativeLibEnabled && mandelbrotEnabled) {
				for (int i = 0; i < nativeLibNames.Length; i++)
				{
					var benchmark = new MandelbrotGCC {
						width = 1920,
						height = 1080,
						iterations = mandelbrotIterations,
						libName = nativeLibNames[i]
					};

					stopwatch.Stop();
					benchmark.Run();

					stopwatch.Restart();
					benchmark.Run();
					stopwatch.Stop();

					time = stopwatch.ElapsedTicks;

					Console.WriteLine($"({nativeLibNames[i]}) Mandelbrot: {time} ticks. Result: {benchmark.result}");
					benchmarkResultsStreamWriter.WriteLine($"({nativeLibNames[i]}) Mandelbrot: {time} ticks. Result: {benchmark.result}");
				}
			}

			if (netRyuJitEnabled && nbodyEnabled) {
				var benchmark = new NBodyNET {
					advancements = nbodyAdvancements
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();
				stopwatch.Stop();

				time = stopwatch.ElapsedTicks;

				Console.WriteLine($"(RyuJIT) NBody: {time} ticks. Result: {benchmark.result}");
				benchmarkResultsStreamWriter.WriteLine($"(RyuJIT) NBody: {time} ticks. Result: {benchmark.result}");
			}

			if (nativeLibEnabled && nbodyEnabled) {
				for (int i = 0; i < nativeLibNames.Length; i++)
				{
					var benchmark = new NBodyGCC {
						advancements = nbodyAdvancements,
						libName = nativeLibNames[i]
					};

					stopwatch.Stop();
					benchmark.Run();

					stopwatch.Restart();
					benchmark.Run();
					stopwatch.Stop();

					time = stopwatch.ElapsedTicks;

					Console.WriteLine($"({nativeLibNames[i]}) NBody: {time} ticks. Result: {benchmark.result}");
					benchmarkResultsStreamWriter.WriteLine($"({nativeLibNames[i]}) NBody: {time} ticks. Result: {benchmark.result}");
				}
			}

			if (netRyuJitEnabled && sieveOfEratosthenesEnabled) {
				var benchmark = new SieveOfEratosthenesNET {
					iterations = sieveOfEratosthenesIterations
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();
				stopwatch.Stop();

				time = stopwatch.ElapsedTicks;

				Console.WriteLine($"(RyuJIT) Sieve of Eratosthenes: {time} ticks. Result: {benchmark.result}");
				benchmarkResultsStreamWriter.WriteLine($"(RyuJIT) Sieve of Eratosthenes: {time} ticks. Result: {benchmark.result}");
			}

			if (nativeLibEnabled && sieveOfEratosthenesEnabled) {
				for (int i = 0; i < nativeLibNames.Length; i++)
				{
					var benchmark = new SieveOfEratosthenesGCC {
						iterations = sieveOfEratosthenesIterations,
						libName = nativeLibNames[i]
					};

					stopwatch.Stop();
					benchmark.Run();

					stopwatch.Restart();
					benchmark.Run();
					stopwatch.Stop();

					time = stopwatch.ElapsedTicks;

					Console.WriteLine($"({nativeLibNames[i]}) Sieve of Eratosthenes: {time} ticks. Result: {benchmark.result}");
					benchmarkResultsStreamWriter.WriteLine($"({nativeLibNames[i]}) Sieve of Eratosthenes: {time} ticks. Result: {benchmark.result}");
				}
			}

			if (netRyuJitEnabled && pixarRaytracerEnabled) {
				var benchmark = new PixarRaytracerNET {
					width = 720,
					height = 480,
					samples = pixarRaytracerSamples
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();
				stopwatch.Stop();

				time = stopwatch.ElapsedTicks;

				Console.WriteLine($"(RyuJIT) Pixar Raytracer: {time} ticks. Result: {benchmark.result}");
				benchmarkResultsStreamWriter.WriteLine($"(RyuJIT) Pixar Raytracer: {time} ticks. Result: {benchmark.result}");
			}

			if (nativeLibEnabled && pixarRaytracerEnabled) {
				for (int i = 0; i < nativeLibNames.Length; i++)
				{
					var benchmark = new PixarRaytracerGCC {
						width = 720,
						height = 480,
						samples = pixarRaytracerSamples,
						libName = nativeLibNames[i]
					};

					stopwatch.Stop();
					benchmark.Run();

					stopwatch.Restart();
					benchmark.Run();
					stopwatch.Stop();

					time = stopwatch.ElapsedTicks;

					Console.WriteLine($"({nativeLibNames[i]}) Pixar Raytracer: {time} ticks. Result: {benchmark.result}");
					benchmarkResultsStreamWriter.WriteLine($"({nativeLibNames[i]}) Pixar Raytracer: {time} ticks. Result: {benchmark.result}");
				}
			}

			if (netRyuJitEnabled && firefliesFlockingEnabled) {
				var benchmark = new FirefliesFlockingNET {
					boids = 1000,
					lifetime = firefliesFlockingLifetime
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();
				stopwatch.Stop();

				time = stopwatch.ElapsedTicks;

				Console.WriteLine($"(RyuJIT) Fireflies Flocking: {time} ticks. Result: {benchmark.result}");
				benchmarkResultsStreamWriter.WriteLine($"(RyuJIT) Fireflies Flocking: {time} ticks. Result: {benchmark.result}");
			}

			if (nativeLibEnabled && firefliesFlockingEnabled) {
				for (int i = 0; i < nativeLibNames.Length; i++)
				{
					var benchmark = new FirefliesFlockingGCC {
						boids = 1000,
						lifetime = firefliesFlockingLifetime,
						libName = nativeLibNames[i]
					};

					stopwatch.Stop();
					benchmark.Run();

					stopwatch.Restart();
					benchmark.Run();
					stopwatch.Stop();

					time = stopwatch.ElapsedTicks;

					Console.WriteLine($"({nativeLibNames[i]}) Fireflies Flocking: {time} ticks. Result: {benchmark.result}");
					benchmarkResultsStreamWriter.WriteLine($"({nativeLibNames[i]}) Fireflies Flocking: {time} ticks. Result: {benchmark.result}");
				}
			}

			if (netRyuJitEnabled && polynomialsEnabled) {
				var benchmark = new PolynomialsNET {
					iterations = polynomialsIterations
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();
				stopwatch.Stop();

				time = stopwatch.ElapsedTicks;

				Console.WriteLine($"(RyuJIT) Polynomials: {time} ticks. Result: {benchmark.result}");
				benchmarkResultsStreamWriter.WriteLine($"(RyuJIT) Polynomials: {time} ticks. Result: {benchmark.result}");
			}

			if (nativeLibEnabled && polynomialsEnabled) {
				for (int i = 0; i < nativeLibNames.Length; i++)
				{
					var benchmark = new PolynomialsGCC {
						iterations = polynomialsIterations,
						libName = nativeLibNames[i]
					};

					stopwatch.Stop();
					benchmark.Run();

					stopwatch.Restart();
					benchmark.Run();
					stopwatch.Stop();

					time = stopwatch.ElapsedTicks;

					Console.WriteLine($"({nativeLibNames[i]}) Polynomials: {time} ticks. Result: {benchmark.result}");
					benchmarkResultsStreamWriter.WriteLine($"({nativeLibNames[i]}) Polynomials: {time} ticks. Result: {benchmark.result}");
				}
			}

			if (netRyuJitEnabled && particleKinematicsEnabled) {
				var benchmark = new ParticleKinematicsNET {
					quantity = 1000,
					iterations = particleKinematicsIterations
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();
				stopwatch.Stop();

				time = stopwatch.ElapsedTicks;

				Console.WriteLine($"(RyuJIT) Particle Kinematics: {time} ticks. Result: {benchmark.result}");
				benchmarkResultsStreamWriter.WriteLine($"(RyuJIT) Particle Kinematics: {time} ticks. Result: {benchmark.result}");
			}

			if (nativeLibEnabled && particleKinematicsEnabled) {
				for (int i = 0; i < nativeLibNames.Length; i++)
				{
					var benchmark = new ParticleKinematicsGCC {
						quantity = 1000,
						iterations = particleKinematicsIterations,
						libName = nativeLibNames[i]
					};

					stopwatch.Stop();
					benchmark.Run();

					stopwatch.Restart();
					benchmark.Run();
					stopwatch.Stop();

					time = stopwatch.ElapsedTicks;

					Console.WriteLine($"({nativeLibNames[i]}) Particle Kinematics: {time} ticks. Result: {benchmark.result}");
					benchmarkResultsStreamWriter.WriteLine($"({nativeLibNames[i]}) Particle Kinematics: {time} ticks. Result: {benchmark.result}");
				}
			}

			if (netRyuJitEnabled && arcfourEnabled) {
				var benchmark = new ArcfourNET {
					iterations = arcfourIterations
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();
				stopwatch.Stop();

				time = stopwatch.ElapsedTicks;

				Console.WriteLine($"(RyuJIT) Arcfour: {time} ticks. Result: {benchmark.result}");
				benchmarkResultsStreamWriter.WriteLine($"(RyuJIT) Arcfour: {time} ticks. Result: {benchmark.result}");
			}

			if (nativeLibEnabled && arcfourEnabled) {
				for (int i = 0; i < nativeLibNames.Length; i++)
				{
					var benchmark = new ArcfourGCC {
						iterations = arcfourIterations,
						libName = nativeLibNames[i]
					};

					stopwatch.Stop();
					benchmark.Run();

					stopwatch.Restart();
					benchmark.Run();
					stopwatch.Stop();

					time = stopwatch.ElapsedTicks;

					Console.WriteLine($"({nativeLibNames[i]}) Arcfour: {time} ticks. Result: {benchmark.result}");
					benchmarkResultsStreamWriter.WriteLine($"({nativeLibNames[i]}) Arcfour: {time} ticks. Result: {benchmark.result}");
				}
			}

			if (netRyuJitEnabled && seahashEnabled) {
				var benchmark = new SeahashNET {
					iterations = seahashIterations
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();
				stopwatch.Stop();

				time = stopwatch.ElapsedTicks;

				Console.WriteLine($"(RyuJIT) Seahash: {time} ticks. Result: {benchmark.result}");
				benchmarkResultsStreamWriter.WriteLine($"(RyuJIT) Seahash: {time} ticks. Result: {benchmark.result}");
			}

			if (nativeLibEnabled && seahashEnabled) {
				for (int i = 0; i < nativeLibNames.Length; i++)
				{
					var benchmark = new SeahashGCC {
						iterations = seahashIterations,
						libName = nativeLibNames[i]
					};

					stopwatch.Stop();
					benchmark.Run();

					stopwatch.Restart();
					benchmark.Run();
					stopwatch.Stop();

					time = stopwatch.ElapsedTicks;

					Console.WriteLine($"({nativeLibNames[i]}) Seahash: {time} ticks. Result: {benchmark.result}");
					benchmarkResultsStreamWriter.WriteLine($"({nativeLibNames[i]}) Seahash: {time} ticks. Result: {benchmark.result}");
				}
			}

			if (netRyuJitEnabled && radixEnabled) {
				var benchmark = new RadixNET {
					iterations = radixIterations
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();
				stopwatch.Stop();

				time = stopwatch.ElapsedTicks;

				Console.WriteLine($"(RyuJIT) Radix: {time} ticks. Result: {benchmark.result}");
				benchmarkResultsStreamWriter.WriteLine($"(RyuJIT) Radix: {time} ticks. Result: {benchmark.result}");
			}

			if (nativeLibEnabled && radixEnabled) {
				for (int i = 0; i < nativeLibNames.Length; i++)
				{
					var benchmark = new RadixGCC {
						iterations = radixIterations,
						libName = nativeLibNames[i]
					};

					stopwatch.Stop();
					benchmark.Run();

					stopwatch.Restart();
					benchmark.Run();
					stopwatch.Stop();

					time = stopwatch.ElapsedTicks;

					Console.WriteLine($"({nativeLibNames[i]}) Radix: {time} ticks. Result: {benchmark.result}");
					benchmarkResultsStreamWriter.WriteLine($"({nativeLibNames[i]}) Radix: {time} ticks. Result: {benchmark.result}");
				}
			}

			benchmarkResultsStreamWriter.Close();
		}
		Environment.Exit(0);
	}
}
