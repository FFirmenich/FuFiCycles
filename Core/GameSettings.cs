using Fusee.Math.Core;
using System.Collections.Generic;

namespace Fusee.FuFiCycles.Core {
	public static class GameSettings {
		public static FuFiCycles INSTANCE;
		public static List<Round> ROUNDS = new List<Round>();
		public static bool SHOW_MINIMAP = true;
		public static byte PLAYER_QUANTITY = 2;
		public static int MAP_SIZE;
		public static float4x4 SCENE_SCALE = float4x4.CreateScale(0.04f);
	}
}