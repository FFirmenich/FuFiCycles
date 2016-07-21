using Fusee.Math.Core;
using System.Collections.Generic;

namespace Fusee.FuFiCycles.Core {
	public static class GameSettings {
		public static FuFiCycles INSTANCE;
		public static bool SHOW_MINIMAP = true;
		public static byte PLAYER_QUANTITY = 2;
		public static ushort MAP_SIZE;
		public static float4x4 SCENE_SCALE = float4x4.CreateScale(0.04f);
		public static bool SHOWMENU = true;
		public static byte MAXROUNDS = 3;
		// Vars that can be changed through the website
		public static bool WEB = false;
		public static string PLAYER1_NAME = "PLAYER1";
		public static string PLAYER2_NAME = "PLAYER2";
		public static float3 CYCLE1_COLOR = new float3(0, 0.9f, 1f);
		public static float3 CYCLE2_COLOR = new float3(0, 1.0f, 0);
		public static int SPEED = 50;
	}
}