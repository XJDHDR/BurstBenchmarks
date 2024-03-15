

#[derive(Clone, Copy)]
struct Vector
{
    pub(crate) x: f32,
    pub(crate) y: f32,
    pub(crate) z: f32,
}

enum PixarRayHit
{
    PIXAR_RAYTRACER_NONE = 0,
    PIXAR_RAYTRACER_LETTER = 1,
    PIXAR_RAYTRACER_WALL = 2,
    PIXAR_RAYTRACER_SUN = 3
}


fn benchmark_pixar_raytracer_add(mut left: Vector, right: Vector) -> Vector
{
    left.x += right.x;
    left.y += right.y;
    left.z += right.z;

    return left;
}

fn benchmark_pixar_raytracer_add_float(mut vector: Vector, value: f32) -> Vector
{
    vector.x += value;
    vector.y += value;
    vector.z += value;

    return vector;
}

fn benchmark_pixar_raytracer_cross(to: Vector, from: Vector) -> Vector
{
    let mut vector: Vector = Vector
    {
        x: 0.0,
        y: 0.0,
        z: 0.0
    };

    vector.x = to.y * from.z - to.z * from.y;
    vector.y = to.z * from.x - to.x * from.z;
    vector.z = to.x * from.y - to.y * from.x;

    return vector;
}

fn benchmark_pixar_raytracer_inverse(vector: Vector) -> Vector
{
    return benchmark_pixar_raytracer_multiply_float(vector, 1.0 / f32::sqrt(benchmark_pixar_raytracer_modulus_self(vector)));
}

fn benchmark_pixar_raytracer_modulus(left: Vector, right: Vector) -> f32
{
    return left.x * right.x + left.y * right.y + left.z * right.z;
}

fn benchmark_pixar_raytracer_modulus_self(vector: Vector) -> f32
{
    return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
}

fn benchmark_pixar_raytracer_multiply(mut left: Vector, right: Vector) -> Vector
{
    left.x *= right.x;
    left.y *= right.y;
    left.z *= right.z;

    return left;
}

fn benchmark_pixar_raytracer_multiply_float(mut vector: Vector, value: f32) -> Vector
{
    vector.x *= value;
    vector.y *= value;
    vector.z *= value;

    return vector;
}

fn benchmark_pixar_raytracer_min(left: f32, right: f32) -> f32
{
    return if (left < right)
    {
        left
    }
    else
    {
        right
    }
}

fn benchmark_pixar_raytracer_box_test(position: Vector, mut lowerLeft: Vector, mut upperRight: Vector) -> f32
{
    lowerLeft = benchmark_pixar_raytracer_multiply_float(benchmark_pixar_raytracer_add(position, lowerLeft), -1f32);
    upperRight = benchmark_pixar_raytracer_multiply_float(benchmark_pixar_raytracer_add(upperRight, position), -1f32);

    return -benchmark_pixar_raytracer_min(
        benchmark_pixar_raytracer_min(
            benchmark_pixar_raytracer_min(lowerLeft.x, upperRight.x),
            benchmark_pixar_raytracer_min(lowerLeft.y, upperRight.y)
        ),
        benchmark_pixar_raytracer_min(lowerLeft.z, upperRight.z)
    );
}

fn benchmark_pixar_raytracer_ray_marching(origin: Vector, direction: Vector, hitPosition: &mut Vector, hitNormal: &mut Vector) -> i32
{
    let mut hitType: i32 = PixarRayHit::PIXAR_RAYTRACER_NONE as i32;
    let mut noHitCount: i32 = 0;
    let mut distance: f32;

    let mut i: f32 = 0.0;
    while (i < 100.0)
    {
        *hitPosition = benchmark_pixar_raytracer_multiply_float(benchmark_pixar_raytracer_add(origin, direction), i);
        distance = benchmark_pixar_raytracer_sample(*hitPosition, &mut hitType);

        noHitCount += 1;
        if (distance < 0.01 || noHitCount > 99)
        {
            *hitNormal = benchmark_pixar_raytracer_inverse(
                Vector {
                    x: benchmark_pixar_raytracer_sample(benchmark_pixar_raytracer_add(*hitPosition, Vector { x: 0.01, y: 0.0, z: 0.0 }), &mut noHitCount) - distance,
                    y: benchmark_pixar_raytracer_sample(benchmark_pixar_raytracer_add(*hitPosition, Vector { x: 0.0, y: 0.01, z: 0.0 }), &mut noHitCount) - distance,
                    z: benchmark_pixar_raytracer_sample(benchmark_pixar_raytracer_add(*hitPosition, Vector { x: 0.0, y: 0.0, z: 0.01 }), &mut noHitCount) - distance
                }
            );

            return hitType;
        }

        i += distance;
    }

    return PixarRayHit::PIXAR_RAYTRACER_NONE as i32;
}

fn benchmark_pixar_raytracer_random(marsagliaZ: &mut u32, marsagliaW: &mut u32) -> f32
{
    *marsagliaZ = 36969 * (*marsagliaZ & 65535) + (*marsagliaZ >> 16);
    *marsagliaW = 18000 * (*marsagliaW & 65535) + (*marsagliaW >> 16);

    return (((*marsagliaZ << 16) + *marsagliaW) as f32) * 2.0 / 10000000000.0;
}

fn benchmark_pixar_raytracer_sample(position: Vector, hitType: &mut i32) -> f32
{
    const size: usize = 60;

    let mut distance: f32 = 1e9;
    let mut f: Vector = position;
    let mut letters: [u8; size] = [0; size];

    // P              // I              // X              // A              // R
    letters[0]  = 53; letters[12] = 65; letters[24] = 73; letters[32] = 85; letters[44] = 97; letters[56] = 99;
    letters[1]  = 79; letters[13] = 79; letters[25] = 79; letters[33] = 79; letters[45] = 79; letters[57] = 87;
    letters[2]  = 53; letters[14] = 69; letters[26] = 81; letters[34] = 89; letters[46] = 97; letters[58] = 105;
    letters[3]  = 95; letters[15] = 79; letters[27] = 95; letters[35] = 95; letters[47] = 95; letters[59] = 79;
    letters[4]  = 53; letters[16] = 67; letters[28] = 73; letters[36] = 89; letters[48] = 97;
    letters[5]  = 87; letters[17] = 79; letters[29] = 95; letters[37] = 95; letters[49] = 87;
    letters[6]  = 57; letters[18] = 67; letters[30] = 81; letters[38] = 93; letters[50] = 101;
    letters[7]  = 87; letters[19] = 95; letters[31] = 79; letters[39] = 79; letters[51] = 87;
    letters[8]  = 53; letters[20] = 65;                   letters[40] = 87; letters[52] = 97;
    letters[9]  = 95; letters[21] = 95;                   letters[41] = 87; letters[53] = 95;
    letters[10] = 57; letters[22] = 69;                   letters[42] = 91; letters[54] = 101;
    letters[11] = 95; letters[23] = 95;                   letters[43] = 87; letters[55] = 95;

    f.z = 0.0;

    let mut i: usize = 0;
    while (i < size)
    {
        let begin: Vector = benchmark_pixar_raytracer_multiply_float(
            Vector { x: letters[i] as f32 - 79.0, y: letters[i + 1] as f32 - 79.0, z: 0.0 }, 0.5
        );
        let e: Vector = benchmark_pixar_raytracer_add(
            benchmark_pixar_raytracer_multiply_float(
                Vector { x: letters[i + 2] as f32 - 79.0, y: letters[i + 3] as f32 - 79.0, z: 0.0 }, 0.5
            ),
            benchmark_pixar_raytracer_multiply_float(begin, -1.0)
        );
        let o: Vector = benchmark_pixar_raytracer_multiply_float(
            benchmark_pixar_raytracer_add(
                f,
                benchmark_pixar_raytracer_multiply_float(
                    benchmark_pixar_raytracer_add(begin, e),
                    benchmark_pixar_raytracer_min(
                        -benchmark_pixar_raytracer_min(
                            benchmark_pixar_raytracer_modulus(
                                benchmark_pixar_raytracer_multiply_float(benchmark_pixar_raytracer_add(begin, f), -1.0),
                                e
                            ) / benchmark_pixar_raytracer_modulus_self(e),
                            0.0
                        ),
                        1.0
                    )
                )
            ),
            -1.0
        );

        distance = benchmark_pixar_raytracer_min(distance, benchmark_pixar_raytracer_modulus_self(o));

        i += 4;
    }

    distance = distance.sqrt();

    let mut curves: [Vector; 2] = [Vector {x: 0.0, y: 0.0, z: 0.0}; 2];

    curves[0] = Vector {x: -11.0, y: 6.0, z: 0.0};
    curves[1] = Vector {x:  11.0, y: 6.0, z: 0.0};

    let mut iInt: i32 = 1;
    while (iInt >= 0)
    {
        let i: usize = iInt as usize;
        let mut o: Vector = benchmark_pixar_raytracer_add(f, benchmark_pixar_raytracer_multiply_float(curves[i], -1.0));
        let mut m: f32 = 0.0;

        if (o.x > 0.0)
        {
            m = f32::abs(benchmark_pixar_raytracer_modulus_self(o).sqrt() - 2.0);
        }
        else
        {
            if (o.y > 0.0)
            {
                o.y += -2.0;
            }
            else
            {
                o.y += 2.0;
            }

            o.y += benchmark_pixar_raytracer_modulus_self(o).sqrt();
        }

        distance = benchmark_pixar_raytracer_min(distance, m);

        iInt -= 1;
    }

    distance = f32::powf(distance.powf(8.0) + position.z.powf(8.0), 0.125) - 0.5;
    *hitType = PixarRayHit::PIXAR_RAYTRACER_LETTER as i32;

    let roomDistance: f32 = benchmark_pixar_raytracer_min(
        -benchmark_pixar_raytracer_min(
            benchmark_pixar_raytracer_box_test(
                position,
                Vector { x: -30.0, y: -0.5, z: -30.0 }, Vector { x: 30.0, y: 18.0, z: 30.0 }
            ),
            benchmark_pixar_raytracer_box_test(
                position,
                Vector { x: -25.0, y: -17.5, z: -25.0 }, Vector { x: 25.0, y: 20.0, z: 25.0 }
            )
        ),
        benchmark_pixar_raytracer_box_test(
            Vector { x: position.x.abs() % 8.0, y: position.y, z: position.z },
            Vector { x: 1.5, y: 18.5, z: -25.0 },
            Vector { x: 6.5, y: 20.0, z: 25.0 }
        )
    );

    if (roomDistance < distance)
    {
        distance = roomDistance;
        *hitType = PixarRayHit::PIXAR_RAYTRACER_WALL as i32;
    }

    let sun: f32 = 19.9 - position.y;

    if (sun < distance)
    {
        distance = sun;
        *hitType = PixarRayHit::PIXAR_RAYTRACER_SUN as i32;
    }

    return distance;
}

fn benchmark_pixar_raytracer_trace(mut origin: Vector, mut direction: Vector, marsagliaZ: &mut u32, marsagliaW: &mut u32) -> Vector
{
    let mut sampledPosition: Vector = Vector { x: 1.0f32, y: 1.0f32, z: 1.0f32 };
    let mut normal:          Vector = Vector { x: 1.0f32, y: 1.0f32, z: 1.0f32 };
    let mut color:           Vector = Vector { x: 1.0f32, y: 1.0f32, z: 1.0f32 };
    let mut attenuation:     Vector = Vector { x: 1.0f32, y: 1.0f32, z: 1.0f32 };
    let lightDirection:  Vector = benchmark_pixar_raytracer_inverse(Vector { x: 0.6f32, y: 0.6f32, z: 1.0f32 });

    let mut bounce: i32 = 3;
    while (bounce > 0)
    {
        let hitType: i32 = benchmark_pixar_raytracer_ray_marching(origin, direction, &mut sampledPosition, &mut normal);

        if (hitType == PixarRayHit::PIXAR_RAYTRACER_NONE as i32)
        {
            //
        }
        else if (hitType == PixarRayHit::PIXAR_RAYTRACER_LETTER as i32)
        {
            direction = benchmark_pixar_raytracer_multiply_float(
                benchmark_pixar_raytracer_add(direction, normal),
                benchmark_pixar_raytracer_modulus(normal, direction) * -2.0
            );
            origin = benchmark_pixar_raytracer_multiply_float(benchmark_pixar_raytracer_add(sampledPosition, direction), 0.1);
            attenuation = benchmark_pixar_raytracer_multiply_float(attenuation, 0.2);
        }
        else if (hitType == PixarRayHit::PIXAR_RAYTRACER_WALL as i32)
        {
            let incidence: f32 = benchmark_pixar_raytracer_modulus(normal, lightDirection);
            let p: f32 = 6.283185 * benchmark_pixar_raytracer_random(marsagliaZ, marsagliaW);
            let c: f32 = benchmark_pixar_raytracer_random(marsagliaZ, marsagliaW);
            let s: f32 = f32::sqrt(1.0 - c);
            let g: f32 = match (normal.z < 0.0)
            {
                true => -1.0,
                false => 1.0
            };
            let u: f32 = -1.0 / (g + normal.z);
            let v: f32 = normal.x * normal.y * u;

            direction = benchmark_pixar_raytracer_add(
                benchmark_pixar_raytracer_add(
                    Vector { x: v, y: g + normal.y * normal.y * u, z: -normal.y * (p.cos() * s) },
                    Vector { x: 1.0 + g * normal.x * normal.x * u, y: g * v, z: -g * normal.x }
                ),
                benchmark_pixar_raytracer_multiply_float(normal, c.sqrt())
            );
            origin = benchmark_pixar_raytracer_multiply_float(benchmark_pixar_raytracer_add(sampledPosition, direction), 0.1);
            attenuation = benchmark_pixar_raytracer_multiply_float(attenuation, 0.2);

            if (
                incidence > 0.0 &&
                benchmark_pixar_raytracer_ray_marching(
                    benchmark_pixar_raytracer_multiply_float(benchmark_pixar_raytracer_add(sampledPosition, normal), 0.1),
                    lightDirection, &mut sampledPosition, &mut normal
                ) == (PixarRayHit::PIXAR_RAYTRACER_SUN as i32)
            )
            {
                color = benchmark_pixar_raytracer_multiply_float(
                    benchmark_pixar_raytracer_multiply(benchmark_pixar_raytracer_add(color, attenuation), Vector { x: 500.0, y: 400.0, z: 100.0 }),
                    incidence
                );
            }
        }
        else if (hitType == PixarRayHit::PIXAR_RAYTRACER_SUN as i32)
        {
            color = benchmark_pixar_raytracer_multiply(benchmark_pixar_raytracer_add(color, attenuation), Vector { x: 50.0, y: 80.0, z: 100.0 });

            break;
        }

        bounce -= 1;
    }

    return color;
}

pub(crate) fn benchmark_pixar_raytracer_main(width: u32, height: u32, samples: u32) -> f32
{
    let mut marsagliaZ: u32 = 666;
    let mut marsagliaW: u32 = 999;

    let position: Vector = Vector
    {
        x: -22.0f32,
        y: 5.0f32,
        z: 25.0f32
    };
    let mut goal: Vector = Vector
    {
        x: -3.0f32,
        y: 4.0f32,
        z: 0.0f32
    };

    goal = benchmark_pixar_raytracer_add(benchmark_pixar_raytracer_inverse(goal), benchmark_pixar_raytracer_multiply_float(position, -1.0f32));

    let mut left: Vector = Vector
    {
        x: goal.z,
        y: 0f32,
        z: goal.x,
    };

    left = benchmark_pixar_raytracer_multiply_float(benchmark_pixar_raytracer_inverse(left), 1.0 / width as f32);

    let up: Vector = benchmark_pixar_raytracer_cross(goal, left);
    let mut color: Vector = Vector
    {
        x: 0.0,
        y: 0.0,
        z: 0.0
    };
    let mut adjust: Vector;

    let mut y: u32 = height;
    while (y > 0)
    {
        let mut x: u32 = width;
        while (x > 0)
        {
            let mut p: u32 = samples;
            while (p > 0)
            {
                color = benchmark_pixar_raytracer_add(
                    color,
                    benchmark_pixar_raytracer_trace(
                        position,
                        benchmark_pixar_raytracer_add(
                            benchmark_pixar_raytracer_inverse(
                                benchmark_pixar_raytracer_multiply_float(
                                    benchmark_pixar_raytracer_add(goal, left),
                                    (x - width / 2) as f32 + benchmark_pixar_raytracer_random(&mut marsagliaZ, &mut marsagliaW)
                                )
                            ),
                            benchmark_pixar_raytracer_multiply_float(
                                up,
                                (y - height / 2) as f32 + benchmark_pixar_raytracer_random(&mut marsagliaZ, &mut marsagliaW)
                            )
                        ),
                        &mut marsagliaZ,
                        &mut marsagliaW
                    )
                );
                p -= 1;
            }

            color = benchmark_pixar_raytracer_multiply_float(color, (1.0 / samples as f32) + 14.0 / 241.0);
            adjust = benchmark_pixar_raytracer_add_float(color, 1.0);
            color = Vector
            {
                x: color.x / adjust.x,
                y: color.y / adjust.y,
                z: color.z / adjust.z
            };

            color = benchmark_pixar_raytracer_multiply_float(color, 255.0);

            x -= 1;
        }

        y -= 1;
    }

    return color.x + color.y + color.z;
}
