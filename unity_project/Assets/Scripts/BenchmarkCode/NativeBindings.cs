//#define NATIVE_LIB_IS_GCC
//#define NATIVE_LIB_IS_CLANG
#define NATIVE_LIB_IS_MS
//#define NATIVE_LIB_IS_RUST

using System.Runtime.InteropServices;

namespace BenchmarkCode
{
	internal static class NativeBindings
	{
	#if NATIVE_LIB_IS_GCC
		internal const string nativeLibraryPrefixString = "GCC";
		private const string nativeLibrary = "benchmarks_gcc";
	#elif NATIVE_LIB_IS_CLANG
		internal const string nativeLibraryPrefixString = "Clang";
		private const string nativeLibrary = "benchmarks_clang";
	#elif NATIVE_LIB_IS_MS
		internal const string nativeLibraryPrefixString = "MS";
		private const string nativeLibrary = "benchmarks_ms";
	#elif NATIVE_LIB_IS_RUST
		internal const string nativeLibraryPrefixString = "Rust";
		private const string nativeLibrary = "benchmarks_rust";
	#endif

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern uint benchmark_fibonacci(uint number);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern float benchmark_mandelbrot(uint width, uint height, uint iterations);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern double benchmark_nbody(uint advancements);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern uint benchmark_sieve_of_eratosthenes(uint iterations);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern float benchmark_pixar_raytracer(uint width, uint height, uint samples);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern float benchmark_fireflies_flocking(uint boids, uint lifetime);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern float benchmark_polynomials(uint iterations);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern float benchmark_particle_kinematics(uint quantity, uint iterations);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int benchmark_arcfour(uint iterations);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern ulong benchmark_seahash(uint iterations);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int benchmark_radix(uint iterations);
	}
}
