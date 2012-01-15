using System;
using RPGGen.TerrainGeneration.ChunkFillers;
using Substrate;

namespace RPGGen.TerrainGeneration
{
	public class BiomePicker
	{
		static BiomePicker _instance = new BiomePicker();
		
		public static BiomePicker Get ()
		{
			return _instance;
		}
		
		//List of all known CFs.
		static ChunkFiller test = new TestChunkFiller();
		
		BiomePicker ()
		{
		}
		
		public double Get (int X, int Y, int Z, double Temperature, double Humidity, ChunkProvider CP)
		{
			return test.GetDensity (X, Y, Z, CP);
		}
		
		public int GetMapping (double Temperature, double Humidity, double f)
		{
			if (f > 1.2)
				return BlockInfo.Bedrock.ID;
			if (f > 1.0)
				return BlockInfo.Stone.ID;
			if (f > 0.98)
				return BlockInfo.Dirt.ID;
			return BlockInfo.Air.ID;
		}
	}
}

