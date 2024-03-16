using System.Globalization;
using System.IO;
using BenchmarkCode;
using BenchmarkCode.Double;
using BenchmarkCode.Integer;
using BenchmarkCode.Single;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

// ReSharper disable once PartialTypeWithSinglePart
public partial class Benchmarks : SystemBase {
	// Options
	
	private readonly string scriptingBackend;

	public Benchmarks()
	{
	#if ENABLE_MONO
		scriptingBackend = "Mono JIT";
	#elif ENABLE_IL2CPP
		scriptingBackend = "IL2CPP";
	#else
		scriptingBackend = "Unknown backend";
	#endif
	}

	private const bool
		burstEnabled = true,
		nativeLibraryEnabled = false,
		scriptBackendEnabled = true;

	private const bool
		fibonacciEnabled = true,
		mandelbrotEnabled = true,
		nbodyEnabled = true,
		sieveOfEratosthenesEnabled = true,
		pixarRaytracerEnabled = true,
		firefliesFlockingEnabled = true,
		polynomialsEnabled = true,
		particleKinematicsEnabled = true,
		arcfourEnabled = true,
		seahashEnabled = true,
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


	protected override void OnCreate() {
		var stopwatch = new System.Diagnostics.Stopwatch();
		long time = 0;
		using (StreamWriter benchmarkResultStreamWriter = new($"{Application.persistentDataPath}/Benchmark_results_{NativeBindings.nativeLibraryPrefixString}.txt", true))
		{
			// Benchmarks

			if (burstEnabled && fibonacciEnabled) {
				var benchmark = new FibonacciBurst {
					number = fibonacciNumber
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"(Burst) Fibonacci: {time} ticks. Result: {benchmark.result.ToString()}");
			}

			if (nativeLibraryEnabled && fibonacciEnabled) {
				var benchmark = new FibonacciGCC {
					number = fibonacciNumber
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"({NativeBindings.nativeLibraryPrefixString}) Fibonacci: {time} ticks. Result: {benchmark.result.ToString()}");
			}

			if (scriptBackendEnabled && fibonacciEnabled) {
				var benchmark = new FibonacciBurst {
					number = fibonacciNumber
				};

				stopwatch.Stop();
				benchmark.Execute();

				stopwatch.Restart();
				benchmark.Execute();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"({scriptingBackend}) Fibonacci: {time} ticks. Result: {benchmark.result.ToString()}");
			}

			if (burstEnabled && mandelbrotEnabled) {
				var benchmark = new MandelbrotBurst {
					width = 1920,
					height = 1080,
					iterations = mandelbrotIterations
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"(Burst) Mandelbrot: {time} ticks. Result: {benchmark.result.ToString(CultureInfo.InvariantCulture)}");
			}

			if (nativeLibraryEnabled && mandelbrotEnabled) {
				var benchmark = new MandelbrotGCC {
					width = 1920,
					height = 1080,
					iterations = mandelbrotIterations
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"({NativeBindings.nativeLibraryPrefixString}) Mandelbrot: {time} ticks. Result: {benchmark.result.ToString(CultureInfo.InvariantCulture)}");
			}

			if (scriptBackendEnabled && mandelbrotEnabled) {
				var benchmark = new MandelbrotBurst {
					width = 1920,
					height = 1080,
					iterations = mandelbrotIterations
				};

				stopwatch.Stop();
				benchmark.Execute();

				stopwatch.Restart();
				benchmark.Execute();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"({scriptingBackend}) Mandelbrot: {time} ticks. Result: {benchmark.result.ToString(CultureInfo.InvariantCulture)}");
			}

			if (burstEnabled && nbodyEnabled) {
				var benchmark = new NBodyBurst {
					advancements = nbodyAdvancements
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"(Burst) NBody: {time} ticks. Result: {benchmark.result.ToString(CultureInfo.InvariantCulture)}");
			}

			if (nativeLibraryEnabled && nbodyEnabled) {
				var benchmark = new NBodyGCC {
					advancements = nbodyAdvancements
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"({NativeBindings.nativeLibraryPrefixString}) NBody: {time} ticks. Result: {benchmark.result.ToString(CultureInfo.InvariantCulture)}");
			}

			if (scriptBackendEnabled && nbodyEnabled) {
				var benchmark = new NBodyBurst {
					advancements = nbodyAdvancements
				};

				stopwatch.Stop();
				benchmark.Execute();

				stopwatch.Restart();
				benchmark.Execute();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"({scriptingBackend}) NBody: {time} ticks. Result: {benchmark.result.ToString(CultureInfo.InvariantCulture)}");
			}

			if (burstEnabled && sieveOfEratosthenesEnabled) {
				var benchmark = new SieveOfEratosthenesBurst {
					iterations = sieveOfEratosthenesIterations
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"(Burst) Sieve of Eratosthenes: {time} ticks. Result: {benchmark.result.ToString()}");
			}

			if (nativeLibraryEnabled && sieveOfEratosthenesEnabled) {
				var benchmark = new SieveOfEratosthenesGCC {
					iterations = sieveOfEratosthenesIterations
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"({NativeBindings.nativeLibraryPrefixString}) Sieve of Eratosthenes: {time} ticks. Result: {benchmark.result.ToString(CultureInfo.InvariantCulture)}");
			}

			if (scriptBackendEnabled && sieveOfEratosthenesEnabled) {
				var benchmark = new SieveOfEratosthenesBurst {
					iterations = sieveOfEratosthenesIterations
				};

				stopwatch.Stop();
				benchmark.Execute();

				stopwatch.Restart();
				benchmark.Execute();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"({scriptingBackend}) Sieve of Eratosthenes: {time} ticks. Result: {benchmark.result.ToString(CultureInfo.InvariantCulture)}");
			}

			if (burstEnabled && pixarRaytracerEnabled) {
				var benchmark = new PixarRaytracerBurst {
					width = 720,
					height = 480,
					samples = pixarRaytracerSamples
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"(Burst) Pixar Raytracer: {time} ticks. Result: {benchmark.result.ToString(CultureInfo.InvariantCulture)}");
			}

			if (nativeLibraryEnabled && pixarRaytracerEnabled) {
				var benchmark = new PixarRaytracerGCC {
					width = 720,
					height = 480,
					samples = pixarRaytracerSamples
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"({NativeBindings.nativeLibraryPrefixString}) Pixar Raytracer: {time} ticks. Result: {benchmark.result.ToString(CultureInfo.InvariantCulture)}");
			}

			if (scriptBackendEnabled && pixarRaytracerEnabled) {
				var benchmark = new PixarRaytracerUnity {
					width = 720,
					height = 480,
					samples = pixarRaytracerSamples
				};

				stopwatch.Stop();
				benchmark.Execute();

				stopwatch.Restart();
				benchmark.Execute();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"({scriptingBackend}) Pixar Raytracer: {time} ticks. Result: {benchmark.result.ToString(CultureInfo.InvariantCulture)}");
			}

			if (burstEnabled && firefliesFlockingEnabled) {
				var benchmark = new FirefliesFlockingBurst {
					boids = 1000,
					lifetime = firefliesFlockingLifetime
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"(Burst) Fireflies Flocking: {time} ticks. Result: {benchmark.result.ToString(CultureInfo.InvariantCulture)}");
			}

			if (nativeLibraryEnabled && firefliesFlockingEnabled) {
				var benchmark = new FirefliesFlockingGCC {
					boids = 1000,
					lifetime = firefliesFlockingLifetime
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"({NativeBindings.nativeLibraryPrefixString}) Fireflies Flocking: {time} ticks. Result: {benchmark.result.ToString(CultureInfo.InvariantCulture)}");
			}

			if (scriptBackendEnabled && firefliesFlockingEnabled) {
				var benchmark = new FirefliesFlockingUnity {
					boids = 1000,
					lifetime = firefliesFlockingLifetime
				};

				stopwatch.Stop();
				benchmark.Execute();

				stopwatch.Restart();
				benchmark.Execute();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"({scriptingBackend}) Fireflies Flocking: {time} ticks. Result: {benchmark.result.ToString(CultureInfo.InvariantCulture)}");
			}

			if (burstEnabled && polynomialsEnabled) {
				var benchmark = new PolynomialsBurst {
					iterations = polynomialsIterations
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"(Burst) Polynomials: {time} ticks. Result: {benchmark.result.ToString(CultureInfo.InvariantCulture)}");
			}

			if (nativeLibraryEnabled && polynomialsEnabled) {
				var benchmark = new PolynomialsGCC {
					iterations = polynomialsIterations
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"({NativeBindings.nativeLibraryPrefixString}) Polynomials: {time} ticks. Result: {benchmark.result.ToString(CultureInfo.InvariantCulture)}");
			}

			if (scriptBackendEnabled && polynomialsEnabled) {
				var benchmark = new PolynomialsBurst {
					iterations = polynomialsIterations
				};

				stopwatch.Stop();
				benchmark.Execute();

				stopwatch.Restart();
				benchmark.Execute();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"({scriptingBackend}) Polynomials: {time} ticks. Result: {benchmark.result.ToString(CultureInfo.InvariantCulture)}");
			}

			if (burstEnabled && particleKinematicsEnabled) {
				var benchmark = new ParticleKinematicsBurst {
					quantity = 1000,
					iterations = particleKinematicsIterations
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"(Burst) Particle Kinematics: {time} ticks. Result: {benchmark.result.ToString(CultureInfo.InvariantCulture)}");
			}

			if (nativeLibraryEnabled && particleKinematicsEnabled) {
				var benchmark = new ParticleKinematicsGCC {
					quantity = 1000,
					iterations = particleKinematicsIterations
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"({NativeBindings.nativeLibraryPrefixString}) Particle Kinematics: {time} ticks. Result: {benchmark.result.ToString(CultureInfo.InvariantCulture)}");
			}

			if (scriptBackendEnabled && particleKinematicsEnabled) {
				var benchmark = new ParticleKinematicsUnity {
					quantity = 1000,
					iterations = particleKinematicsIterations
				};

				stopwatch.Stop();
				benchmark.Execute();

				stopwatch.Restart();
				benchmark.Execute();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"({scriptingBackend}) Particle Kinematics: {time} ticks. Result: {benchmark.result.ToString(CultureInfo.InvariantCulture)}");
			}

			if (burstEnabled && arcfourEnabled) {
				var benchmark = new ArcfourBurst {
					iterations = arcfourIterations
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"(Burst) Arcfour: {time} ticks. Result: {benchmark.result.ToString()}");
			}

			if (nativeLibraryEnabled && arcfourEnabled) {
				var benchmark = new ArcfourGCC {
					iterations = arcfourIterations
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"({NativeBindings.nativeLibraryPrefixString}) Arcfour: {time} ticks. Result: {benchmark.result.ToString()}");
			}

			if (scriptBackendEnabled && arcfourEnabled) {
				var benchmark = new ArcfourBurst {
					iterations = arcfourIterations
				};

				stopwatch.Stop();
				benchmark.Execute();

				stopwatch.Restart();
				benchmark.Execute();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"({scriptingBackend}) Arcfour: {time} ticks. Result: {benchmark.result.ToString()}");
			}

			if (burstEnabled && seahashEnabled) {
				var benchmark = new SeahashBurst {
					iterations = seahashIterations
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"(Burst) Seahash: {time} ticks. Result: {benchmark.result.ToString()}");
			}

			if (nativeLibraryEnabled && seahashEnabled) {
				var benchmark = new SeahashGCC {
					iterations = seahashIterations
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"({NativeBindings.nativeLibraryPrefixString}) Seahash: {time} ticks. Result: {benchmark.result.ToString()}");
			}

			if (scriptBackendEnabled && seahashEnabled) {
				var benchmark = new SeahashUnity {
					iterations = seahashIterations
				};

				stopwatch.Stop();
				benchmark.Execute();

				stopwatch.Restart();
				benchmark.Execute();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"({scriptingBackend}) Seahash: {time} ticks. Result: {benchmark.result.ToString()}");
			}

			if (burstEnabled && radixEnabled) {
				var benchmark = new RadixBurst {
					iterations = radixIterations
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"(Burst) Radix: {time} ticks. Result: {benchmark.result.ToString()}");
			}

			if (nativeLibraryEnabled && radixEnabled) {
				var benchmark = new RadixGCC {
					iterations = radixIterations
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"({NativeBindings.nativeLibraryPrefixString}) Radix: {time} ticks. Result: {benchmark.result.ToString()}");
			}

			if (scriptBackendEnabled && radixEnabled) {
				var benchmark = new RadixUnity {
					iterations = radixIterations
				};

				stopwatch.Stop();
				benchmark.Execute();

				stopwatch.Restart();
				benchmark.Execute();

				time = stopwatch.ElapsedTicks;
				benchmarkResultStreamWriter.WriteLine($"({scriptingBackend}) Radix: {time} ticks. Result: {benchmark.result.ToString()}");
			}
			benchmarkResultStreamWriter.Close();
		}

		Application.Quit();
		#if UNITY_EDITOR
			EditorApplication.ExitPlaymode();
		#endif
	}

	protected override void OnUpdate() {
	}
}