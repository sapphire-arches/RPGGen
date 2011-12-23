using System;
using Substrate;
using RPGGen.TerrainGeneration;

namespace RPGGen.TerrainGeneration.ChunkFillers
{
	public interface ChunkFiller
	{
		void FillColum(int X, int Y, ChunkRef CR, ChunkProvider CP);
	}
}

