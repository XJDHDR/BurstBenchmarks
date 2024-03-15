


pub(crate) struct NBody
{
    pub(crate) x: f64,
    pub(crate) y: f64,
    pub(crate) z: f64,
    pub(crate) vx: f64,
    pub(crate) vy: f64,
    pub(crate) vz: f64,
    pub(crate) mass: f64
}

impl Default for NBody
{
    fn default() -> Self
    {
        NBody
        {
            x: 0f64,
            y: 0f64,
            z: 0f64,
            vx: 0f64,
            vy: 0f64,
            vz: 0f64,
            mass: 0f64
        }
    }
}


pub(crate) fn benchmark_nbody_initialize_bodies(sun: &mut [NBody; 5])
{
    const pi: f64 = std::f64::consts::PI;
    const solarMass: f64 = 4.0 * pi * pi;
    const daysPerYear: f64 = 365.24;

    sun[1] = NBody  // Jupiter
    {
        x: 4.84143144246472090e+00,
        y: -1.16032004402742839e+00,
        z: -1.03622044471123109e-01,
        vx: 1.66007664274403694e-03 * daysPerYear,
        vy: 7.69901118419740425e-03 * daysPerYear,
        vz: -6.90460016972063023e-05 * daysPerYear,
        mass: 9.54791938424326609e-04 * solarMass
    };

    sun[2] = NBody  // Saturn
    {
        x: 8.34336671824457987e+00,
        y: 4.12479856412430479e+00,
        z: -4.03523417114321381e-01,
        vx: -2.76742510726862411e-03 * daysPerYear,
        vy: 4.99852801234917238e-03 * daysPerYear,
        vz: 2.30417297573763929e-05 * daysPerYear,
        mass: 2.85885980666130812e-04 * solarMass
    };

    sun[3] = NBody  // Uranus
    {
        x: 1.28943695621391310e+01,
        y: -1.51111514016986312e+01,
        z: -2.23307578892655734e-01,
        vx: 2.96460137564761618e-03 * daysPerYear,
        vy: 2.37847173959480950e-03 * daysPerYear,
        vz: -2.96589568540237556e-05 * daysPerYear,
        mass: 4.36624404335156298e-05 * solarMass
    };

    sun[4] = NBody  // Neptune
    {
        x: 1.53796971148509165e+01,
        y: -2.59193146099879641e+01,
        z: 1.79258772950371181e-01,
        vx: 2.68067772490389322e-03 * daysPerYear,
        vy: 1.62824170038242295e-03 * daysPerYear,
        vz: -9.51592254519715870e-05 * daysPerYear,
        mass: 5.15138902046611451e-05 * solarMass
    };

    let mut vx: f64 = 0f64;
    let mut vy: f64 = 0f64;
    let mut vz: f64 = 0f64;

    for i in 1..sun.len()
    {
        let mass: f64 = sun[i].mass;

        vx += sun[i].vx * mass;
        vy += sun[i].vy * mass;
        vz += sun[i].vz * mass;
    }

    sun[0].mass = solarMass;
    sun[0].vx = vx / -solarMass;
    sun[0].vy = vy / -solarMass;
    sun[0].vz = vz / -solarMass;
}

pub(crate) fn benchmark_nbody_energy(sun: &[NBody; 5])
{
    let mut e: f64 = 0.0;

    for bi in 0..sun.len()
    {
        let imass: f64 = sun[bi].mass;
        let ix: f64 = sun[bi].x;
        let iy: f64 = sun[bi].y;
        let iz: f64 = sun[bi].z;
        let ivx: f64 = sun[bi].vx;
        let ivy: f64 = sun[bi].vy;
        let ivz: f64 = sun[bi].vz;

        e += 0.5 * imass * (ivx * ivx + ivy * ivy + ivz * ivz);

        for bj in (bi + 1)..sun.len() {
            let jmass: f64 = sun[bj].mass;
            let dx: f64 = ix - sun[bj].x;
            let dy: f64 = iy - sun[bj].y;
            let dz: f64 = iz - sun[bj].z;

            e -= imass * jmass / f64::sqrt(dx * dx + dy * dy + dz * dz);
        }
    }
}

pub(crate) fn benchmark_nbody_advance(sun: &mut [NBody; 5], distance: f64)
{
    for bi in 0..(sun.len() - 1)
    {
        let ix: f64 = sun[bi].x;
        let iy: f64 = sun[bi].y;
        let iz: f64 = sun[bi].z;
        let mut ivx: f64 = sun[bi].vx;
        let mut ivy: f64 = sun[bi].vy;
        let mut ivz: f64 = sun[bi].vz;
        let imass: f64 = sun[bi].mass;

        for bj in (bi + 1)..sun.len()
        {
            let dx: f64 = sun[bj].x - ix;
            let dy: f64 = sun[bj].y - iy;
            let dz: f64 = sun[bj].z - iz;
            let jmass: f64 = sun[bj].mass;
            let mag = distance / benchmark_nbody_get_d2(dx, dy, dz);

            sun[bj].vx = sun[bj].vx - dx * imass * mag;
            sun[bj].vy = sun[bj].vy - dy * imass * mag;
            sun[bj].vz = sun[bj].vz - dz * imass * mag;
            ivx = ivx + dx * jmass * mag;
            ivy = ivy + dy * jmass * mag;
            ivz = ivz + dz * jmass * mag;
        }

        sun[bi].vx = ivx;
        sun[bi].vy = ivy;
        sun[bi].vz = ivz;
        sun[bi].x = ix + ivx * distance;
        sun[bi].y = iy + ivy * distance;
        sun[bi].z = iz + ivz * distance;
    }

    sun[4].x = sun[4].x + sun[4].vx * distance;
    sun[4].y = sun[4].y + sun[4].vy * distance;
    sun[4].z = sun[4].z + sun[4].vz * distance;
}

pub(crate) fn benchmark_nbody_get_d2(dx: f64, dy: f64, dz: f64) -> f64
{
    let d2: f64 = dx * dx + dy * dy + dz * dz;

    return d2 * f64::sqrt(d2);
}
