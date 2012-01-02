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
		
		public void FillColum (int X, int Z, ChunkRef CR, ChunkProvider CP)
		{
			Perlin3D p3 = CP.P3D;
			int x = X + CR.X * 16;
			int z = Z + CR.Z * 16;
			//int yMax = (int)(CP.P2D.Get (x, 1 / 64.0, z, 1 / 64.0, 0.5, 8) * 128);
			AlphaBlockCollection c = CR.Blocks;
			
			for (int y = 1; y < 128; ++y) {
				double f = p3.Get (x, 1 / 64.0, y, 1 / 128.0, z, 1 / 64.0, 0.5, 8);
				f += 1 - ((y + 20) / 128.0);
				if (f > 1) {
					c.SetID (X, y, Z, BlockInfo.Stone.ID);
				} else {
#if DEBUG
					c.SetID (X, y, Z, BlockInfo.Air.ID);
#endif
				}
			}
			//dirt and grass.
			int height = CR.Blocks.GetHeight (X, Z);
			for (int y = height - 1; y < height; ++y) {
				if (y > 0)
					c.SetID (X, y, Z, BlockInfo.Dirt.ID);
			}
			if (height - 1 > 0)
				c.SetID (X, height, Z, BlockInfo.Grass.ID);
			//Bottom layer must be bedrock
			c.SetID (X, 0, Z, BlockInfo.Bedrock.ID);
		}
	}
}

