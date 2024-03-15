namespace NET_project.Benchmarks;

public unsafe struct SieveOfEratosthenesNET : IJob {
	public uint iterations;
	public uint result;

	public void Run() {
		result = SieveOfEratosthenes(iterations);
	}

	private uint SieveOfEratosthenes(uint iterations) {
		const int size = 1024;

		byte* flags = stackalloc byte[size];
		uint a, b, c, prime, count = 0;

		for (a = 1; a <= iterations; a++) {
			count = 0;

			for (b = 0; b < size; b++) {
				flags[b] = 1; // True
			}

			for (b = 0; b < size; b++) {
				if (flags[b] == 1) {
					prime = b + b + 3;
					c = b + prime;

					while (c < size) {
						flags[c] = 0; // False
						c += prime;
					}

					count++;
				}
			}
		}

		return count;
	}
}

internal unsafe struct SieveOfEratosthenesGCC : IJob {
	public uint iterations;
	public uint result;
	public string libName;

	public void Run() {
		switch (libName)
		{
			case "GCC":
				result = GccNativeBindings.benchmark_sieve_of_eratosthenes(iterations);
				break;

			case "Clang":
				result = ClangNativeBindings.benchmark_sieve_of_eratosthenes(iterations);
				break;

			case "MS":
				result = MsNativeBindings.benchmark_sieve_of_eratosthenes(iterations);
				break;

			case "Rust":
				result = RustNativeBindings.benchmark_sieve_of_eratosthenes(iterations);
				break;
		}
	}
}
