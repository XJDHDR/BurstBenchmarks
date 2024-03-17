using System.Runtime.CompilerServices;

namespace NET_project.Benchmarks;

[SkipLocalsInit]
public unsafe struct PolynomialsNET : IJob
{
	public uint iterations;
	public float result;

	public void Run() {
		result = Polynomials(iterations);
	}

	private static float Polynomials(uint iterations) {
		const float x = 0.2f;

		float pu = 0.0f;
		Span<float> poly = stackalloc float[100];

		for (uint i = 0; i < iterations; i++) {
			float mu = 10.0f;

			for (int j = 0; j < poly.Length; j++) {
				mu = (mu + 2.0f) / 2.0f;
				poly[j] = mu;
			}

			float s = 0.0f;

			for (int j = 0; j < poly.Length; j++) {
				s = (x * s) + poly[j];
			}

			pu += s;
		}

		return pu;
	}
}

internal unsafe struct PolynomialsGCC : IJob
{
	public uint iterations;
	public float result;
	public string libName;

	public void Run()
	{
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
