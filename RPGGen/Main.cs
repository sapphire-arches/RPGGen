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
			DateTime start = System.DateTime.Now;
			//Console.Write ("Enter name of world: ");
			String worldName = "RPGTest";//Console.ReadLine ();
			String sep = System.IO.Path.DirectorySeparatorChar.ToString ();
			//Create the world
			NbtWorld world = MCLib.Util.Util.CreateWorld (MCLib.Util.Util.GetPathToMCSaves () + worldName + sep);
			world.Level.LevelName = worldName;
#if DEBUG
			world.Level.GameType = GameType.CREATIVE;
#endif
			//Save the level file.
			world.Level.Save ();
			//Generate the terain.
			ChunkProvider cp = new ChunkProvider (100);
			
			cp.PopulateWorld (world, 8, 8);
			
			//Update lights.
			Console.Write ("Updating Light Data... ");
			{
				Substrate.Core.IChunkManager cm = world.GetChunkManager ();
				foreach (ChunkRef c in cm) {
					AlphaBlockCollection blocks = (AlphaBlockCollection)c.Blocks;
					int xdim = blocks.XDim;
					int ydim = blocks.YDim;
					int zdim = blocks.ZDim;
					for (int x = 0; x < xdim; ++x) {
						for (int y = 0; y < ydim; ++y) {
							for (int z = 0; z < zdim; ++z) {
								blocks.UpdateSkyLight (x, y, z);
								blocks.UpdateBlockLight (x, y, z);
								blocks.UpdateFluid (x, y, z);
							}
						}
					}
					blocks.RebuildHeightMap ();
					world.GetChunkManager ().Save ();
				}
			}
			Console.WriteLine ("Done");
			
			Console.WriteLine (new TownGeneration.Town (50, 2, 256, 256).CanPlace (65, 65, world));
			world.GetChunkManager ().Save ();
			if (world.Level.Player == null)
				world.Level.Player = new Player ();
			world.Level.Player.Spawn = new SpawnPoint (0, world.GetBlockManager ().GetHeight (0, 0), 0);
			world.Level.Player.Position.Y = world.GetBlockManager ().GetHeight (0, 0) + 1;
			world.Level.Time = 0; //Make it morning.
			world.Level.LastPlayed = DateTime.UtcNow.Ticks / 10000;
			world.Level.Save ();
			Console.Write ("Done! ");
			TimeSpan t = new TimeSpan (DateTime.Now.Ticks - start.Ticks);
			Console.WriteLine ("Took : " + t);
		}
	}
}