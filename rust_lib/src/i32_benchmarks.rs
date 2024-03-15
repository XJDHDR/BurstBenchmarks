

mod Arcfour;
mod Seahash;
mod Radix;

// Fibonacci
#[no_mangle]
pub extern "C" fn benchmark_fibonacci(Number: u32) -> u32
{
    if Number <= 1
    {
        return 1;
    }

    return benchmark_fibonacci(Number - 1) + benchmark_fibonacci(Number - 2);
}

// Sieve of Eratosthenes
#[no_mangle]
pub extern "C" fn benchmark_sieve_of_eratosthenes(iterations: u32) -> u32
{
    const size: usize = 1024;

    let mut flags: [u8; size] = [0; size];
    let mut a: u32 = 1;
    let mut b: usize;
    let mut c: usize;
    let mut prime: usize;
    let mut count: u32 = 0;

    while (a <= iterations)
    {
        count = 0;

        b = 0;
        while (b < size)
        {
            flags[b] = 1; // True
            b += 1;
        }

        b = 0;
        while (b < size)
        {
            if (flags[b] == 1)
            {
                prime = b + b + 3;
                c = b + prime;

                while (c < size)
                {
                    flags[c] = 0; // False
                    c += prime;
                }

                count += 1;
            }

            b += 1;
        }

        a += 1;
    }

    return count;
}

// Arcfour
#[no_mangle]
pub extern "C" fn benchmark_arcfour(iterations: u32) -> i32
{
    return Arcfour::benchmark_arcfour_main(iterations);
}

// Seahash
#[no_mangle]
pub extern "C" fn benchmark_seahash(iterations: u32) -> u64
{
    return Seahash::benchmark_seahash_main(iterations);
}

// Radix
#[no_mangle]
pub extern "C" fn benchmark_radix(iterations: u32) -> i32
{
    return Radix::benchmark_radix_main(iterations);
}
