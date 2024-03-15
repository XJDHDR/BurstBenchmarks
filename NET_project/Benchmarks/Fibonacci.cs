namespace NET_project.Benchmarks;

public struct FibonacciNET : IJob {
	public uint number;
	public uint result;

	public void Run() {
		result = Fibonacci(number);
	}

	private uint Fibonacci(uint number) {
		if (number <= 1)
			return 1;

		return Fibonacci(number - 1) + Fibonacci(number - 2);
	}
}

internal struct FibonacciGCC : IJob {
	public uint number;
	public uint result;
	public string libName;

	public void Run() {
		switch (libName)
		{
			case "GCC":
				result = GccNativeBindings.benchmark_fibonacci(number);
				break;

			case "Clang":
				result = ClangNativeBindings.benchmark_fibonacci(number);
				break;

			case "MS":
				result = MsNativeBindings.benchmark_fibonacci(number);
				break;

			case "Rust":
				result = RustNativeBindings.benchmark_fibonacci(number);
				break;
		}
	}
}
