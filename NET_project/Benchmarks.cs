using System.Diagnostics;
using System.Text;
using NET_project.Benchmarks;

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

	
	[STAThread]
	private static void Main() {
		Process thisProcess = Process.GetCurrentProcess();
		thisProcess.PriorityClass = ProcessPriorityClass.High;
		
		var stopwatch = new Stopwatch();
		long time;
		using (StreamWriter benchmarkResultsStreamWriter = new($"{Directory.GetCurrentDirectory()}/Results.txt", true))
		{
			benchmarkResultsStreamWriter.WriteLine("\r\nBenchmark Results:");

			StringBuilder exceptionLogger = new();

			// Benchmarks
			if (netRyuJitEnabled && fibonacciEnabled)
			{
				try
				{
					var benchmark = new FibonacciNET
					{
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
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred in the RyuJIT Fibonacci benchmark:\r\n{exception}");
					exceptionLogger.Append($"An exception occurred in the RyuJIT Fibonacci benchmark:\r\n{exception}");
				}
			}

			if (nativeLibEnabled && fibonacciEnabled)
			{
				for (int i = 0; i < nativeLibNames.Length; i++)
    			{
				    try
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
				    catch (Exception exception)
				    {
					    Console.WriteLine($"An exception occurred in the {nativeLibNames[i]} Fibonacci benchmark:\r\n{exception}");
					    exceptionLogger.Append($"An exception occurred in the {nativeLibNames[i]} Fibonacci benchmark:\r\n{exception}");
				    }
    			}
			}

			if (netRyuJitEnabled && mandelbrotEnabled)
			{
				try
				{
					var benchmark = new MandelbrotNET
					{
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
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred in the RyuJIT Mandelbrot benchmark:\r\n{exception}");
					exceptionLogger.Append($"An exception occurred in the RyuJIT Mandelbrot benchmark:\r\n{exception}");
				}
			}

			if (nativeLibEnabled && mandelbrotEnabled)
			{
				for (int i = 0; i < nativeLibNames.Length; i++)
				{
					try
					{
						var benchmark = new MandelbrotGCC
						{
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
					catch (Exception exception)
					{
						Console.WriteLine($"An exception occurred in the {nativeLibNames[i]} Mandelbrot benchmark:\r\n{exception}");
						exceptionLogger.Append($"An exception occurred in the {nativeLibNames[i]} Mandelbrot benchmark:\r\n{exception}");
					}
				}
			}

			if (netRyuJitEnabled && nbodyEnabled)
			{
				try
				{
					var benchmark = new NBodyNET
    				{
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
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred in the RyuJIT NBody benchmark:\r\n{exception}");
					exceptionLogger.Append($"An exception occurred in the RyuJIT NBody benchmark:\r\n{exception}");
				}
			}

			if (nativeLibEnabled && nbodyEnabled)
			{
				for (int i = 0; i < nativeLibNames.Length; i++)
				{
					try
					{
						var benchmark = new NBodyGCC
    					{
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
					catch (Exception exception)
					{
						Console.WriteLine($"An exception occurred in the {nativeLibNames[i]} NBody benchmark:\r\n{exception}");
						exceptionLogger.Append($"An exception occurred in the {nativeLibNames[i]} NBody benchmark:\r\n{exception}");
					}
				}
			}

			if (netRyuJitEnabled && sieveOfEratosthenesEnabled)
			{
				try
				{
					var benchmark = new SieveOfEratosthenesNET
					{
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
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred in the RyuJIT Sieve of Eratosthenes benchmark:\r\n{exception}");
					exceptionLogger.Append($"An exception occurred in the RyuJIT Sieve of Eratosthenes benchmark:\r\n{exception}");
				}
			}

			if (nativeLibEnabled && sieveOfEratosthenesEnabled) {
				for (int i = 0; i < nativeLibNames.Length; i++)
				{
					try
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
					catch (Exception exception)
					{
						Console.WriteLine($"An exception occurred in the {nativeLibNames[i]} Sieve of Eratosthenes benchmark:\r\n{exception}");
						exceptionLogger.Append($"An exception occurred in the {nativeLibNames[i]} Sieve of Eratosthenes benchmark:\r\n{exception}");
					}
				}
			}

			if (netRyuJitEnabled && pixarRaytracerEnabled)
			{
				try
				{
					var benchmark = new PixarRaytracerNET
					{
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
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred in the RyuJIT Pixar Raytracer benchmark:\r\n{exception}");
					exceptionLogger.Append($"An exception occurred in the RyuJIT Pixar Raytracer benchmark:\r\n{exception}");
				}
			}
			
			if (nativeLibEnabled && pixarRaytracerEnabled)
			{
				for (int i = 0; i < nativeLibNames.Length; i++)
				{
					try
					{
						var benchmark = new PixarRaytracerGCC
						{
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
					catch (Exception exception)
					{
						Console.WriteLine($"An exception occurred in the {nativeLibNames[i]} Pixar Raytracer benchmark:\r\n{exception}");
						exceptionLogger.Append($"An exception occurred in the {nativeLibNames[i]} Pixar Raytracer benchmark:\r\n{exception}");
					}
				}
			}

			if (netRyuJitEnabled && firefliesFlockingEnabled)
			{
				try
				{
					var benchmark = new FirefliesFlockingNET
					{
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
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred in the RyuJIT Fireflies Flocking benchmark:\r\n{exception}");
					exceptionLogger.Append($"An exception occurred in the RyuJIT Fireflies Flocking benchmark:\r\n{exception}");
				}
			}
			
			if (nativeLibEnabled && firefliesFlockingEnabled)
			{
				for (int i = 0; i < nativeLibNames.Length; i++)
				{
					try
					{
						var benchmark = new FirefliesFlockingGCC
						{
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
					catch (Exception exception)
					{
						Console.WriteLine($"An exception occurred in the {nativeLibNames[i]} Fireflies Flocking benchmark:\r\n{exception}");
						exceptionLogger.Append($"An exception occurred in the {nativeLibNames[i]} Fireflies Flocking benchmark:\r\n{exception}");
					}
				}
			}

			if (netRyuJitEnabled && polynomialsEnabled)
			{
				try
				{
					var benchmark = new PolynomialsNET
    				{
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
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred in the RyuJIT Polynomials benchmark:\r\n{exception}");
					exceptionLogger.Append($"An exception occurred in the RyuJIT Polynomials benchmark:\r\n{exception}");
				}
			}
			
			if (nativeLibEnabled && polynomialsEnabled)
			{
				for (int i = 0; i < nativeLibNames.Length; i++)
				{
					try
					{
						var benchmark = new PolynomialsGCC
    					{
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
					catch (Exception exception)
					{
						Console.WriteLine($"An exception occurred in the {nativeLibNames[i]} Polynomials benchmark:\r\n{exception}");
						exceptionLogger.Append($"An exception occurred in the {nativeLibNames[i]} Polynomials benchmark:\r\n{exception}");
					}
				}
			}

			if (netRyuJitEnabled && particleKinematicsEnabled)
			{
				try
				{
					var benchmark = new ParticleKinematicsNET
					{
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
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred in the RyuJIT Particle Kinematics benchmark:\r\n{exception}");
					exceptionLogger.Append($"An exception occurred in the RyuJIT Particle Kinematics benchmark:\r\n{exception}");
				}
			}
			
			if (nativeLibEnabled && particleKinematicsEnabled) {
				for (int i = 0; i < nativeLibNames.Length; i++)
				{
					try
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
					catch (Exception exception)
					{
						Console.WriteLine($"An exception occurred in the {nativeLibNames[i]} Particle Kinematics benchmark:\r\n{exception}");
						exceptionLogger.Append($"An exception occurred in the {nativeLibNames[i]} Particle Kinematics benchmark:\r\n{exception}");
					}
				}
			}

			if (netRyuJitEnabled && arcfourEnabled)
			{
				try
				{
					var benchmark = new ArcfourNET
					{
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
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred in the RyuJIT Arcfour benchmark:\r\n{exception}");
					exceptionLogger.Append($"An exception occurred in the RyuJIT Arcfour benchmark:\r\n{exception}");
				}
			}
			
			if (nativeLibEnabled && arcfourEnabled)
			{
				for (int i = 0; i < nativeLibNames.Length; i++)
				{
					try
					{
						var benchmark = new ArcfourGCC
						{
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
					catch (Exception exception)
					{
						Console.WriteLine($"An exception occurred in the {nativeLibNames[i]} Arcfour benchmark:\r\n{exception}");
						exceptionLogger.Append($"An exception occurred in the {nativeLibNames[i]} Arcfour benchmark:\r\n{exception}");
					}
				}
			}

			if (netRyuJitEnabled && seahashEnabled)
			{
				try
				{
					var benchmark = new SeahashNET
					{
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
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred in the RyuJIT Seahash benchmark:\r\n{exception}");
					exceptionLogger.Append($"An exception occurred in the RyuJIT Seahash benchmark:\r\n{exception}");
				}
			}
			
			if (nativeLibEnabled && seahashEnabled)
			{
				for (int i = 0; i < nativeLibNames.Length; i++)
				{
					try
					{
						var benchmark = new SeahashGCC
						{
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
					catch (Exception exception)
					{
						Console.WriteLine($"An exception occurred in the {nativeLibNames[i]} Seahash benchmark:\r\n{exception}");
						exceptionLogger.Append($"An exception occurred in the {nativeLibNames[i]} Seahash benchmark:\r\n{exception}");
					}
				}
			}

			if (netRyuJitEnabled && radixEnabled)
			{
				try
				{
					var benchmark = new RadixNET
					{
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
				catch (Exception exception)
				{
					Console.WriteLine($"An exception occurred in the RyuJIT Radix benchmark:\r\n{exception}");
					exceptionLogger.Append($"An exception occurred in the RyuJIT Radix benchmark:\r\n{exception}");
				}
			}

			if (nativeLibEnabled && radixEnabled)
			{
				for (int i = 0; i < nativeLibNames.Length; i++)
				{
					try
					{
						var benchmark = new RadixGCC
						{
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
					catch (Exception exception)
					{
						Console.WriteLine($"An exception occurred in the {nativeLibNames[i]} Radix benchmark:\r\n{exception}");
						exceptionLogger.Append($"An exception occurred in the {nativeLibNames[i]} Radix benchmark:\r\n{exception}");
					}
				}
			}
			
			benchmarkResultsStreamWriter.WriteLine($"\r\n\r\n{exceptionLogger}");

			benchmarkResultsStreamWriter.Close();
		}
		Environment.Exit(0);
	}
}
