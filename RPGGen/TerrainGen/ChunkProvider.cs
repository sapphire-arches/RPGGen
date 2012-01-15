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
		
		public ChunkRef GetChunk (NbtWorld World, int X, int Z)
		{
			BiomePicker bp = BiomePicker.Get ();
			double temp;
			double h;
			ChunkRef tr = null;
			if (World.GetChunkManager ().ChunkExists (X, Z)) {
				World.GetChunkManager ().DeleteChunk (X, Z); //Delete the chunk if it exists.
			}
			tr = World.GetChunkManager ().CreateChunk (X, Z);
			for (int x = 0; x < 16; ++x) {
				for (int z = 0; z < 16; ++z) {
					temp = _temperature.Get (x + X * 16, 1.0 / 256, z + Z * 16, 1.0 / 256, 0.5, 8);
					h = _temperature.Get (x + X * 16, 1.0 / 256, z + Z * 16, 1.0 / 256, 0.5, 8);
					
					for (int y = 0; y < 128; ++y) {
						double f = bp.Get (x + X * 16, y, z + Z * 16, temp, h, this);
						int id = bp.GetMapping (temp, h, f);
						if (id != BlockInfo.Air.ID)
							tr.Blocks.SetID (x, y, z, id);
					}
				}
			}
			tr.IsTerrainPopulated = false;
			return tr;
		}
	}
}

