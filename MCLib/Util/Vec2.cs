using System;
using Substrate;

namespace MCLib.Util
{
	public class Vector2
	{
		
		public double X {
			get;
			set;
		}
		
		public double Y {
			get;
			set;
		}
		
		public Vector2 ()
		{
			this.X = this.Y = 0;
		}
		
		public Vector2 (double X, double Y)
		{
			this.X = X;
			this.Y = Y;
		}
		
		public Vector2 (Vector2 other)
		{
			this.X = other.X;
			this.Y = other.Y;
		}
		
		/// <summary>
		/// Creates a Vec3 with (X, Y Z) of (this.X, Y, this.Y)
		/// </summary>
		/// <returns>
		/// The vec3.
		/// </returns>
		/// <param name='Y'>
		/// The Y coordinate of the Vec3 to create.
		/// </param>
		public Vector3 CreateVec3 (double Y)
		{
			Vector3 v3 = new Vector3 ();
			v3.X = this.X;
			v3.Y = Y;
			v3.Z = this.Y;
			return v3;
		}
		
		public override string ToString ()
		{
			return "(" + this.X + ", " + this.Y + ")";
		}
	}
}

