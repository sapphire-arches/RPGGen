using System;
using System.IO;
using Substrate;

namespace MCLib.Util
{
	public class Util
	{
		public static String GetPathToMCSaves ()
		{
			String sep = Path.DirectorySeparatorChar.ToString ();
			String appdata = "." + sep + "Output Levels" + sep;
			PlatformID plid = Environment.OSVersion.Platform;
			#if DEBUG
			Console.WriteLine (plid);
			#endif
			if (plid == PlatformID.Win32Windows || plid == PlatformID.Win32NT) {
				#if DEBUG
				Console.WriteLine ("WINDOWS!");
				#endif
				appdata = @"C:\Users\" + Environment.UserName + @"\Appdata\Roaming\.minecraft\saves\";
			} else if (plid == PlatformID.MacOSX) {
				#if DEBUG
				Console.WriteLine ("OSX!");
				#endif
				appdata = "/Users/" + Environment.UserName + "/Library/Application Support/minecraft/saves/";
			} else if (plid == PlatformID.Unix) {
				#if DEBUG
				Console.WriteLine ("LINIX!");
				#endif
				appdata = "/home/" + Environment.UserName + "/.minecraft/saves/";
			}
			return appdata;
		}
		
		public static NbtWorld CreateWorld (string path)
		{
			if (!Directory.Exists (path) || !File.Exists (path + Path.DirectorySeparatorChar + "level.dat")) {
				Directory.CreateDirectory (path);
				return BetaWorld.Create (path);
			}
			return BetaWorld.Open (path);
		}
	}
	
	public class IntegerMath {
		public static int Abs (int I)
		{
			return (I < 0) ? -I : I;
		}
		
		public static int Sign (double D)
		{
			return (D < 0) ? -1 : ((D == 0) ? 0 : 1);
		}
	}
}

