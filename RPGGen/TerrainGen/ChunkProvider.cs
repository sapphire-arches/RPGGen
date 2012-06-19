using System;
using MCLib.Noise;
using Substrate;
using System.Threading;

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
		
		public void PopulateWorld (NbtWorld World, int Width, int Height)
		{
			Thread[] ts = new Thread [Width * Height];
			ChunkRef tr;
			for (int x = 0; x < Width; ++x) {
				for (int y = 0; y < Height; ++y) {
					if (World.GetChunkManager ().ChunkExists (x, y)) {
						World.GetChunkManager ().DeleteChunk (x, y); //Delete the chunk if it exists.
					}
					tr = World.GetChunkManager ().CreateChunk (x, y);
					Thread t = new Thread (Populate);
					t.Name = "Chunk Generation Thread (" + x + ", " + y + ")";
					t.Start (tr);
					ts [x + y * Width] = t;
				}
			}
			for (int i = 0; i < ts.Length; ++i) {
				ts [i].Join ();
			}
		}
		
		public ChunkRef GetChunk (NbtWorld World, int X, int Z)
		{
			ChunkRef tr;
			if (World.GetChunkManager ().ChunkExists (X, Z)) {
				World.GetChunkManager ().DeleteChunk (X, Z); //Delete the chunk if it exists.
			}
			tr = World.GetChunkManager ().CreateChunk (X, Z);
			Populate (tr);
			return tr;
		}
		
		private void Populate (object param)
		{
			//Param extraction
			ChunkRef tr = (ChunkRef)param;
			int X = tr.X;
			int Z = tr.Z;
			//Lerp lambada
			Func<double, double, double, double > lerp = (double q, double u, double e) => (u * e) + (q * (1 - e));
			//
			BiomePicker bp = BiomePicker.Get ();
			double temp;
			double h;
			for (int x = 0; x < 16; ++x) {
				for (int z = 0; z < 16; ++z) {
					temp = _temperature.Get (x + X * 16, 1.0 / 256, z + Z * 16, 1.0 / 256, 0.5, 8);
					h = _temperature.Get (x + X * 16, 1.0 / 256, z + Z * 16, 1.0 / 256, 0.5, 8);
					
					double f = Double.MaxValue;
					double nextF = bp.Get (x + X * 16, 0, z + Z * 16, temp, h, this);
					double interpolatedF = 0;
					
					//Bedrock floor.
					tr.Blocks.SetID (x, 0, z, BlockInfo.Bedrock.ID);
					for (int y = 1; y < 128; ++y) {
						if (y % 8 == 0) {
							f = nextF;
							nextF = bp.Get (x + X * 16, y, z + Z * 16, temp, h, this);
						}
						interpolatedF = lerp (f, nextF, (y % 8) / 8.0); 
						int id = bp.GetMapping (temp, h, interpolatedF);
						if (tr.Blocks.GetID (x, y, z) != id)
							tr.Blocks.SetID (x, y, z, id);
					}
					//Grassify.
					int hei = tr.Blocks.GetHeight (x, z) - 1;
					if (tr.Blocks.GetID (x, hei, z) == BlockInfo.Dirt.ID) {
						tr.Blocks.SetID (x, hei, z, BlockInfo.Grass.ID);
					}
				}
			}
			tr.IsTerrainPopulated = false;
		}
	}
}

