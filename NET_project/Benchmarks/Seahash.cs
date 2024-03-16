using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NET_project.Benchmarks;

public unsafe struct SeahashNET : IJob {
	public uint iterations;
	public ulong result;

	public void Run() {
		result = Seahash(iterations);
	}

	private static ulong Seahash(uint Iterations) {
		const int bufferLength = 1024 * 128;

		// Using span here would be more complex due to it unsafely reading
		// 8 byte pieces of data and depending on alignment to make overreading
		// it does "safe".
		byte* buffer = (byte*)NativeMemory.AlignedAlloc(bufferLength, 8);

		for (int i = 0; i < bufferLength; i++) {
			buffer[i] = (byte)(i % 256);
		}

		ulong hash = 0;

		for (uint i = 0; i < Iterations; i++) {
			hash = Compute(buffer, bufferLength, 0x16F11FE89B0D677C, 0xB480A793D8E6C86C, 0x6FE2E5AAF078EBC9, 0x14F994A4C5259381);
		}

		NativeMemory.AlignedFree(buffer);

		return hash;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong Read(byte* pointer) {
		return *(ulong*)pointer;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong Diffuse(ulong value) {
		value *= 0x6EED0E9DA4D94A4F;
		value ^= value >> 32 >> (int)(value >> 60);
		value *= 0x6EED0E9DA4D94A4F;

		return value;
	}

	private static ulong Compute(byte* buffer, ulong length, ulong a, ulong b, ulong c, ulong d) {
		const uint blockSize = 32;

		ulong end = length & ~(blockSize - 1);

		for (uint i = 0; i < end; i += blockSize) {
			a ^= Read(buffer + i);
			b ^= Read(buffer + i + 8);
			c ^= Read(buffer + i + 16);
			d ^= Read(buffer + i + 24);

			a = Diffuse(a);
			b = Diffuse(b);
			c = Diffuse(c);
			d = Diffuse(d);
		}

		ulong excessive = length - end;
		byte* bufferEnd = buffer + end;

		if (excessive > 0) {
			a ^= Read(bufferEnd);

			if (excessive > 8) {
				b ^= Read(bufferEnd);

				if (excessive > 16) {
					c ^= Read(bufferEnd);

					if (excessive > 24) {
						d ^= Read(bufferEnd);
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

internal unsafe struct SeahashGCC : IJob
{
	public uint iterations;
	public ulong result;
	public string libName;

	public void Run()
	{
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
