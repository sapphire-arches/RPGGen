using System.Collections.Generic;
using Substrate;
using MCLib.Rand;

namespace RPGGen.TownGeneration {
	class TownPlacer {
		//Constant representing the square area that must be within 5 blocks +- to place a town
		const int TOWN_RAD = 10;
		BetaWorld _world;
		MersenneTwister _rand;
		//X and Z maxes.
		int _xMax, _zMax;
		
		public TownPlacer (BetaWorld World, uint Seed, int XMax, int ZMax)
		{
			this._world = World;
			this._rand = new MersenneTwister (Seed);
			this._xMax = XMax;
			this._zMax = ZMax;
		}
		
		public List<Vector3> GetTownLocations (int NumTowns)
		{
			List<Vector3 > tr = new List<Vector3> ();
			for (int i = 0; i < NumTowns; ++i) {
				Vector3 pos = null;
				for (int j = 0; j < 5; ++j) {
					pos = TryPlaceTown (tr);
					if (pos != null)
						break;
				}
				tr.Add (pos);
			}
			return tr;
		}
		
		private Vector3 TryPlaceTown (List<Vector3> Others)
		{
			//Save the time it takes to call _world.GetBlockManager
			BlockManager bm = _world.GetBlockManager ();
			int x = (int)(_rand.Next () * _xMax);
			int z = (int)(_rand.Next () * _zMax);
			Vector3 tr = new Vector3 ();
			tr.X = x;
			tr.Z = z;
			tr.Y = bm.GetHeight (x, z);
			//Make sure that +- town radius is inside the world
			if (x - 10 < 0 || z - 10 < 0 || z + 10 > _zMax || x + 10 > _xMax)
				return null;
			//Get the min and max y values inside the town radius and make sure that they are within +- 5
			int yMin = 0;
			int yMax = 128;
			for (int xx = x - 10; xx < x + 10; ++xx) {
				for (int zz = z - 10; zz < z + 10; ++zz) {
					int h = bm.GetHeight (xx, zz);
					yMin = Min (yMin, h);
					yMax = Max (yMax, h);
				}
			}
			//Difference is > +- 10 from orrigin.
			if (yMax - yMin > 10)
				return null;
			//Make sure that the towns are atleast 5 * the town radius appart
			foreach (Vector3 v in Others) {
				if (SquaredDistance (v, tr) < 25 * TOWN_RAD * TOWN_RAD)
					return null;
			}
			return tr;
		}
		
		private int Min (int One, int Two)
		{
			return (One < Two) ? One : Two;
		}
		
		private int Max (int One, int Two)
		{
			return (One > Two) ? One : Two;
		}
		
		private double SquaredDistance (Vector3 One, Vector3 Two)
		{
			double xDif = One.X - Two.X;
			double yDif = One.Y - Two.Y;
			double zDif = One.Z - Two.Z;
			return xDif * xDif + yDif * yDif + zDif * zDif;
		}
	}
}