
mod nbody;

use crate::f64_benchmarks::nbody::
    {
        NBody,
        benchmark_nbody_initialize_bodies,
        benchmark_nbody_energy,
        benchmark_nbody_advance
    };

// NBody
#[no_mangle]
pub extern "C" fn benchmark_nbody(mut advancements: u32) -> f64
{
    let mut sun: [NBody; 5] = [
        NBody { ..Default::default() },
        NBody { ..Default::default() },
        NBody { ..Default::default() },
        NBody { ..Default::default() },
        NBody { ..Default::default() },
    ];

    benchmark_nbody_initialize_bodies(&mut sun);
    benchmark_nbody_energy(&sun);

    while (advancements > 0)
    {
        benchmark_nbody_advance(& mut sun, 0.01);
        advancements -= 1;
    }

    benchmark_nbody_energy(&sun);

    return sun[0].x + sun[0].y;
}
