using System;
using MCLib.Util;
using Substrate;

namespace RPGGen.TownGeneration.RoadGeneration
{
	public class RoadSegment
	{
		public Vector2 Start {
			get;
			private set;
		}
		
		public Vector2 End {
			get;
			private set;
		}
		
		public int Width {
			get;
			private set;
		}
		
		private RoadType _rt;
		
		public RoadSegment (Vector2 start, Vector2 end, int Width)
		{
			this.Start = start;
			this.End = end;
			this.Width = Width;
		}
		
		public Orientation GetOrientation ()
		{
			int xDelta = (int)(End.X - Start.X);
			int yDelta = (int)(End.Y - Start.Y);
			
			if (IntegerMath.Abs (xDelta) > IntegerMath.Abs (yDelta)) {
				if (xDelta < 0)
					return Orientation.NORTH;
				else
					return Orientation.SOUTH;
			} else {
				if (yDelta < 0)
					return Orientation.EAST;
				else
					return Orientation.WEST;
			}
			
			return Orientation.NORTH;
		}
		
		public RoadType GetRoadType (NbtWorld World)
		{
			//First, figure out which way the road is going.
			bool roadDir = Math.Abs ((End.Y - Start.Y) / (End.X - Start.X)) > 1.0;
			//Detect the min/max Y vals, as well as the average.
			
			return _rt;
		}
		
		public int RenderToWorld (BlockManager bm)
		{
			int t = Width / 2;
			Vector2 tStart = new Vector2 (Start);
			Vector2 tEnd = new Vector2 (End);
			//Number of set tiles
			int ts = 0;
			
			Orientation o = GetOrientation ();
			bool roadDir = o == Orientation.NORTH || o == Orientation.SOUTH;
			
			if (!roadDir) {
				for (int y = -t; y <= t; ++y) {
					tStart.Y = Start.Y + y;
					tEnd.Y = End.Y + y;
					ts += DrawRoadLine (tStart, tEnd, bm);
				}
			} else {
				for (int x = -t; x <= t; ++x) {
					tStart.X = Start.X + x;
					tEnd.X = End.X + x;
					ts += DrawRoadLine (tStart, tEnd, bm);
				}
			}
			return ts;		
		}
		
		private int DrawRoadLine (Vector2 Start, Vector2 End, BlockManager bm)
		{
			//How many tiles have we set?
			int ts = 0;
			//We use integer Bresenham's algorithim.
			int x = (int)Start.X;
			int y = (int)Start.Y;
			
			int xDelta = IntegerMath.Abs ((int)(End.X - Start.X));
			int yDelta = IntegerMath.Abs ((int)(End.Y - Start.Y));
			
			int s1 = IntegerMath.Sign (End.X - Start.X);
			int s2 = IntegerMath.Sign (End.Y - Start.Y);
			
			bool interchange = yDelta > xDelta;
			
			if (interchange) {
				int t = xDelta;
				xDelta = yDelta;
				yDelta = t;
			}
			
			int error = 2 * yDelta - xDelta;
			
			for (int i = 0; i < xDelta; ++i) {
				//Place a road block at x, y
				SetToRoadBlock (x, y, bm);
				++ts;
				while (error > 0) {
					if (interchange) {
						x += s1;
					} else {
						y += s2;
					}
					error -= 2 * xDelta;
				}
				
				if (interchange)
					y += s2;
				else 
					x += s1;
				error += 2 * yDelta;
			}
			return ts;
		}
		
		private void SetToRoadBlock (int X, int Z, BlockManager bm)
		{
			//Average the 7x7 block surrounding the thing we are going to sample.
			int average = 0;
			for (int xx = -3; xx <= 3; ++xx) {
				for (int zz = -3; zz <= 3; ++zz) {
					average += bm.GetHeight (X + xx, Z + zz) - 1;
				}
			}
			average /= 49;
			//Set the block below the average to dirt, at the average to cobble, and everything above to air
			for (int y = average; y < bm.GetHeight(X, Z); ++y) {
				bm.SetID (X, y, Z, BlockInfo.Air.ID);
			}
			bm.SetID (X, average, Z, BlockInfo.Cobblestone.ID);
			bm.SetID (X, average - 1, Z, BlockInfo.Dirt.ID);
		}
	}
}

