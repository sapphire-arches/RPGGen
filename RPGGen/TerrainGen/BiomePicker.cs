using System;
using RPGGen.TerrainGeneration.ChunkFillers;

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
		
		public RPGGen.TerrainGeneration.ChunkFillers.ChunkFiller Get (double Temperature, double Humidity)
		{
			return test;
		}
	}
}

