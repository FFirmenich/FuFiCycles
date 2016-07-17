using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Fusee.Xene;
using static Fusee.FuFiCycles.Core.GameSettings;

namespace Fusee.FuFiCycles.Core {
	public class Match {
		byte id;
		private List<Player> players = new List<Player>();
		private List<Round> rounds = new List<Round>();

		public Match() {
			id = (byte) (MATCHS.Count() + 1);
			if(getId() == 1) {
				INSTANCE.addSceneContainers();
				INSTANCE.setMapSize();
				INSTANCE.hideOriginalCycle();
			}
			addPlayers();
			newRound();
		}

		public void tick() {
			getCurrentRound().tick();
		}

		public int getId() {
			return this.id;
		}
		/// <summary>
		/// Add players to the list
		/// </summary>
		private void addPlayers() {
			for (int i = 0; i < PLAYER_QUANTITY; i++) {
				players.Add(new Player((byte)(i + 1)));
			}
		}
		/// <summary>
		/// Get all players
		/// </summary>
		/// <returns>List of all players</returns>
		public List<Player> getPlayers() {
			return this.players;
		}
		public void removePlayers() {
			for (int i = 0; i < getPlayers().Count; i++) {
				INSTANCE.getSceneContainers()["cycle"].Children.Remove(getPlayers()[i].getCycle().getSNC());
				getPlayers()[i] = null;
			}
			getPlayers().Clear();
		}
		/// <summary>
		///  Inits all variables for a new round
		/// </summary>
		public void newRound() {
			Debug.WriteLine(INSTANCE.getSceneContainers()["cycle"].Children);
			try {
				rounds.Last().nullVars();
			} catch(InvalidOperationException ioe) {
				Debug.WriteLine(ioe.StackTrace);
			}
			removePlayers();
			addPlayers();
			GC.Collect();
			rounds.Add(new Round());
			INSTANCE.Resize();
		}
		public Round getCurrentRound() {
			return rounds.Last();
		}
		public void nullVars() {
			for(int i = 0; i < rounds.Count(); i++) {
				rounds.ElementAt(i).nullVars();
			}
			players = null;
			rounds = null;
		}
	}
}
