using System;
using Substrate;

namespace RPGGen.TownGeneration.BuildingGeneration
{
	public interface Building
	{
		bool CanPlace(int X, int Z, NbtWorld World);
		int[] GetFootprint();
	}
}

