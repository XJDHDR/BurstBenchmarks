

const bufferLength: usize = 1024 * 128;

fn benchmark_seahash_compute(buffer: [u8; bufferLength], mut a: u64, mut b: u64, mut c: u64, mut d: u64) -> u64
{
    const blockSize: usize = 32;

    let end: usize = bufferLength & !(blockSize - 1);

    let mut i: usize = 0;
    while (i < end)
    {
        a ^= benchmark_seahash_read(&buffer, i);
        b ^= benchmark_seahash_read(&buffer, i + 8);
        c ^= benchmark_seahash_read(&buffer, i + 16);
        d ^= benchmark_seahash_read(&buffer, i + 24);

        a = benchmark_seahash_diffuse(a);
        b = benchmark_seahash_diffuse(b);
        c = benchmark_seahash_diffuse(c);
        d = benchmark_seahash_diffuse(d);

        i += blockSize;
    }

    let excessive: u64 = (bufferLength - end) as u64;

    if (excessive > 0)
    {
        a ^= benchmark_seahash_read(&buffer, i);

        if (excessive > 8)
        {
            b ^= benchmark_seahash_read(&buffer, i);

            if (excessive > 16)
            {
                c ^= benchmark_seahash_read(&buffer, i);

                if (excessive > 24)
                {
                    d ^= benchmark_seahash_read(&buffer, i);
                    d = benchmark_seahash_diffuse(d);
                }

                c = benchmark_seahash_diffuse(c);
            }

            b = benchmark_seahash_diffuse(b);
        }

        a = benchmark_seahash_diffuse(a);
    }

    a ^= b;
    c ^= d;
    a ^= c;
    a ^= bufferLength as u64;

    return benchmark_seahash_diffuse(a);
}

fn benchmark_seahash_diffuse(mut value: u64) -> u64
{
    value *= 0x6EED0E9DA4D94A4F;
    value ^= ((value >> 32) >> ((value >> 60)) as i32);
    value *= 0x6EED0E9DA4D94A4F;

    return value;
}

fn benchmark_seahash_read(buffer: &[u8; bufferLength], i: usize) -> u64
{
    return (buffer[i]) as u64 |
        (buffer[i + 1] as u64) << 8 |
        (buffer[i + 2] as u64) << 16 |
        (buffer[i + 3] as u64) << 24 |
        (buffer[i + 4] as u64) << 32 |
        (buffer[i + 5] as u64) << 40 |
        (buffer[i + 6] as u64) << 48 |
        (buffer[i + 7] as u64) << 56;
}

pub(crate) fn benchmark_seahash_main(iterations: u32) -> u64
{
    let mut buffer: [u8; bufferLength] = [0; bufferLength];

    let mut i: usize = 0;
    while (i < bufferLength)
    {
        buffer[i] = (i % 256) as u8;
        i += 1;
    }

    let mut hash: u64 = 0;

    let mut i: u32 = 0;
    while (i < iterations)
    {
        hash = benchmark_seahash_compute(buffer, 0x16F11FE89B0D677C, 0xB480A793D8E6C86C, 0x6FE2E5AAF078EBC9, 0x14F994A4C5259381);
        i += 1;
    }

    return hash;
}
