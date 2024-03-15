use std::fs::{File, OpenOptions};
use std::io::Write;
use std::ops::Rem;

const arrayLength: usize = 128;

fn benchmark_radix_find_largest(array: [i32; arrayLength]) -> i32
{
	let mut i: usize;
	let mut largest: i32 = -1;

	i = 0;
	while (i < arrayLength)
	{
		if (array[i] > largest)
		{
			largest = array[i];
		}

		i += 1;
	}

	return largest;
}

fn benchmark_radix_random(classicRandom: &mut u32) -> i32
{
	*classicRandom = *classicRandom * 6253729 + 4396403;

	return (classicRandom.rem(32767)) as i32;
}

fn benchmark_radix_sort(array: &mut [i32; arrayLength], mut printDone: bool)
{
	let mut i: usize;
	let mut semiSorted: [i32; arrayLength] = [0; arrayLength];
	let mut significantDigit: i32 = 1;
	let largest: i32 = benchmark_radix_find_largest(*array);

	let mut loops: i32 = 0;
	while (largest / significantDigit > 0)
	{
		loops += 1;
		let mut bucket: [i32; 10] = [0; 10];

		i = 0;
		while (i < arrayLength)
		{
			bucket[((array[i] / significantDigit) % 10) as usize] += 1;

			i += 1;
		}

		i = 1;
		while (i < 10)
		{
			bucket[i] += bucket[i - 1];
			i += 1;
		}

		//let mut values: String = String::new();
		let mut values: Vec<u8> = Vec::with_capacity(200);
		let mut jInt: i32 = (arrayLength - 1) as i32;
		while (jInt >= 0)
		{
			let j: usize = jInt as usize;

			let bucketIndex: usize = ((array[j] / significantDigit) as usize) % 10;
			let semiSortedIndex: usize = (bucket[bucketIndex] as usize - 1) % arrayLength;
			semiSorted[semiSortedIndex] = array[j];

			jInt -= 1;
		}

		// The error is happening in one of these two blocks.

		i = 0;
		while (i < arrayLength)
		{
			array[i] = semiSorted[i];

			if (printDone && loops <= 2) {
				values.extend_from_slice("Loop ".as_bytes());
				values.extend_from_slice(loops.to_string().as_bytes());
				values.extend_from_slice(", Index ".as_bytes());
				values.extend_from_slice(i.to_string().as_bytes());
				values.extend_from_slice(" = ".as_bytes());
				values.extend_from_slice(array[i].to_string().as_bytes());
				values.extend_from_slice("\r\n".as_bytes());
			}

			i += 1;
		}

		if (printDone && loops <= 2) {
			let mut output = OpenOptions::new()
				.write(true)
				.append(true)
				.create(true)
				.open("./benchmark_output_rust.txt")
				.unwrap();
			let _ = output.write_all(values.as_slice());
		}

		significantDigit *= 10;
	}
}

pub(crate) fn benchmark_radix_main(iterations: u32) -> i32
{
	let mut classicRandom: u32 = 7525;

	let mut array: [i32; arrayLength] = [0; arrayLength];

	let mut a: u32 = 0;
	while (a < iterations)
	{
		let mut b: usize = 0;
		while (b < arrayLength)
		{
			array[b] = benchmark_radix_random(&mut classicRandom);

			b += 1;
		}

		benchmark_radix_sort(&mut array, (a == 0));

		a += 1;
	}

	let head: i32 = array[0];

	return head;
}
