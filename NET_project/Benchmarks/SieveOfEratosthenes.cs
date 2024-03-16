using System.Runtime.CompilerServices;

namespace NET_project.Benchmarks;

[SkipLocalsInit]
public unsafe struct SieveOfEratosthenesNET : IJob {
	public uint iterations;
	public uint result;

	public void Run() {
		result = SieveOfEratosthenes(iterations);
	}

	private static uint SieveOfEratosthenes(uint iterations) {
		Span<byte> flags = stackalloc byte[1024];
		uint count = 0;

		for (uint a = 1; a <= iterations; a++) {
			count = 0;

			flags.Fill(1);

			for (uint b = 0; b < flags.Length; b++) {
				if (flags[(int)b] == 1) {
					uint prime = b + b + 3;

					for (uint c = b + prime; c < flags.Length; c += prime) {
						flags[(int)c] = 0;
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
