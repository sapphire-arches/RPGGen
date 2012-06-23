using System;
using Substrate;

namespace RPGGen.TownGeneration.BuildingGeneration
{
	public interface IBuilding
	{
		bool CanPlace(NbtWorld World);
		void Build(NbtWorld World);
		Footprint GetFootprint();
		int GetID();
		int GetType();
	}
	
	public struct Footprint {
		public Footprint (int X, int Z)
		{
			this._x = X;
			this._z = Z;
		}
		int _x,_z;	
		
		public int X {
			get { return _x;}
		}
		
		public int Z {
			get { return _z;}
		}
	}
}

