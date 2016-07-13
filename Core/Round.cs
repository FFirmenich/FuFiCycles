using System.Collections.Generic;
using static Fusee.FuFiCycles.Core.GameSettings;

namespace Fusee.FuFiCycles.Core {
	/// <summary>
	///  One round of the game
	/// </summary>
	public class Round {
		private int id;
		private byte[,] mapMirror;
		public bool firstFrame = true;
		private List<Player> players = new List<Player>();

		public Round(int id) {
			this.id = id;
			mapMirror = new byte[MAP_SIZE, MAP_SIZE];
			addPlayers();
		}

		public void tick() {
			// set first frame false
			if (getFirstFrame()) {
				setNotFirstFrame();
			}
			// fast check if there is an uncollided cycle in this round
			if (allCyclesCollided()) {
				INSTANCE.newRound();
			}
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
		public byte[,] getMapMirror() {
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
		private void setNotFirstFrame() {
			this.firstFrame = false;
		}
		/// <summary>
		/// Add players to the list
		/// </summary>
		private void addPlayers() {
			for (int i = 0; i < PLAYER_QUANTITY; i++) {
				players.Add(new Player((byte) (i + 1)));
			}
		}
		/// <summary>
		/// Get all players
		/// </summary>
		/// <returns>List of all players</returns>
		public List<Player> getPlayers() {
			return this.players;
		}
		/// <summary>
		/// True if all cycles are collided
		/// </summary>
		/// <returns></returns>
		private bool allCyclesCollided() {
			for (int i = 0; i < getPlayers().Count; i++) {
				if (!getPlayers()[i].getCycle().isCollided()) {
					return false;
				}
			}
			return true;
		}
		/// <summary>
		/// set all vars to null to free some memory
		/// </summary>
		public void nullVars() {
			mapMirror = null;
			players = null;
		}
	}
}
