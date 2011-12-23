using System;
using System.Collections.Generic;
using MCLib.Rand;
using Substrate;

namespace RPGGen.TownGeneration
{
	public class TownGenerator
	{
		List<Town> _towns;
		MersenneTwister _mt;
		int _maxXSearchArea;
		int _maxZSearchArea;
		
		public TownGenerator (int MaxXSearchArea, int MaxZSearchArea)
		{
			this._towns = new List<Town> ();
			this._mt = new MersenneTwister ((uint)DateTime.Now.Ticks);
			this._maxXSearchArea = MaxXSearchArea;
			this._maxZSearchArea = MaxZSearchArea;
		}
		
		public bool PlaceTown (NbtWorld world, Town t)
		{
			int x;
			int z;
			for (int i = 0; i < 5; ++i) {
				x = _mt.Next (_maxXSearchArea);
				z = _mt.Next (_maxZSearchArea);
				if (t.CanPlace (x, z, world))
					return true;
			}
			return false;
		}
	}
}

