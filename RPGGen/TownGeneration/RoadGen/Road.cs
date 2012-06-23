using System;
using MCLib.Util;
using Substrate;

namespace RPGGen.TownGeneration.RoadGeneration
{
	public class Road
	{
		//Backend vars.
		private Vector2 _start, _end;
		private double _slope;
		
		public Vector2 Start {
			get { return _start;}
			set {
				_start = value;
				_slope = (_end.Y - _start.Y) / (_end.X - _start.X);
			}
		}
		
		public Vector2 End {
			get { return _end;}
			private set {
				_end = value;
				_slope = (_end.Y - _start.Y) / (_end.X - _start.X);
			}
		}
		
		public double Slope {
			get { return _slope;}
			private set { _slope = value;}
		}
		
		public int Width {
			get;
			private set;
		}
		
		public Road (Vector2 start, Vector2 end)
		{
			this.Start = start;
			this.End = end;
		}
	}
}

