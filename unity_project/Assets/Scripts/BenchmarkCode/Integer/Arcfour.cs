using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace BenchmarkCode.Integer
{
	[BurstCompile(CompileSynchronously = true)]
	public unsafe struct ArcfourBurst : IJob
	{
		public uint iterations;
		public int result;

		public void Execute()
		{
			result = Arcfour(iterations);
		}

		private int Arcfour(uint iterations)
		{
			const int keyLength = 5;
			const int streamLength = 10;

			byte* state = (byte*)UnsafeUtility.Malloc(256, 8, Allocator.Persistent);
			byte* buffer = (byte*)UnsafeUtility.Malloc(64, 8, Allocator.Persistent);
			byte* key = stackalloc byte[5];
			byte* stream = stackalloc byte[10];

			key[0] = 0xDB;
			key[1] = 0xB7;
			key[2] = 0x60;
			key[3] = 0xD4;
			key[4] = 0x56;

			stream[0] = 0xEB;
			stream[1] = 0x9F;
			stream[2] = 0x77;
			stream[3] = 0x81;
			stream[4] = 0xB7;
			stream[5] = 0x34;
			stream[6] = 0xCA;
			stream[7] = 0x72;
			stream[8] = 0xA7;
			stream[9] = 0x19;

			int idx = 0;

			for (uint i = 0; i < iterations; i++)
			{
				idx = KeySetup(state, key, keyLength);
				idx = GenerateStream(state, buffer, streamLength);
			}

			UnsafeUtility.Free(state, Allocator.Persistent);
			UnsafeUtility.Free(buffer, Allocator.Persistent);

			return idx;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int KeySetup(byte* state, byte* key, int length)
		{
			int i, j;
			byte t;

			for (i = 0; i < 256; ++i)
			{
				state[i] = (byte)i;
			}

			for (i = 0, j = 0; i < 256; ++i)
			{
				j = (j + state[i] + key[i % length]) % 256;
				t = state[i];
				state[i] = state[j];
				state[j] = t;
			}

			return i;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int GenerateStream(byte* state, byte* buffer, int length)
		{
			int i, j;
			int idx;
			byte t;

			for (idx = 0, i = 0, j = 0; idx < length; ++idx)
			{
				i = (i + 1) % 256;
				j = (j + state[i]) % 256;
				t = state[i];
				state[i] = state[j];
				state[j] = t;
				buffer[idx] = state[(state[i] + state[j]) % 256];
			}

			return i;
		}
	}

	[BurstCompile(CompileSynchronously = true)]
	internal struct ArcfourGCC : IJob
	{
		public uint iterations;
		public int result;

		public void Execute()
		{
			result = NativeBindings.benchmark_arcfour(iterations);
		}
	}
}
