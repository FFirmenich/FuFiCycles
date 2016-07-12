using static Fusee.FuFiCycles.Core.GameSettings;

namespace Fusee.FuFiCycles.Core {
	/// <summary>
	///  One round of the game
	/// </summary>
	public class Round {
		// round id
		private int id;
		// map mirror for cycle positions
		private float[,] mapMirror;

		public Round(int id) {
			this.id = id;
			this.mapMirror = new float[MAP_SIZE, MAP_SIZE];
		}

		/// <summary>
		///  returns the id of this round
		/// </summary>
		public int getId() {
			return this.id;
		}

		/// <summary>
		///  returns the map mirror with cycle and wall positions
		/// </summary>
		public float[,] getMapMirror() {
			return this.mapMirror;
		}
	}
}
