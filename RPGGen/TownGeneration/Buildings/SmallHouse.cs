using System;
using Substrate;

namespace RPGGen.TownGeneration.BuildingGeneration
{
	public class SmallHouse : Building
	{
		WallSignOrientation _o;
		
		public SmallHouse (WallSignOrientation o)
		{
			this._o = o;
		}
		
	}
}

