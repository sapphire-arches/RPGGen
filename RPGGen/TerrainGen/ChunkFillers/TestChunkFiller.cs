using System;
using Substrate;
using MCLib.Noise;

namespace RPGGen.TerrainGeneration.ChunkFillers
{
	public class TestChunkFiller : ChunkFiller
	{
		public TestChunkFiller ()
		{
		}
		
		public double GetDensity (int X, int Y, int Z, ChunkProvider CP)
		{
			if (Y == 0)
				return Double.MaxValue;
			Perlin3D p3 = CP.P3D;
			
			double f = p3.Get (X, 1 / 64.0, Y, 1 / 128.0, Z, 1 / 64.0, 0.5, 8);
			f += 1 - ((Y + 20) / 128.0);
			
			return f;
		}
	}
}

