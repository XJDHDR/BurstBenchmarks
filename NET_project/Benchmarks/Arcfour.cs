using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NET_project.Benchmarks;

[SkipLocalsInit]
public unsafe struct ArcfourNET : IJob
{
	public uint iterations;
	public int result;

	public void Run() {
		result = Arcfour(iterations);
	}

	private static int Arcfour(uint iterations) {
		const int keyLength = 5;
		const int streamLength = 10;

		// Using span here would be more complex due to the general
		// inability to prove that several accesses are safe due to
		// the index being the value of another buffer i.e. x[y[n]]

		byte* state = (byte*)NativeMemory.AlignedAlloc(256, 8);
		byte* buffer = (byte*)NativeMemory.AlignedAlloc(64, 8);
		byte* key = stackalloc byte[keyLength] {
			0xDB,
			0xB7,
			0x60,
			0xD4,
			0x56,
		};
		byte* stream = stackalloc byte[streamLength] {
			0xEB,
			0x9F,
			0x77,
			0x81,
			0xB7,
			0x34,
			0xCA,
			0x72,
			0xA7,
			0x19,
		};

		int idx = 0;

		for (uint i = 0; i < iterations; i++) {
			idx = KeySetup(state, key, keyLength);
			idx = GenerateStream(state, buffer, streamLength);
		}

		NativeMemory.AlignedFree(state);
		NativeMemory.AlignedFree(buffer);

		return idx;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int KeySetup(byte* state, byte* key, int keyLength)
	{
		for (uint i = 0; i < 256; i++) {
			state[i] = (byte)i;
		}

		uint j = 0;

		for (uint i = 0; i < 256; i++) {
			j = (j + state[i] + key[i % keyLength]) % 256;
			byte t = state[i];
			state[i] = state[j];
			state[j] = t;
		}

		return 256;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GenerateStream(byte* state, byte* buffer, int length)
	{
		uint i = 0;
		uint j = 0;

		for (int idx = 0; idx < length; idx++) {
			i = (i + 1) % 256;
			j = (j + state[i]) % 256;
			byte t = state[i];
			state[i] = state[j];
			state[j] = t;
			buffer[idx] = state[(state[i] + state[j]) % 256];
		}

		return (int)i;
	}
}

internal unsafe struct ArcfourGCC : IJob
{
	public uint iterations;
	public int result;
	public string libName;

	public void Run()
	{
		switch (libName)
		{
			case "GCC":
				result = GccNativeBindings.benchmark_arcfour(iterations);
				break;

			case "Clang":
				result = ClangNativeBindings.benchmark_arcfour(iterations);
				break;

			case "MS":
				result = MsNativeBindings.benchmark_arcfour(iterations);
				break;

			case "Rust":
				result = RustNativeBindings.benchmark_arcfour(iterations);
				break;
		}
	}
}
