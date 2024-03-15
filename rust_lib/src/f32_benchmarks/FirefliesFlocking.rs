

#[derive(Clone, Copy)]
struct Vector
{
    x: f32,
    y: f32,
    z: f32,
}

#[derive(Clone, Copy)]
struct Boid {
    position: Vector,
    velocity: Vector,
    acceleration: Vector,
}


fn benchmark_fireflies_flocking_add(left: &mut Vector, right: &mut Vector)
{
    left.x += right.x;
    left.y += right.y;
    left.z += right.z;
}

fn benchmark_fireflies_flocking_divide(vector: &mut Vector, value: f32)
{
    vector.x /= value;
    vector.y /= value;
    vector.z /= value;
}

fn benchmark_fireflies_flocking_length(vector: Vector) -> f32
{
    return f32::sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
}

fn benchmark_fireflies_flocking_multiply(vector: &mut Vector, value: f32)
{
    vector.x *= value;
    vector.y *= value;
    vector.z *= value;
}

fn benchmark_fireflies_flocking_normalize(vector: &mut Vector)
{
    let length: f32 = f32::sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);

    vector.x /= length;
    vector.y /= length;
    vector.z /= length;
}

fn benchmark_fireflies_flocking_random(parkMiller: &mut u32) -> f32
{
    *parkMiller = (((*parkMiller as u64) * 48271) % 0x7fffffff) as u32;

    return *parkMiller as f32 / 10000000.0;
}

fn benchmark_fireflies_flocking_subtract(left: &mut Vector, right: Vector)
{
    left.x -= right.x;
    left.y -= right.y;
    left.z -= right.z;
}

pub(crate) fn benchmark_fireflies_flocking_main(boids: usize, lifetime: usize) -> f32
{
    let mut i: usize;
    let mut parkMiller: u32 = 666;
    let maxSpeed: f32 = 1.0;
    let maxForce: f32 = 0.03;
    let separationDistance: f32 = 15.0;
    let neighbourDistance: f32 = 30.0;

    let mut fireflies: Vec<Boid> = Vec::with_capacity(boids);

    i = 0;
    while (i < boids)
    {
        let firefly: Boid = Boid {
            position: Vector {
                x: benchmark_fireflies_flocking_random(&mut parkMiller),
                y: benchmark_fireflies_flocking_random(&mut parkMiller),
                z: benchmark_fireflies_flocking_random(&mut parkMiller)
            },
            velocity: Vector {
                x: benchmark_fireflies_flocking_random(&mut parkMiller),
                y: benchmark_fireflies_flocking_random(&mut parkMiller),
                z: benchmark_fireflies_flocking_random(&mut parkMiller)
            },
            acceleration: Vector {
                x: 0.0,
                y: 0.0,
                z: 0.0,
            },
        };
        fireflies.push(firefly);

        i += 1;
    }

    i = 0;
    while (i < lifetime)
    {
        // Update
        let mut boid: usize = 0;
        while (boid < boids)
        {
            let mut fireflyVelocity: Vector = fireflies[boid].velocity;
            let mut fireflyAcceleration: Vector = fireflies[boid].acceleration;
            benchmark_fireflies_flocking_add(&mut fireflyVelocity, &mut fireflyAcceleration);
            fireflies[boid].velocity = fireflyVelocity;
            fireflies[boid].acceleration = fireflyAcceleration;

            let speed: f32 = benchmark_fireflies_flocking_length(fireflies[boid].velocity);

            if (speed > maxSpeed) {
                benchmark_fireflies_flocking_divide(&mut fireflies[boid].velocity, speed);
                benchmark_fireflies_flocking_multiply(&mut fireflies[boid].velocity, maxSpeed);
            }

            fireflyVelocity = fireflies[boid].velocity;
            let mut fireflyPosition: Vector = fireflies[boid].position;
            benchmark_fireflies_flocking_add(&mut fireflyPosition, &mut fireflyVelocity);
            fireflies[boid].velocity = fireflyVelocity;
            fireflies[boid].position = fireflyPosition;

            benchmark_fireflies_flocking_multiply(&mut fireflies[boid].acceleration, maxSpeed);

            boid += 1;
        }

        // Separation
        boid = 0;
        while (boid < boids)
        {
            let mut separation: Vector = Vector { x: 0.0, y: 0.0, z: 0.0 };
            let mut count: i32 = 0;

            let mut target: usize = 0;
            while (target < boids)
            {
                let mut position: Vector = fireflies[boid].position;

                benchmark_fireflies_flocking_subtract(&mut position, fireflies[target].position);

                let distance: f32 = benchmark_fireflies_flocking_length(position);

                if (distance > 0.0 && distance < separationDistance)
                {
                    benchmark_fireflies_flocking_normalize(&mut position);
                    benchmark_fireflies_flocking_divide(&mut position, distance);

                    separation = position;
                    count += 1;
                }

                target += 1;
            }

            if (count > 0)
            {
                benchmark_fireflies_flocking_divide(&mut separation, count as f32);
                benchmark_fireflies_flocking_normalize(&mut separation);
                benchmark_fireflies_flocking_multiply(&mut separation, maxSpeed);
                benchmark_fireflies_flocking_subtract(&mut separation, fireflies[boid].velocity);

                let force: f32 = benchmark_fireflies_flocking_length(separation);

                if (force > maxForce) {
                    benchmark_fireflies_flocking_divide(&mut separation, force);
                    benchmark_fireflies_flocking_multiply(&mut separation, maxForce);
                }

                benchmark_fireflies_flocking_multiply(&mut separation, 1.5);
                benchmark_fireflies_flocking_add(&mut fireflies[boid].acceleration, &mut separation);
            }

            boid += 1;
        }

        // Cohesion
        boid = 0;
        while (boid < boids)
        {
            let mut cohesion: Vector = Vector { x: 0.0, y: 0.0, z: 0.0 };
            let mut count: i32 = 0;

            let mut target: usize = 0;
            while (target < boids)
            {
                let mut position: Vector = fireflies[boid].position;

                benchmark_fireflies_flocking_subtract(&mut position, fireflies[target].position);

                let distance: f32 = benchmark_fireflies_flocking_length(position);

                if (distance > 0.0 && distance < neighbourDistance)
                {
                    cohesion = fireflies[boid].position;
                    count += 1;
                }

                target += 1;
            }

            if (count > 0)
            {
                benchmark_fireflies_flocking_divide(&mut cohesion, count as f32);
                benchmark_fireflies_flocking_subtract(&mut cohesion, fireflies[boid].position);
                benchmark_fireflies_flocking_normalize(&mut cohesion);
                benchmark_fireflies_flocking_multiply(&mut cohesion, maxSpeed);
                benchmark_fireflies_flocking_subtract(&mut cohesion, fireflies[boid].velocity);

                let force: f32 = benchmark_fireflies_flocking_length(cohesion);

                if (force > maxForce)
                {
                    benchmark_fireflies_flocking_divide(&mut cohesion, force);
                    benchmark_fireflies_flocking_multiply(&mut cohesion, maxForce);
                }

                benchmark_fireflies_flocking_add(&mut fireflies[boid].acceleration, &mut cohesion);
            }

            boid += 1;
        }

        i += 1;
    }

    return parkMiller as f32;
}
