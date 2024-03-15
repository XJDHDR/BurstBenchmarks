using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace BenchmarkCode.Integer
{
	[BurstCompile(CompileSynchronously = true)]
	public unsafe struct SeahashBurst : IJob
	{
		public uint iterations;
		public ulong result;

		public void Execute()
		{
			result = Seahash(iterations);
		}

		private ulong Seahash(uint iterations)
		{
			const int bufferLength = 1024 * 128;

			byte* buffer = (byte*)UnsafeUtility.Malloc(bufferLength, 8, Allocator.Persistent);

			for (int i = 0; i < bufferLength; i++) {
				buffer[i] = (byte)(i % 256);
			}

			ulong hash = 0;

			for (uint i = 0; i < iterations; i++)
			{
				hash = Compute(buffer, bufferLength, 0x16F11FE89B0D677C, 0xB480A793D8E6C86C, 0x6FE2E5AAF078EBC9, 0x14F994A4C5259381);
			}

			UnsafeUtility.Free(buffer, Allocator.Persistent);

			return hash;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private ulong Read(byte* pointer)
		{
			return *(ulong*)pointer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private ulong Diffuse(ulong value)
		{
			value *= 0x6EED0E9DA4D94A4F;
			value ^= ((value >> 32) >> (int)(value >> 60));
			value *= 0x6EED0E9DA4D94A4F;

			return value;
		}

		private ulong Compute(byte* buffer, ulong length, ulong a, ulong b, ulong c, ulong d)
		{
			const uint blockSize = 32;

			ulong end = length & ~(blockSize - 1);

			for (uint i = 0; i < end; i += blockSize)
			{
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

			if (excessive > 0)
			{
				a ^= Read(bufferEnd);

				if (excessive > 8)
				{
					b ^= Read(bufferEnd);

					if (excessive > 16)
					{
						c ^= Read(bufferEnd);

						if (excessive > 24)
						{
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

	[BurstCompile(CompileSynchronously = true)]
	internal struct SeahashGCC : IJob
	{
		public uint iterations;
		public ulong result;

		public void Execute()
		{
			result = NativeBindings.benchmark_seahash(iterations);
		}
	}
}
