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
		
		object _worldLock = new object();
		
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
				for (int z = 0; z < Height; ++z) {
					Thread t = new Thread (Run);
					t.Name = "Chunk Generation Thread (" + x + ", " + z + ")";
					t.Start (new ChunkThreadParams (x, z, 1, 1, World));
					ts [x + z * Width] = t;
				}
			}
			for (int i = 0; i < ts.Length; ++i) {
				ts [i].Join ();
			}
		}
		
		public ChunkRef GetChunk (NbtWorld World, int X, int Z)
		{
			ChunkRef tr;
			lock (World) {
				if (World.GetChunkManager ().ChunkExists (X, Z)) {
					World.GetChunkManager ().DeleteChunk (X, Z); //Delete the chunk if it exists.
				}
				tr = World.GetChunkManager ().CreateChunk (X, Z);
				Populate (tr);
				return tr;
			}
		}
		
		private void Run (object Params)
		{
			ChunkThreadParams param = (ChunkThreadParams)Params;
			
			for (int x = 0; x < param.Width; ++x) {
				for (int z = 0; z < param.Height; ++z) {
					GetChunk (param.World, x + param.X, z + param.Z);
				}
			}
		}
		
		private void Populate (ChunkRef Ret)
		{
			//Param extraction
			ChunkRef tr = Ret;
			int X = tr.X;
			int Z = tr.Z;
			//Lerp lambada
			Func<double, double, double, double > lerp = (double q, double u, double e) => (u * e) + (q * (1 - e));
			//
			BiomePicker bp = BiomePicker.Get ();
			AlphaBlockCollection bc = tr.Blocks;
			//We will call these by ourselves, when we know that all threads are done.
			bc.AutoLight = false;
			bc.AutoFluid = false;
			bc.AutoTileTick = false;
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
					lock (bc) {
						bc.SetID (x, 0, z, BlockInfo.Bedrock.ID);
						int hei = 0;
						for (int y = 1; y < 128; ++y) {
							if (y % 8 == 0) {
								f = nextF;
								nextF = bp.Get (x + X * 16, y, z + Z * 16, temp, h, this);
							}
							interpolatedF = lerp (f, nextF, (y % 8) / 8.0); 
							int id = bp.GetMapping (temp, h, interpolatedF);
							if (bc.GetID (x, y, z) != id)
								bc.SetID (x, y, z, id);
							if (id != BlockInfo.Air.ID) {
								hei = y;
							}
						}
						//Grassify.
						if (hei > 0 && bc.GetID (x, hei, z) == BlockInfo.Dirt.ID) {
							bc.SetID (x, hei, z, BlockInfo.Grass.ID);
						}
					}
				} 
			}
			tr.IsTerrainPopulated = false;
			Console.WriteLine ("Built chunk: {0}, {1}", X, Z);
		}
	}
	
	internal class ChunkThreadParams {
		int _x;
		int _z;
		int _width;
		int _height;
		NbtWorld _world;
		
		public int X {
			get { return _x;}
		}
		
		public int Z {
			get { return _z;}
		}
		
		public int Width {
			get { return _width;}
		}
		
		public int Height {
			get { return _height;}
		}
		
		public NbtWorld World {
			get {return _world;}
		}
		
		public ChunkThreadParams (int X, int Z, int Width, int Height, NbtWorld World)
		{
			_x = X;
			_z = Z;
			_width = Width;
			_height = Height;
			_world = World;
		}
	}
}

