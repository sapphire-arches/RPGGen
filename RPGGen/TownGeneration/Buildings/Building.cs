using System;
using Substrate;

namespace RPGGen.TownGeneration.BuildingGeneration
{
	public interface Building
	{
		bool CanPlace(int X, int Z, NbtWorld World);
		Footprint GetFootprint();
	}
	
	public struct Footprint {
		public Footprint (int X, int Z) {
			this.X = X;
			this.Z = Z;
		}
		public int X,Z;	
	}
}

