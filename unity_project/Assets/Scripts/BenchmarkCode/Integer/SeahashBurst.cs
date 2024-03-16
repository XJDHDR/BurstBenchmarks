using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace BenchmarkCode.Integer
{
	[BurstCompile(CompileSynchronously = true)]
	public struct SeahashBurst : IJob
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

			NativeArray<byte> buffer = new NativeArray<byte>(bufferLength, Allocator.Persistent);

			for (int i = 0; i < bufferLength; i++) {
				buffer[i] = (byte)(i % 256);
			}

			ulong hash = 0;

			for (uint i = 0; i < iterations; i++)
			{
				hash = Compute(buffer, bufferLength, 0x16F11FE89B0D677C, 0xB480A793D8E6C86C, 0x6FE2E5AAF078EBC9, 0x14F994A4C5259381);
			}

			buffer.Dispose();

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

		private ulong Compute(NativeArray<byte> buffer, ulong length, ulong a, ulong b, ulong c, ulong d)
		{
			const int blockSize = 32;

			ulong end = length & ~((uint)blockSize - 1);

			for (int i = 0; (ulong)i < end; i += blockSize)
			{
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
}
