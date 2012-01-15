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
			MersenneTwister mt = new MersenneTwister ((uint)System.DateTime.Now.Ticks);
			_buildings = new Building[NumBuildings];
			for (int i = 0; i < NumBuildings; ++i) {
				_buildings [i] = GetRandomBuilding (mt);
			}
		}
		
		public bool CanPlace (int X, int Z, NbtWorld world)
		{
			return _buildings[0].CanPlace(X, Z, world);
		}
		
		private static Building GetRandomBuilding (MersenneTwister MT)
		{
			//TODO: Randomly select a building type here. For now, we always use a SmallHouse
			WallSignOrientation o = WallSignOrientation.NORTH;
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

