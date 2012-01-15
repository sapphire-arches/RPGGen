using System;
using Substrate;
using RPGGen.TerrainGeneration;

namespace RPGGen.TerrainGeneration.ChunkFillers
{
	public interface ChunkFiller
	{
		double GetDensity(int X, int Y, int Z, ChunkProvider CP);
	}
}

