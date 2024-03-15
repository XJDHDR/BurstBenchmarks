namespace NET_project.Benchmarks;

public unsafe struct PolynomialsNET : IJob
{
	public uint iterations;
	public float result;

	public void Run() {
		result = Polynomials(iterations);
	}

	private float Polynomials(uint iterations)
	{
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
