using System;
using System.Collections.Generic;
using Substrate;
using MCLib.Rand;
using MCLib.Util;
using RPGGen.TownGeneration.BuildingGeneration;

namespace RPGGen.TownGeneration
{
	public class Town
	{
		Random _rand;
		List<IBuilding> _buildings;
		List<Vector2> _roadConnectPoints;
		List<int> _registeredIDs;
		int _radius;
		int _xMax, _zMax;
		int [,] _map;
		
		/// <summary>
		/// Gets the road connect points relative to the town's center.
		/// </summary>
		/// <value>
		/// The road connect points relative to the town's center.
		/// </value>
		public List<Vector2> RoadConnectPoints {
			get { return _roadConnectPoints;}
		}
		
		public Town (int Radius, uint Seed, int WorldXMax, int WorldZMax)
		{
			Console.Write ("Generating Town... ");
			_rand = new MersenneTwister (Seed);
			this._radius = Radius;
			this._xMax = WorldXMax;
			this._zMax = WorldZMax;
			this._map = new int [2 * Radius, 2 * Radius];
			this._registeredIDs = new List<int> ();
			
			_buildings = new List<IBuilding> ();
			
			//Build the road connection points list.
			_roadConnectPoints = new List<Vector2> ();
			_roadConnectPoints.Add (new Vector2 (-_radius, 0));
			_roadConnectPoints.Add (new Vector2 (_radius, 0));
			Console.WriteLine ("Done");
		}
		
		public bool CanPlace (int X, int Z, NbtWorld World)
		{
			if (X - _radius < 0 || X + _radius >= _xMax || Z - _radius < 0 || Z + _radius >= _zMax)
				return false;
			BlockManager bm = (BlockManager)World.GetBlockManager ();
			bool tr = true;
			foreach (IBuilding b in _buildings) {
				tr = tr && b.CanPlace (World);
			}
			if (tr) {
				foreach (IBuilding b in _buildings) {
					b.Build (World);
				}
				
				for (int x = 0; x < 2 * _radius; ++x) {
					for (int z = 0; z < 2 * _radius; ++z) {
						int xp = x + X - _radius;
						int zp = z + Z - _radius;
						bool isEdge = x == 0 || z == 0 || x == 2 * _radius - 1 || z == 2 * _radius - 1;
						if (isEdge) {
							bm.SetID (xp, bm.GetHeight (xp, zp) - 1, zp, BlockInfo.Wool.ID);
							bm.SetData (xp, bm.GetHeight (xp, zp) - 1, zp, (int)WoolColor.PURPLE);
						}
					}
				}
			}
			return tr;
		}
		
		/// <summary>
		/// Registers the a Type ID
		/// </summary>
		/// <param name='ID'>
		/// TypeID to be regestered
		/// </param>
		/// <exception cref='InvalidOperationException'>
		/// Is thrown when the ID is already registed.
		/// </exception>
		public void RegisterID (int ID)
		{
			if (IsIDRegisted (ID))
				throw new InvalidOperationException ("Registed already registed ID");
			_registeredIDs.Add (ID);
		}
		
		/// <summary>
		/// Determines whether this instance of a <c>Town</c> has this Type ID registed yet.
		/// </summary>
		/// <returns>
		/// <c>true</c> if this instance is identifier registed the specified Type ID; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='ID'>
		/// If the ID is registed.
		/// </param>
		public bool IsIDRegisted (int ID)
		{
			return _registeredIDs.Contains (ID);
		}
		
		private static Orientation GetRandomBuildingOrientation (int X, int Z, MersenneTwister MT)
		{
			//TODO: Randomly select a building type here. For now, we always use a SmallHouse
			Orientation o = Orientation.NORTH;
			int dir = MT.Next (4);
			switch (dir) {
			case 0:
				o = Orientation.NORTH;
				break;
			case 1:
				o = Orientation.EAST;
				break;
			case 2:
				o = Orientation.SOUTH;
				break;
			case 3:
				o = Orientation.WEST;
				break;
			default:
				Console.WriteLine ("BADNESS IN PICK OREINTATION FOR BUILDING");
				break;
			}
			return o;
		}
		
		private bool RasterizeToMap (int X, int Y, Footprint FP, int To)
		{
			if (X < 0 || Y < 0 || X + FP.X >= 2 * _radius || Y + FP.Z >= 2 * _radius)
				return false;
			for (int x = 0; x < FP.X; ++x) {
				for (int y = 0; y < FP.Z; ++y) {
					_map [x, y] = To;
				}
			}
			return true;
		}
		
		private string DecodeInt (int I)
		{
			return "Dir: " + (I & 0x4) + " Type: " + ((I >> 2) & 0xFF) + " ID" + (I >> 10);
		}
		
		private int CreateIntIndentifier (int ID, int BuildingType, byte Dir)
		{
			if (!IsIDRegisted(ID))
				throw new InvalidOperationException ("Can't use unregisted IDs! ID: " + ID);
			return (ID << 10) | (BuildingType << 2) | Dir;
		}
		
		private byte GetForOrientation (Orientation BO)
		{
			switch (BO) {
			case Orientation.NORTH:
				return 0;
			case Orientation.EAST:
				return 1;
			case Orientation.SOUTH:
				return 2;
			case Orientation.WEST:
				return 3;
			default:
				return 4;
			}
		}
	}
}