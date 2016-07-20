using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static Fusee.FuFiCycles.Core.GameSettings;

namespace Fusee.FuFiCycles.Core {
	public class Match {
		private byte id;
		private List<Player> players = new List<Player>();
		private List<Round> rounds = new List<Round>();
		private byte winner;
		private bool ended = false;

		public Match() {
			id = (byte) (INSTANCE.getMatchs().Count() + 1);
			if(getId() == 1) {
				INSTANCE.addSceneContainers();
				INSTANCE.setMapSize();
				INSTANCE.hideOriginalCycle();
			}
			addPlayers();
			newRound();
		}
		public void tick() {
			getLastRound().tick();
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
			if(ended) {
				return;
			}
			if(getRounds().Count >= 3) {
				end();
				return;
			}
			if (getRounds() != null) {
				if (getRounds().Any()) {
					if (getLastRound() != null) {
						getLastRound().nullVars();
					}
				}
			}
			removePlayers();
			addPlayers();
			GC.Collect();
			getRounds().Add(new Round());
			INSTANCE.Resize();
		}
		public void nullVars() {
			for(int i = 0; i < getRounds().Count(); i++) {
				getRounds().ElementAt(i).nullVars();
			}
			players = null;
			rounds = null;
		}
		private void end() {
			ended = true;
			getLastRound().pause();
			// define winner
			byte wins1 = 0;
			byte wins2 = 0;
			for(int i = 0; i < getRounds().Count(); i++) {
				if(getRounds()[i].getWinner() == 1) {
					wins1++;
				} else if(getRounds()[i].getWinner() == 2) {
					wins2++;
				}
			}
			if(wins1 > wins2) {
				winner = 1;
			} else {
				winner = 2;
			}
			INSTANCE.getIngameGui().addWinner();
			INSTANCE.getMenuGui().deActivateContinueButton();
			for (int i = 0; i < getPlayers().Count(); i++) {
				getPlayers()[i].getCycle().setSpeed(0);
			}
		}
		public bool isEnded() {
			return ended;
		}
		public int getWinner() {
			return winner;
		}
		public List<Round> getRounds() {
			return rounds;
		}
		public Round getLastRound() {
			return getRounds()[getRounds().Count() - 1];
		}
	}
}
