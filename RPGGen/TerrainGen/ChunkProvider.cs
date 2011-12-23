using System;
using MCLib.Noise;
using Substrate;

namespace RPGGen.TerrainGeneration
{
	public class ChunkProvider
	{
		Perlin2D _temperature;
		Perlin2D _humidity;
		Perlin3D _p3d;
		Perlin2D _p2d;
		
		public Perlin2D Temperature {
			get { return _temperature;}
		}
		
		public Perlin2D Humidity {
			get { return _humidity;}
		}
		
		public Perlin2D P2D {
			get { return _p2d;}
		}
		
		public Perlin3D P3D {
			get { return _p3d;}
		}
		
		public ChunkProvider (uint seed)
		{
			Console.Write ("Building Perlin noise maps... ");
			this._temperature = new Perlin2D (seed);
			this._humidity = new Perlin2D (seed + 1);
			this._p3d = new Perlin3D (seed + 2);
			this._p2d = new Perlin2D (seed + 3);
			Console.WriteLine ("Done");
		}
		
		public ChunkRef GetChunk (NbtWorld World, int X, int Y)
		{
			BiomePicker bp = BiomePicker.Get ();
			double temp;
			double h;
			ChunkRef tr = null;
			if (World.GetChunkManager ().ChunkExists (X, Y)) {
				tr = World.GetChunkManager ().GetChunkRef (X, Y);
			} else {
				tr = World.GetChunkManager ().CreateChunk (X, Y);
			}
			for (int x = 0; x < 16; ++x) {
				for (int z = 0; z < 16; ++z) {
					temp = _temperature.Get (x + X * 16, 1.0 / 256, z + z * 16, 1.0 / 256, 0.5, 8);
					h = _temperature.Get (x + X * 16, 1.0 / 256, z + z * 16, 1.0 / 256, 0.5, 8);
					bp.Get (temp, h).FillColum (x, z, tr, this);
				}
			}
			tr.IsTerrainPopulated = false;
			return tr;
		}
	}
}

