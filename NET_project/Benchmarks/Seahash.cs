using System.Runtime.CompilerServices;

namespace NET_project.Benchmarks;

public unsafe struct SeahashNET : IJob {
	public uint iterations;
	public ulong result;

	public void Run() {
		result = Seahash(iterations);
	}

	private ulong Seahash(uint Iterations)
	{
		const int bufferLength = 1024 * 128;

		byte[] buffer = new byte[bufferLength];

		for (int i = 0; i < bufferLength; i++)
		{
			buffer[i] = (byte)(i % 256);
		}

		ulong hash = 0;

		for (uint i = 0; i < Iterations; i++)
		{
			hash = Compute(buffer, bufferLength, 0x16F11FE89B0D677C, 0xB480A793D8E6C86C, 0x6FE2E5AAF078EBC9, 0x14F994A4C5259381);
		}

		return hash;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private ulong Diffuse(ulong value)
	{
		value *= 0x6EED0E9DA4D94A4F;
		value ^= ((value >> 32) >> (int)(value >> 60));
		value *= 0x6EED0E9DA4D94A4F;

		return value;
	}

	private ulong Compute(byte[] buffer, ulong length, ulong a, ulong b, ulong c, ulong d) {
		const uint blockSize = 32;

		ulong end = length & ~(blockSize - 1);

		for (uint i = 0; i < end; i += blockSize) {
			a ^= buffer[i];
			b ^= buffer[i + 8];
			c ^= buffer[i + 16];
			d ^= buffer[i + 24];

			a = Diffuse(a);
			b = Diffuse(b);
			c = Diffuse(c);
			d = Diffuse(d);
		}

		ulong excessive = length - end;

		if (excessive > 0)
		{
			a ^= buffer[^1];

			if (excessive > 8)
			{
				b ^= buffer[^1];

				if (excessive > 16)
				{
					c ^= buffer[^1];

					if (excessive > 24)
					{
						d ^= buffer[^1];
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

internal unsafe struct SeahashGCC : IJob {
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
