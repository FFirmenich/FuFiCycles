using System;
using System.Linq;
using static System.DateTime;
using static Fusee.FuFiCycles.Core.GameSettings;
using System.Diagnostics;

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
		private int second = 0;
		public byte secondsToWait = 3;
		// web
		private Stopwatch stopWatch = new Stopwatch();

		// winner
		private byte winner;

		public Round() {
			id = 0;
			if (INSTANCE.getMatchs() != null) {
				if (INSTANCE.getMatchs().Count > 0) {
					if (INSTANCE.getLastMatch() != null) {
						if(INSTANCE.getLastMatch().getRounds() != null) {
							id = (byte)INSTANCE.getLastMatch().getRounds().Count();
						}
					}
				}
			}
			if (WEB) {
				stopWatch.Start();
			}
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
				for(int i = 0; i < INSTANCE.getLastMatch().getPlayers().Count; i++) {
					if (!INSTANCE.getLastMatch().getPlayers()[i].gotFirstWall()) {
						gotAllWalls = false;
					}
					if(gotAllWalls) {
						setNotFirstFrame();
					}
				}
			}
			// fast check if there is an uncollided cycle in this round
			if (onlyOneCycleLeft()) {
				INSTANCE.getLastMatch().newRound();
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
		private bool onlyOneCycleLeft() {
			byte cyclesLeft = (byte) INSTANCE.getLastMatch().getPlayers().Count();
			for (int i = 0; i < INSTANCE.getLastMatch().getPlayers().Count; i++) {
				if (INSTANCE.getLastMatch().getPlayers()[i].getCycle().isCollided()) {
					cyclesLeft--;
				}
			}
			if(cyclesLeft <= 1) {
				for (int i = 0; i < INSTANCE.getLastMatch().getPlayers().Count; i++) {
					if (!INSTANCE.getLastMatch().getPlayers()[i].getCycle().isCollided()) {
						setWinner((INSTANCE.getLastMatch().getPlayers()[i].getPlayerId()));
					}
				}
				return true;
			}
			return false;
		}
		/// <summary>
		/// set all vars to null to free some memory and remove cycles and walls from the map
		/// </summary>
		public void nullVars() {
			INSTANCE.getSceneContainers()["wall"].Children.RemoveRange(1, INSTANCE.getSceneContainers()["wall"].Children.Count - 1);
			// set variables null
			mapMirror = null;
			if(WEB) {
				stopWatch.Stop();
				stopWatch = null;
			}
		}
		/// <summary>
		/// Calculates the Ticks per Second
		/// </summary>
		private void calcTps() {
			TimeSpan ts = stopWatch.Elapsed;

			if (WEB) {
				if (stopWatch.Elapsed.Seconds != second) {
					newSecond();
				}
			} else {
				if (UtcNow.TimeOfDay.Seconds != second) {
					newSecond();
				}
			}
			tpsTemp++;
		}
		public int getTps() {
			return tps;
		}
		private void newSecond() {
			if(WEB) {
				second = stopWatch.Elapsed.Seconds;
			} else {
				second = UtcNow.TimeOfDay.Seconds;
			}
			tps = tpsTemp;
			tpsTemp = 0;
			absseconds++;
			if (getAbseconds() > secondsToWait && isPaused()) {
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
		private void setWinner(byte winner_id) {
			winner = winner_id;
		}
		public byte getWinner() {
			return winner;
		}
	}
}