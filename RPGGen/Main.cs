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
			int pos = Console.CursorTop;
			
			cp.PopulateWorld (world, 8, 8);
			
			Console.CursorTop = pos + 1;
			world.GetChunkManager ().Save ();
			Console.WriteLine (new TownGeneration.Town (50, 2, 256, 256).CanPlace (65, 65, world));
			world.GetChunkManager ().Save ();
			if (world.Level.Player == null)
				world.Level.Player = new Player ();
			world.Level.Player.Spawn = new SpawnPoint (0, world.GetBlockManager ().GetHeight (0, 0), 0);
			world.Level.Player.Position.Y = world.GetBlockManager ().GetHeight (0, 0) + 1;
			world.Level.Time = 0; //Make it morning.
			world.Level.LastPlayed = DateTime.UtcNow.Ticks / 10000;
			world.Level.Save ();
			Console.WriteLine ("Done!");
			TimeSpan t = new TimeSpan (DateTime.Now.Ticks - start.Ticks);
			Console.WriteLine ("Took : " + t);
		}
	}
}