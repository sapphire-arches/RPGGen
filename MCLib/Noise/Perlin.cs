using System;
using MCLib.Rand;

namespace MCLib.Noise
{
	//Some utilities that we need to make perlin noise. (The interpolation functions)
	class PerlinUtils {
		static double d; //Scratch var for cosInt function.
		const double RAD_MAX = 360 * (Math.PI/180); //Max value of a radian.
		
		public static double Lerp (double F1, double F2, double A)
		{
			//We use a Linear Interpolation
			return F1 * A + F2 * (1 - A);
		}
		
		public static double CosInt (double F1, double F2, double A)
		{
			d = Math.Cos (A * RAD_MAX);
			return Lerp (F1, F2, d); //WTH not? If it's to slow, we can replace it, but at this point we don't need to.
		}
	}
	
	public class Perlin1D
	{
		const int NUM_VALS = 256; //The number of values to use. Good number. Power of 2!
		//We use an array of doubles as a lookup table.
		double [] vals;
		
		
		public Perlin1D (uint seed)
		{
			vals = new double[NUM_VALS];
			Random r = new MersenneTwister (seed);
			for (int i = 0; i < vals.Length; ++i) 
				vals [i] = r.NextDouble ();
		}
		
		public double Get (double X, double XScale, double P, int O)
		{
			X *= XScale; //Get that out of the way.
			double total = 0.0;
			int frequency = 0;
			double amplitude = 0;
			for (int i = 0; i < O; ++i) {
				frequency = (int)Math.Pow (2, i);
				amplitude = Math.Pow (P, i+1);
				total += InterpolatedNoise (X * frequency) * amplitude;
			}
			return total;
		}
			
		private double InterpolatedNoise (double X)
		{
			int ix = (int)Math.Floor (X);
			double fx = X - ix;
			return PerlinUtils.Lerp (SmoothNoise (ix), SmoothNoise (ix + 1), fx);
		}
		
		private double SmoothNoise (int X)
		{
			X %= NUM_VALS;
			return vals[X] / 2 + ((X + 1 < NUM_VALS) ? vals[X + 1] / 4 : 0.25) + ((X - 1 > 0) ? vals [X - 1] / 4 : 0.25);
		}
	}
	
	public class Perlin2D {
		public const int NUM_VALS = 256;
		//We use an array of doubles as a lookup table,
		double [,] vals;
		//Optimization technique. We use n instead of declaring new doubles every time the function is called.
		//Gains are probably not significant, but WTH, it's not to painful/confusing so I say it's ok!
		double[] n;
		
		public Perlin2D (uint seed)
		{
			this.vals = new double[NUM_VALS, NUM_VALS];
			MersenneTwister mt = new MersenneTwister (seed);
			for (int x = 0; x < NUM_VALS; ++x) {
				for (int y = 0; y < NUM_VALS; ++y) {
					vals [x, y] = mt.NextDouble ();
				}
			}
			this.n = new double[4];
		}
		
		public double Get (double X, double XScale, double Y, double YScale, double P, int O)
		{
			X *= XScale; //Get that out of the way.
			Y *= YScale; //And this.
			double total = 0.0;
			int frequency = 0;
			double amplitude = 0;
			for (int i = 0; i < O; ++i) {
				frequency = (int)Math.Pow (2, i);
				amplitude = Math.Pow (P, i + 1);
				total += InterpolatedNoise (X * frequency, Y * frequency) * amplitude;
			}
			return total;
		}
		
		private double InterpolatedNoise (double X, double Y)
		{
			int ix = (int)Math.Floor (X);
			double fx = 1 - (X - ix);
			int iy = (int)Math.Floor (Y);
			double fy = 1 - (Y - iy);
			
			n [0] = SmoothNoise (ix, iy);
			n [1] = SmoothNoise (ix + 1, iy);
			n [2] = SmoothNoise (ix, iy + 1);
			n [3] = SmoothNoise (ix + 1, iy + 1);
			
			n [0] = PerlinUtils.Lerp (n [0], n [1], fx);
			n [1] = PerlinUtils.Lerp (n [2], n [3], fx);
			
			return PerlinUtils.Lerp (n [0], n [1], fy);
		}
		
		private double SmoothNoise (int X, int Y)
		{
			double corners = (Noise (X + 1, Y + 1) + Noise (X + 1, Y - 1) + Noise (X - 1, Y + 1) + Noise (X - 1, Y - 1)) / 16;
			double sides = (Noise (X - 1, Y) + Noise (X + 1, Y) + Noise (X, Y - 1) + Noise (X, Y + 1)) / 16;
			double center = Noise (X, Y) / 2;
			return corners + sides + center;
		}
		
		private double Noise (int X, int Y)
		{
			X %= NUM_VALS;
			Y %= NUM_VALS;
			if (X < 0 || X >= NUM_VALS || Y < 0 || Y >= NUM_VALS)
				return 1;
			return vals [X, Y];
		}
	}
	
	public class Perlin3D {
		public const int NUM_VALS = 256;
		//We use an array of doubles as a lookup table,
		double [,,] vals;
		//Optimization technique. We use n instead of declaring new doubles every time the function is called.
		//Gains are probably not significant, but WTH, it's not to painful/confusing so I say it's ok!
		double[] n;
		
		public Perlin3D (uint seed)
		{
			this.vals = new double[NUM_VALS, NUM_VALS, NUM_VALS];
			MersenneTwister mt = new MersenneTwister (seed);
			for (int x = 0; x < NUM_VALS; ++x) {
				for (int y = 0; y < NUM_VALS; ++y) {
					for (int z = 0; z < NUM_VALS; ++z) {
						vals [x, y, z] = mt.NextDouble ();
					}
				}
			}
			this.n = new double[8];
		}
		
		public double Get (double X, double XScale, double Y, double YScale, double Z, double ZScale, double P, int O)
		{
			X *= XScale; //Get that out of the way.
			Y *= YScale; //And this.
			Z *= ZScale; //This as well!
			double total = 0.0;
			int frequency = 0;
			double amplitude = 0;
			for (int i = 0; i < O; ++i) {
				frequency = (int)Math.Pow (2, i);
				amplitude = Math.Pow (P, i + 1);
				total += InterpolatedNoise (X * frequency, Y * frequency, Z * frequency) * amplitude;
			}
			return total;
		}
		
		private double InterpolatedNoise (double X, double Y, double Z)
		{
			int ix = (int)Math.Floor (X);
			double fx = 1 - (X - ix);
			int iy = (int)Math.Floor (Y);
			double fy = 1 - (Y - iy);
			int iz = (int)Math.Floor (Z);
			double fz = 1 - (Z - iz);
			
			n [0] = SmoothNoise (ix, iy, iz);
			n [1] = SmoothNoise (ix + 1, iy, iz);
			n [2] = SmoothNoise (ix, iy + 1, iz);
			n [3] = SmoothNoise (ix + 1, iy + 1, iz);
			n [4] = SmoothNoise (ix, iy, iz + 1);
			n [5] = SmoothNoise (ix + 1, iy, iz + 1);
			n [6] = SmoothNoise (ix, iy + 1, iz + 1);
			n [7] = SmoothNoise (ix + 1, iy + 1, iz + 1);
			
			n [0] = PerlinUtils.Lerp (n [0], n [4], fz);
			n [1] = PerlinUtils.Lerp (n [1], n [5], fz);
			n [2] = PerlinUtils.Lerp (n [2], n [6], fz);
			n [3] = PerlinUtils.Lerp (n [3], n [7], fz);
			
			n [0] = PerlinUtils.Lerp (n [0], n [1], fx);
			n [1] = PerlinUtils.Lerp (n [2], n [3], fx);
			
			return PerlinUtils.Lerp (n [0], n [1], fy);
		}
		
		private double SmoothNoise (int X, int Y, int Z)
		{
			double corners = (Noise (X + 1, Y + 1, Z + 1) + Noise (X + 1, Y - 1, Z + 1) +
				Noise (X - 1, Y + 1, Z + 1) + Noise (X - 1, Y - 1, Z + 1) + 
				Noise (X + 1, Y + 1, Z - 1) + Noise (X + 1, Y - 1, Z - 1) +
				Noise (X - 1, Y + 1, Z - 1) + Noise (X - 1, Y - 1, Z - 1)) / 32;
			double sides = (Noise (X - 1, Y, Z + 1) + Noise (X + 1, Y, Z + 1) + Noise (X, Y - 1, Z + 1) + Noise (X, Y + 1, Z + 1) + 
					Noise (X - 1, Y, Z) + Noise (X + 1, Y, Z) + Noise (X, Y - 1, Z) + Noise (X, Y + 1, Z) + 
					Noise (X - 1, Y, Z - 1) + Noise (X + 1, Y, Z - 1) + Noise (X, Y - 1, Z - 1) + Noise (X, Y + 1, Z - 1)
				) / 48; //1/4 average
			double center = Noise (X, Y, Z) / 2;
			return corners + sides + center;
		}
		
		private double Noise (int X, int Y, int Z)
		{
			X %= NUM_VALS;
			Y %= NUM_VALS;
			Z %= NUM_VALS;
			if (X < 0 || X >= NUM_VALS || Y < 0 || Y >= NUM_VALS ||Z < 0 || Z >= NUM_VALS)
				return 1;
			return vals [X, Y, Z];
		}
	}
}