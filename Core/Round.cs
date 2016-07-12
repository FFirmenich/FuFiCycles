using static Fusee.FuFiCycles.Core.GameSettings;

namespace Fusee.FuFiCycles.Core {
	/// <summary>
	///  One round of the game
	/// </summary>
	public class Round {
		private int id;
		private float[,] mapMirror;
		public bool firstFrame = true;

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
		/// <summary>
		/// returns true if it is the first frame 
		/// </summary>
		/// <returns>boolean if it is the first frame or not</returns>
		public bool getFirstFrame() {
			return this.firstFrame;
		}
		/// <summary>
		/// sets firstFrame false
		/// </summary>
		public void setNotFirstFrame() {
			this.firstFrame = false;
		}
		
	}
}
