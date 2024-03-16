using System;
using System.Globalization;
using System.IO;
using System.Text;
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
		burstEnabled = false,
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

			StringBuilder exceptionMessages = new();
			
			if (burstEnabled && fibonacciEnabled)
			{
				try
				{
					var benchmark = new FibonacciBurst
					{
						number = fibonacciNumber
					};

					stopwatch.Stop();
					benchmark.Run();

					stopwatch.Restart();
					benchmark.Run();

					time = stopwatch.ElapsedTicks;

					benchmarkResultStreamWriter.WriteLine($"(Burst) Fibonacci: {time} ticks. Result: {benchmark.result.ToString()}");
				}
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred while running the Burst Fibonacci benchmark:\r\n{exception}");
					exceptionMessages.AppendLine($"An exception occurred while running the Burst Fibonacci benchmark:\r\n{exception}");
				}
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

			if (scriptBackendEnabled && fibonacciEnabled)
			{
				try
				{
					var benchmark = new FibonacciBurst
					{
						number = fibonacciNumber
					};

					stopwatch.Stop();
					benchmark.Execute();

					stopwatch.Restart();
					benchmark.Execute();

					time = stopwatch.ElapsedTicks;

					benchmarkResultStreamWriter.WriteLine($"({scriptingBackend}) Fibonacci: {time} ticks. Result: {benchmark.result.ToString()}");
				}
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred while running the {scriptingBackend} Fibonacci benchmark:\r\n{exception}");
					exceptionMessages.AppendLine($"An exception occurred while running the {scriptingBackend} Fibonacci benchmark:\r\n{exception}");
				}
			}

			if (burstEnabled && mandelbrotEnabled)
			{
				try
				{
					var benchmark = new MandelbrotBurst
					{
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
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred while running the Burst Mandelbrot benchmark:\r\n{exception}");
					exceptionMessages.AppendLine($"An exception occurred while running the Burst Mandelbrot benchmark:\r\n{exception}");
				}
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

			if (scriptBackendEnabled && mandelbrotEnabled)
			{
				try
				{
					var benchmark = new MandelbrotBurst
					{
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
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred while running the {scriptingBackend} Mandelbrot benchmark:\r\n{exception}");
					exceptionMessages.AppendLine($"An exception occurred while running the {scriptingBackend} Mandelbrot benchmark:\r\n{exception}");
				}
			}

			if (burstEnabled && nbodyEnabled)
			{
				try
				{
					var benchmark = new NBodyBurst
					{
						advancements = nbodyAdvancements
					};

					stopwatch.Stop();
					benchmark.Run();

					stopwatch.Restart();
					benchmark.Run();

					time = stopwatch.ElapsedTicks;

					benchmarkResultStreamWriter.WriteLine($"(Burst) NBody: {time} ticks. Result: {benchmark.result.ToString(CultureInfo.InvariantCulture)}");
				}
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred while running the Burst NBody benchmark:\r\n{exception}");
					exceptionMessages.AppendLine($"An exception occurred while running the Burst NBody benchmark:\r\n{exception}");
				}
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

			if (scriptBackendEnabled && nbodyEnabled)
			{
				try
				{
					var benchmark = new NBodyBurst
					{
						advancements = nbodyAdvancements
					};

					stopwatch.Stop();
					benchmark.Execute();

					stopwatch.Restart();
					benchmark.Execute();

					time = stopwatch.ElapsedTicks;

					benchmarkResultStreamWriter.WriteLine($"({scriptingBackend}) NBody: {time} ticks. Result: {benchmark.result.ToString(CultureInfo.InvariantCulture)}");
				}
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred while running the {scriptingBackend} NBody benchmark:\r\n{exception}");
					exceptionMessages.AppendLine($"An exception occurred while running the {scriptingBackend} NBody benchmark:\r\n{exception}");
				}
			}

			if (burstEnabled && sieveOfEratosthenesEnabled)
			{
				try
				{
					var benchmark = new SieveOfEratosthenesBurst
					{
						iterations = sieveOfEratosthenesIterations
					};

					stopwatch.Stop();
					benchmark.Run();

					stopwatch.Restart();
					benchmark.Run();

					time = stopwatch.ElapsedTicks;

					benchmarkResultStreamWriter.WriteLine($"(Burst) Sieve of Eratosthenes: {time} ticks. Result: {benchmark.result.ToString()}");
				}
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred while running the Burst Sieve of Eratosthenes benchmark:\r\n{exception}");
					exceptionMessages.AppendLine($"An exception occurred while running the Sieve of Eratosthenes NBody benchmark:\r\n{exception}");
				}
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

			if (scriptBackendEnabled && sieveOfEratosthenesEnabled)
			{
				try
				{
					var benchmark = new SieveOfEratosthenesBurst
					{
						iterations = sieveOfEratosthenesIterations
					};

					stopwatch.Stop();
					benchmark.Execute();

					stopwatch.Restart();
					benchmark.Execute();

					time = stopwatch.ElapsedTicks;

					benchmarkResultStreamWriter.WriteLine($"({scriptingBackend}) Sieve of Eratosthenes: {time} ticks. Result: {benchmark.result.ToString(CultureInfo.InvariantCulture)}");
				}
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred while running the {scriptingBackend} Sieve of Eratosthenes benchmark:\r\n{exception}");
					exceptionMessages.AppendLine($"An exception occurred while running the {scriptingBackend} Sieve of Eratosthenes benchmark:\r\n{exception}");
				}
			}

			if (burstEnabled && pixarRaytracerEnabled)
			{
				try
				{
					var benchmark = new PixarRaytracerBurst
					{
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
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred while running the Burst Pixar Raytracer benchmark:\r\n{exception}");
					exceptionMessages.AppendLine($"An exception occurred while running the Pixar Raytracer NBody benchmark:\r\n{exception}");
				}
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

			if (scriptBackendEnabled && pixarRaytracerEnabled)
			{
				try
				{
					var benchmark = new PixarRaytracerUnity
    				{
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
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred while running the {scriptingBackend} Pixar Raytracer benchmark:\r\n{exception}");
					exceptionMessages.AppendLine($"An exception occurred while running the {scriptingBackend} Pixar Raytracer benchmark:\r\n{exception}");
				}
			}

			if (burstEnabled && firefliesFlockingEnabled)
			{
				try
				{
					var benchmark = new FirefliesFlockingBurst
					{
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
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred while running the Burst Fireflies Flocking benchmark:\r\n{exception}");
					exceptionMessages.AppendLine($"An exception occurred while running the Fireflies Flocking NBody benchmark:\r\n{exception}");
				}
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

			if (scriptBackendEnabled && firefliesFlockingEnabled)
			{
				try
				{
					var benchmark = new FirefliesFlockingUnity
					{
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
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred while running the {scriptingBackend} Fireflies Flocking benchmark:\r\n{exception}");
					exceptionMessages.AppendLine($"An exception occurred while running the {scriptingBackend} Fireflies Flocking benchmark:\r\n{exception}");
				}
			}

			if (burstEnabled && polynomialsEnabled)
			{
				try
				{
					var benchmark = new PolynomialsBurst
					{
						iterations = polynomialsIterations
					};

					stopwatch.Stop();
					benchmark.Run();

					stopwatch.Restart();
					benchmark.Run();

					time = stopwatch.ElapsedTicks;

					benchmarkResultStreamWriter.WriteLine($"(Burst) Polynomials: {time} ticks. Result: {benchmark.result.ToString(CultureInfo.InvariantCulture)}");
				}
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred while running the Burst Polynomials benchmark:\r\n{exception}");
					exceptionMessages.AppendLine($"An exception occurred while running the Polynomials NBody benchmark:\r\n{exception}");
				}
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

			if (scriptBackendEnabled && polynomialsEnabled)
			{
				try
				{
					var benchmark = new PolynomialsBurst
					{
						iterations = polynomialsIterations
					};

					stopwatch.Stop();
					benchmark.Execute();

					stopwatch.Restart();
					benchmark.Execute();

					time = stopwatch.ElapsedTicks;

					benchmarkResultStreamWriter.WriteLine($"({scriptingBackend}) Polynomials: {time} ticks. Result: {benchmark.result.ToString(CultureInfo.InvariantCulture)}");
				}
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred while running the {scriptingBackend} Polynomials benchmark:\r\n{exception}");
					exceptionMessages.AppendLine($"An exception occurred while running the {scriptingBackend} Polynomials benchmark:\r\n{exception}");
				}
			}

			if (burstEnabled && particleKinematicsEnabled)
			{
				try
				{
					var benchmark = new ParticleKinematicsBurst
					{
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
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred while running the Burst Particle Kinematics benchmark:\r\n{exception}");
					exceptionMessages.AppendLine($"An exception occurred while running the Particle Kinematics NBody benchmark:\r\n{exception}");
				}
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

			if (scriptBackendEnabled && particleKinematicsEnabled)
			{
				try
				{
					var benchmark = new ParticleKinematicsUnity
					{
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
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred while running the {scriptingBackend} Particle Kinematics benchmark:\r\n{exception}");
					exceptionMessages.AppendLine($"An exception occurred while running the {scriptingBackend} Particle Kinematics benchmark:\r\n{exception}");
				}
			}

			if (burstEnabled && arcfourEnabled)
			{
				try
				{
					var benchmark = new ArcfourBurst
					{
						iterations = arcfourIterations
					};

					stopwatch.Stop();
					benchmark.Run();

					stopwatch.Restart();
					benchmark.Run();

					time = stopwatch.ElapsedTicks;

					benchmarkResultStreamWriter.WriteLine($"(Burst) Arcfour: {time} ticks. Result: {benchmark.result.ToString()}");
				}
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred while running the Burst Arcfour benchmark:\r\n{exception}");
					exceptionMessages.AppendLine($"An exception occurred while running the Arcfour NBody benchmark:\r\n{exception}");
				}
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

			if (scriptBackendEnabled && arcfourEnabled)
			{
				try
				{
					var benchmark = new ArcfourBurst
					{
						iterations = arcfourIterations
					};

					stopwatch.Stop();
					benchmark.Execute();

					stopwatch.Restart();
					benchmark.Execute();

					time = stopwatch.ElapsedTicks;

					benchmarkResultStreamWriter.WriteLine($"({scriptingBackend}) Arcfour: {time} ticks. Result: {benchmark.result.ToString()}");
				}
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred while running the {scriptingBackend} Arcfour benchmark:\r\n{exception}");
					exceptionMessages.AppendLine($"An exception occurred while running the {scriptingBackend} Arcfour benchmark:\r\n{exception}");
				}
			}

			if (burstEnabled && seahashEnabled)
			{
				try
				{
					var benchmark = new SeahashBurst
					{
						iterations = seahashIterations
					};

					stopwatch.Stop();
					benchmark.Run();

					stopwatch.Restart();
					benchmark.Run();

					time = stopwatch.ElapsedTicks;

					benchmarkResultStreamWriter.WriteLine($"(Burst) Seahash: {time} ticks. Result: {benchmark.result.ToString()}");
				}
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred while running the Burst Seahash benchmark:\r\n{exception}");
					exceptionMessages.AppendLine($"An exception occurred while running the Seahash NBody benchmark:\r\n{exception}");
				}
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

			if (scriptBackendEnabled && seahashEnabled)
			{
				try
				{
					var benchmark = new SeahashUnity
					{
						iterations = seahashIterations
					};

					stopwatch.Stop();
					benchmark.Execute();

					stopwatch.Restart();
					benchmark.Execute();

					time = stopwatch.ElapsedTicks;

					benchmarkResultStreamWriter.WriteLine($"({scriptingBackend}) Seahash: {time} ticks. Result: {benchmark.result.ToString()}");
				}
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred while running the {scriptingBackend} Seahash benchmark:\r\n{exception}");
					exceptionMessages.AppendLine($"An exception occurred while running the {scriptingBackend} Seahash benchmark:\r\n{exception}");
				}
			}

			if (burstEnabled && radixEnabled)
			{
				try
				{
					var benchmark = new RadixBurst
					{
						iterations = radixIterations
					};

					stopwatch.Stop();
					benchmark.Run();

					stopwatch.Restart();
					benchmark.Run();

					time = stopwatch.ElapsedTicks;

					benchmarkResultStreamWriter.WriteLine($"(Burst) Radix: {time} ticks. Result: {benchmark.result.ToString()}");
				}
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred while running the Burst Radix benchmark:\r\n{exception}");
					exceptionMessages.AppendLine($"An exception occurred while running the Radix NBody benchmark:\r\n{exception}");
				}
			}

			if (nativeLibraryEnabled && radixEnabled)
			{
				var benchmark = new RadixGCC
				{
					iterations = radixIterations
				};

				stopwatch.Stop();
				benchmark.Run();

				stopwatch.Restart();
				benchmark.Run();

				time = stopwatch.ElapsedTicks;

				benchmarkResultStreamWriter.WriteLine($"({NativeBindings.nativeLibraryPrefixString}) Radix: {time} ticks. Result: {benchmark.result.ToString()}");
			}

			if (scriptBackendEnabled && radixEnabled)
			{
				try
				{
					var benchmark = new RadixUnity
					{
						iterations = radixIterations
					};

					stopwatch.Stop();
					benchmark.Execute();

					stopwatch.Restart();
					benchmark.Execute();

					time = stopwatch.ElapsedTicks;
					benchmarkResultStreamWriter.WriteLine($"({scriptingBackend}) Radix: {time} ticks. Result: {benchmark.result.ToString()}");
				}
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred while running the {scriptingBackend} Radix benchmark:\r\n{exception}");
					exceptionMessages.AppendLine($"An exception occurred while running the {scriptingBackend} Radix benchmark:\r\n{exception}");
				}
			}
			
			benchmarkResultStreamWriter.WriteLine($"\r\n\r\n{exceptionMessages}");
			
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