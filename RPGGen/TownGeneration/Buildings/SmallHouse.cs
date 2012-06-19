using System;
using Substrate;

namespace RPGGen.TownGeneration.BuildingGeneration
{
	public class SmallHouse : IBuilding
	{
		static int _idCount;
		Footprint _fp;
		Orientation _o;
		int _x, _z, _id;
		
		public SmallHouse (Orientation o, int X, int Z, Town t)
		{
			this._o = o;
			if (o == Orientation.NORTH || 0 == Orientation.SOUTH)
				_fp = new Footprint (8, 6);
			else
				_fp = new Footprint (6, 8);
			this._x = X;
			this._z = Z;
			this._id = _idCount++;
			if (!t.IsIDRegisted (2))
				t.RegisterID (2);
		}
		
		public bool CanPlace (NbtWorld World)
		{
			int averageHeight = 0;
			int maxHeight = 0;
			BlockManager bm = (BlockManager)World.GetBlockManager ();
			for (int x = 0; x < _fp.X; ++x) {
				for (int z = 0; z < _fp.Z; ++z) {
					int h = bm.GetHeight (x + _x, z + _z);
					averageHeight += h;
					if (maxHeight < h)
						maxHeight = h;
				}
			}
			averageHeight /= _fp.X * _fp.Z;
			if (averageHeight > 120)
				return false;
			
			for (int x = 0; x < _fp.X; ++x) {
				for (int z = 0; z < _fp.Z; ++z) {
					if (Abs (bm.GetHeight (x + _x, z + _z) - averageHeight) > 3)
						return false;
				}
			}
			//Build the house!
			BuildHouse (_x, _z, averageHeight, bm);
			return true;
		}
		
		public void Build (NbtWorld World)
		{
			int averageHeight = 0;
			BlockManager bm = (BlockManager)World.GetBlockManager ();
			for (int x = 0; x < _fp.X; ++x) {
				for (int z = 0; z < _fp.Z; ++z) {
					averageHeight += bm.GetHeight (x + _x, z + _z);
				}
			}
			averageHeight /= _fp.X * _fp.Z;
			BuildHouse (_x, _z, averageHeight, bm);
		}
		
		private void BuildHouse (int X, int Z, int AverageHeight, BlockManager bm)
		{
			for (int x = 0; x < _fp.X; ++x) {
				for (int z = 0; z < _fp.Z; ++z) {
					if (x == 0 || x == _fp.X - 1 || z == 0 || z == _fp.Z - 1) {
						//Walls. 
						for (int y = AverageHeight; y < AverageHeight + 4; ++y) {
							//Walls should be wood.
							if (y != AverageHeight + 2)
								bm.SetID (x + X, y, z + Z, BlockInfo.Wood.ID);
							else
								bm.SetID (x + X, y, z + Z, BlockInfo.Glass.ID);
						}	
					} else {
						//Floor is wooden planks.
						bm.SetID (x + X, AverageHeight, z + Z, BlockInfo.WoodPlank.ID);
					}
					//Roof.
					int peak = (int) (x * (x - _fp.X - 1) + z * (z - _fp.Z - 1));
					if (_o == Orientation.NORTH || _o == Orientation.SOUTH) {
						//Absolute value function. Graph makes the peak of the roof.
						//peak = (Abs ((int)((x - 0.5) - _fp.X / 2.0)) * -1) + 7;
						//Make the stairs face the right direction.
						bm.SetData (x + X, AverageHeight + peak, z + Z, ((x > _fp.X / 2.0) ? 0x1 : 0x0));
					} else {
						//Absolute value function. Graph makes the peak of the roof.
						//peak = (Abs ((int)((z - 0.5) - _fp.X / 2.0)) * -1) + 7;
						//Make the stairs face the right direction.
						bm.SetData (x + X, AverageHeight + peak, z + Z, ((z + 0.5 > _fp.Z / 2.0) ? 0x3 : 0x2));
					}
					bm.SetID (x + X, AverageHeight + peak, z + Z, BlockInfo.WoodStairs.ID);
					
					if (x == 0 || x == _fp.X - 1 || z == 0 || z == _fp.Z - 1) { //If it is is the sides, fill up to the house height.
						for (int y = AverageHeight + 4; y < AverageHeight + peak; ++y) {
							bm.SetID (x + X, y, z + Z, BlockInfo.Wood.ID);
						}
					}
				}
			}
		}
		
		public Footprint GetFootprint ()
		{
			return _fp;
		}
		
		public int GetID ()
		{
			return this._id;
		}
		
		new public int GetType ()
		{
			return 2;
		}
					
		private int Abs (int I) {
			return (I > 0) ? I : -I;				
		}
	}
}

