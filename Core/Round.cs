using Fusee.Xene;
using System;
using System.Collections.Generic;
using System.Linq;
using static Fusee.FuFiCycles.Core.GameSettings;

namespace Fusee.FuFiCycles.Core {
	/// <summary>
	///  One round of the game
	/// </summary>
	public class Round {
		private byte id;
		private byte[,] mapMirror;
		public bool firstFrame = true;
		private bool paused = true;

		// vars for tps
		private int absseconds = 0;
		private ushort tpsTemp = 0;
		private ushort tps = 0;
		private int second = DateTime.Now.TimeOfDay.Seconds;

		public Round() {
			this.id = (byte)MATCHS.Count();
			mapMirror = new byte[MAP_SIZE, MAP_SIZE];
			INSTANCE.getIngameGui().addTimers();
		}

		public void tick() {
			calcTps();
			if (isPaused()) {
				return;
			}
			// set first frame false
			if (getFirstFrame()) {
				bool gotAllWalls = true;
				for(int i = 0; i < MATCHS.Last().getPlayers().Count; i++) {
					if (!MATCHS.Last().getPlayers()[i].gotFirstWall()) {
						gotAllWalls = false;
					}
					if(gotAllWalls) {
						setNotFirstFrame();
					}
				}
			}
			// fast check if there is an uncollided cycle in this round
			if (allCyclesCollided()) {
				MATCHS.Last().newRound();
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
		/// True if all cycles are collided
		/// </summary>
		/// <returns></returns>
		private bool allCyclesCollided() {
			for (int i = 0; i < MATCHS.Last().getPlayers().Count; i++) {
				if (!MATCHS.Last().getPlayers()[i].getCycle().isCollided()) {
					return false;
				}
			}
			return true;
		}
		/// <summary>
		/// set all vars to null to free some memory and remove cycles and walls from the map
		/// </summary>
		public void nullVars() {
			INSTANCE.getSceneContainers()["wall"].Children.RemoveRange(1, INSTANCE.getSceneContainers()["wall"].Children.Count - 1);
			// set variables null
			mapMirror = null;
		}
		/// <summary>
		/// Calculates the Ticks per Second
		/// </summary>
		private void calcTps() {
			if (DateTime.Now.TimeOfDay.Seconds != second) {
				newSecond();
			}
			tpsTemp++;
		}
		public int getTps() {
			return tps;
		}
		private void newSecond() {
			second = DateTime.Now.TimeOfDay.Seconds;
			tps = tpsTemp;
			tpsTemp = 0;
			absseconds++;
			if (getAbseconds() > 5 && isPaused()) {
				unpause();
			}
		}
		public int getAbseconds() {
			return absseconds;
		}
		public bool isPaused() {
			return paused;
		}
		public void pause() {
			paused = true;
		}
		public void unpause() {
			paused = false;
		}
	}
}