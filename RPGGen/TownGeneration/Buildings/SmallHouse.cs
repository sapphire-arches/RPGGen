using System;
using Substrate;

namespace RPGGen.TownGeneration.BuildingGeneration
{
	public class SmallHouse : Building
	{
		Footprint _fp;
		WallSignOrientation _o;
		
		public SmallHouse (WallSignOrientation o)
		{
			this._o = o;
			if (o == WallSignOrientation.NORTH || 0 == WallSignOrientation.SOUTH)
				_fp = new Footprint(8, 6);
			else
				_fp = new Footprint(6, 8);
		}
		
		public bool CanPlace(int X, int Z, NbtWorld World) {
			int averageHeight = 0;
			int maxHeight = 0;
			BlockManager bm = (BlockManager)World.GetBlockManager();
			for (int x = 0; x < _fp.X; ++x) {
				for (int z = 0; z < _fp.Z; ++z) {
					int h = bm.GetHeight(x + X, z + Z);
					averageHeight += h;
					if (maxHeight < h)
						maxHeight = h;
				}
			}
			averageHeight /= _fp.X * _fp.Z;
			if (averageHeight > 120) return false;
			
			for (int x = 0; x < _fp.X; ++x) {
				for (int z = 0; z < _fp.Z; ++z) {
					if (Abs(bm.GetHeight(x + X, z + Z) - averageHeight) > 3)
						return false;
				}
			}
			//Build the house!
			BuildHouse(X, Z, averageHeight, bm);
			return true;
		}
		
		private void BuildHouse(int X, int Z, int AverageHeight, BlockManager bm) {
			for (int x = 0; x < _fp.X; ++x) {
				for (int z = 0; z < _fp.Z; ++z) {
					if (x == 0 || x == _fp.X - 1 || z == 0 || z == _fp.Z - 1) {
						//Always make the outisde bit of the floor wood.
						bm.SetID(x + X, AverageHeight, z + Z, BlockInfo.Wood.ID);
						//Walls. 
						for (int y = AverageHeight; y < AverageHeight + 4; ++y) {
							//Walls should be wood.
							bm.SetID(x + X, y, z + Z, BlockInfo.Wood.ID);
						}
					} else {
						//Make a wood/stone checkerboard pattern on the inside floor.
						if ((1 + x + z * _fp.X) % 2 == 0)
							bm.SetID (x + X, AverageHeight, z + Z, BlockInfo.WoodPlank.ID);
						else
							bm.SetID(x + X, AverageHeight, z + Z, BlockInfo.Stone.ID);
					}
					//Roof.
					int peak = 0;
					if (_o == WallSignOrientation.NORTH || _o == WallSignOrientation.SOUTH) {
						peak = (Abs((int)(x - _fp.X / 2.0)) * -1) + 3;
					} else {
						peak = (Abs((int)(z - _fp.X / 2.0)) * -1) + 3;
					}
					bm.SetID(x + X, AverageHeight + peak + 3, z + Z, BlockInfo.Cobblestone.ID);
				}
			}
		}
		
		public Footprint GetFootprint() {
			return _fp;
		}
					
		private int Abs (int I) {
			return (I > 0) ? I : -I;				
		}
	}
}

