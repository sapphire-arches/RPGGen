using System;
using System.Drawing;
using MCLib;
using Substrate;
using RPGGen.TerrainGeneration;

namespace RPGGen
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			//Console.Write ("Enter name of world: ");
			String worldName = "RPGTest";//Console.ReadLine ();
			String sep = System.IO.Path.DirectorySeparatorChar.ToString ();
			//Create the world
			NbtWorld world = MCLib.Util.Util.CreateWorld (MCLib.Util.Util.GetPathToMCSaves () + sep + worldName + sep);
			//world.Level.LevelName = worldName;
			//Generate the terain.
			ChunkProvider cp = new ChunkProvider (100);
			for (int x = 0; x < 16; ++x) {
				for (int z = 0; z < 16; ++z) {
					cp.GetChunk (world, x, z);
				}
				world.GetChunkManager ().Save ();
				Console.WriteLine ("X: {0}", x);
			}
			world.GetChunkManager ().Save ();
			world.Level.Player.Spawn = new SpawnPoint(0, world.GetBlockManager ().GetHeight (0, 0), 0);
			world.Level.Player.Position.Y = world.GetBlockManager ().GetHeight (0, 0) + 1;
			world.Level.Save ();
		}
	}
}