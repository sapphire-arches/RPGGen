using System;
using Substrate;
using MCLib.Rand;
using RPGGen.TownGeneration.BuildingGeneration;

namespace RPGGen.TownGeneration
{
	public class Town
	{
		Building[] _buildings;
		
		public Town (int NumBuildings)
		{
			MersenneTwister mt = new MersenneTwister (System.DateTime.Now);
			_buildings = new Building[NumBuilding];
			for (int i = 0; i < NumBuildings; ++i) {
				_buildings [i] = GetRandomBuilding (mt);
			}
		}
		
		public bool CanPlace (int x, int z, NbtWorld world)
		{
			return true;
		}
		
		private static Building GetRandomBuilding (MersenneTwister MT)
		{
			double d = MT.NextDouble ();
			//TODO: Randomly select a building type here. For now, we always use a SmallHouse
			WallSignOrientation o = WallSignOrientation.North;
			int goal = MT.Next (4);
			switch (goal) {
			case 0:
				o = WallSignOrientation.NORTH;
				break;
			case 1:
				o = WallSignOrientation.EAST;
				break;
			case 2:
				o = WallSignOrientation.SOUTH;
				break;
			case 3:
				o = WallSignOrientation.WEST;
				break;
			default:
				Console.WriteLine ("BADNESS IN PICK OREINTATION FOR BUILDING");
				break;
			}
			return new SmallHouse (o);
		}
	}
}
