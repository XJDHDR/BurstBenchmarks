

mod PixarRaytracer;
mod FirefliesFlocking;

// Mandelbrot
#[no_mangle]
pub extern "C" fn benchmark_mandelbrot(width: u32, height: u32, iterations: u32) -> f32
{
    let mut data: f32 = 0.0;

    let mut i: u32 = 0;
    while (i < iterations)
    {
        let left: f32 = -2.1;
        let right: f32 = 1.0;
        let top: f32 = -1.3;
        let bottom: f32 = 1.3;
        let deltaX: f32 = (right - left) / width as f32;
        let deltaY: f32 = (bottom - top) / height as f32;
        let mut coordinateX: f32 = left;

        let mut x: u32 = 0;
        while (x < width)
        {
            let mut coordinateY: f32 = top;

            let mut y: u32 = 0;
            while (y < height)
            {
                let mut workX: f32 = 0.0;
                let mut workY: f32 = 0.0;
                let mut counter: i32 = 0;

                while (counter < 255 && f32::sqrt((workX * workX) + (workY * workY)) < 2.0)
                {
                    counter += 1;

                    let newX: f32 = (workX * workX) - (workY * workY) + coordinateX;

                    workY = 2.0 * workX * workY + coordinateY;
                    workX = newX;
                }

                data = workX + workY;
                coordinateY += deltaY;

                y += 1;
            }

            coordinateX += deltaX;

            x += 1;
        }

        i += 1;
    }

    return data;
}

// Pixar Raytracer
#[no_mangle]
pub extern "C" fn benchmark_pixar_raytracer(width: u32, height: u32, samples: u32) -> f32
{
    return PixarRaytracer::benchmark_pixar_raytracer_main(width, height, samples);
}

// Fireflies Flocking
#[no_mangle]
pub extern "C" fn benchmark_fireflies_flocking(boidsInput: u32, lifetimeInput: u32) -> f32
{
    let boids: usize = boidsInput as usize;
    let lifetime: usize = lifetimeInput as usize;
    return FirefliesFlocking::benchmark_fireflies_flocking_main(boids, lifetime);
}

// Polynomials
#[no_mangle]
pub extern "C" fn benchmark_polynomials(iterations: u32) -> f32
{
    let x: f32 = 0.2;

    let mut pu: f32 = 0.0;
    let mut poly: [f32; 100] = [0.0; 100];

    let mut i: u32 = 0;
    while (i < iterations)
    {
        let mut mu: f32 = 10.0;
        let mut s: f32;
        let mut j: usize = 0;

        while (j < 100)
        {
            mu = (mu + 2.0) / 2.0;
            poly[j] = mu;
            j += 1;
        }

        s = 0.0;

        j = 0;
        while (j < 100)
        {
            s = x * s + poly[j];
            j += 1;
        }

        pu += s;
        i += 1;
    }

    return pu;
}

// Particle Kinematics
#[derive(Clone, Copy)]
struct Particle
{
    x: f32,
    y: f32,
    z: f32,
    vx: f32,
    vy: f32,
    vz: f32
}

#[no_mangle]
pub extern "C" fn benchmark_particle_kinematics(quantityInput: u32, iterations: u32) -> f32
{
    let quantity: usize = quantityInput as usize;
    let mut particles: Vec<Particle> = Vec::with_capacity(quantity);

    let mut i: usize = 0;
    while (i < quantity)
    {
        let particle: Particle = Particle {
            x: i as f32,
            y: (i + 1) as f32,
            z: (i + 2) as f32,
            vx: 1.0,
            vy: 2.0,
            vz: 3.0,
        };
        particles.push(particle);

        i += 1;
    }

    let mut a: u32 = 0;
    while (a < iterations)
    {
        let mut b: usize = 0;
        let c: usize = quantity;
        while (b < c)
        {
            let p: &mut Particle = &mut particles[b];

            p.x += p.vx;
            p.y += p.vy;
            p.z += p.vz;

            b += 1;
        }

        a += 1;
    }

    let particle: Particle = particles[0];

    return particle.x + particle.y + particle.z;
}
