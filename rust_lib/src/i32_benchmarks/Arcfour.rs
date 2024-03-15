

const keyLength: usize = 5;
const streamLength: usize = 10;

fn benchmark_arcfour_key_setup(state: &mut [u8; 256], key: [u8; keyLength], length: usize) -> i32
{
    let mut i: usize;
    let mut j: usize;
    let mut t: u8;

    i = 0;
    while (i < 256)
    {
        state[i] = i as u8;
        i += 1;
    }

    i = 0;
    j = 0;
    while (i < 256)
    {
        j = (j + state[i] as usize + key[i % length] as usize) % 256;
        t = state[i];
        state[i] = state[j];
        state[j] = t;

        i += 1;
    }

    return i as i32;
}

fn benchmark_arcfour_generate_stream(state: &mut [u8; 256], buffer: &mut [u8; 64], length: usize) -> i32
{
    let mut i: usize = 0;
    let mut j: usize = 0;
    let mut idx: usize = 0;
    let mut t: u8;

    while (idx < length)
    {
        i = (i + 1) % 256;
        j = (j + state[i] as usize) % 256;
        t = state[i];
        state[i] = state[j];
        state[j] = t;
        buffer[idx] = state[(state[i] as usize + state[j] as usize) % 256];

        idx += 1;
    }

    return (i as i32);
}

pub(crate) fn benchmark_arcfour_main(iterations: u32) -> i32
{
    let mut state: [u8; 256] = [0; 256];
    let mut buffer: [u8; 64] = [0; 64];
    let mut key: [u8; keyLength] = [0; keyLength];
    let mut stream: [u8; streamLength] = [0; streamLength];

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

    let mut idx: i32 = 0;

    let mut i: u32 = 0;
    while (i < iterations)
    {
        idx = benchmark_arcfour_key_setup(&mut state, key, keyLength);
        idx = benchmark_arcfour_generate_stream(&mut state, &mut buffer, streamLength);

        i += 1;
    }

    return idx;
}
