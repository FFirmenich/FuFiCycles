using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.FuFiCycles.Core {
	public static class GameSettings {
		public static FuFiCycles INSTANCE;
		public static List<Round> ROUNDS = new List<Round>();
		public static bool SHOW_MINIMAP = true;
		public static int PLAYER_QUANTITY = 2;
		public static int MAP_SIZE;
	}
}