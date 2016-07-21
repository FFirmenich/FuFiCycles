using System;
using Fusee.Math.Core;
using Fusee.Engine.Core.GUI;
using static Fusee.FuFiCycles.Core.GameSettings;
using System.Linq;

namespace Fusee.FuFiCycles.Core {
	public class GUIIngame : GUI {
		public GUIButton menuButton;
		private GUIText scorePlayer1;
		private GUIText scorePlayer2;
		private GUIText tps;
		private GUIText player1name;
		private GUIText player2name;
		private GUIText timer1;
		private GUIText timer2;
		private GUIText winner;

		//colors
		private static float4 white = new float4(1, 1, 1, 1);

		public GUIIngame() {
			tps = new GUIText("TPS: 0", getRoboto(), 50, 50);
			tps.TextColor = white;
			getGUIHandler().Add(tps);
			player1name = new GUIText(PLAYER1_NAME, getRoboto(), INSTANCE.Width * 3 / 4 - 40, 50);
			player1name.TextColor = white;
			getGUIHandler().Add(player1name);
			player2name = new GUIText(PLAYER2_NAME, getRoboto(), INSTANCE.Width / 4 - 40, 50);
			player2name.TextColor = white;
			getGUIHandler().Add(player2name);
		}
		public void refresh() {
			tps.Text = "TPS: " + INSTANCE.getLastMatch().getLastRound().getTps();
			INSTANCE.getRC().Viewport(0, 0, INSTANCE.Width, INSTANCE.Height);
			if (INSTANCE.getLastMatch().getLastRound().getAbseconds() <= INSTANCE.getLastMatch().getLastRound().secondsToWait) {
				byte cntr = (byte) ((INSTANCE.getLastMatch().getLastRound().secondsToWait + 1) - INSTANCE.getLastMatch().getLastRound().getAbseconds());
				if (cntr > INSTANCE.getLastMatch().getLastRound().secondsToWait) {
					cntr = INSTANCE.getLastMatch().getLastRound().secondsToWait;
				}
				timer1.Text = (cntr).ToString();
				timer2.Text = (cntr).ToString();
			} else {
				removeTimers();
			}
			getGUIHandler().Refresh();
		}
		public void addTimers() {
			timer1 = new GUIText("3", getRoboto(), (INSTANCE.Width / 4) - 5, (INSTANCE.Height / 2) - 150);
			timer1.TextColor = white;
			getGUIHandler().Add(timer1);

			timer2 = new GUIText("3", getRoboto(), ((INSTANCE.Width / 4) * 3) - 5, (INSTANCE.Height / 2) - 150);
			timer2.TextColor = white;
			getGUIHandler().Add(timer2);
		}
		public void removeTimers() {
			getGUIHandler().Remove(timer1);
			getGUIHandler().Remove(timer2);
		}
		public void addWinner() {
			if(INSTANCE.getLastMatch().getWinner() == 1) {
				winner = new GUIText("AND THE WINNER IS: " + PLAYER1_NAME, getRoboto(), (INSTANCE.Width / 2) - 130, (INSTANCE.Height / 2) - 150);
			} else if(INSTANCE.getLastMatch().getWinner() == 2) {
				winner = new GUIText("AND THE WINNER IS: " + PLAYER2_NAME, getRoboto(), (INSTANCE.Width / 2) - 130, (INSTANCE.Height / 2) - 150);
			}
			winner.TextColor = white;
			getGUIHandler().Add(winner);
			getGUIHandler().Refresh();
		}
		public void removeWinner() {
			getGUIHandler().Remove(winner);
		}
		internal void RenderGUI() {
			throw new NotImplementedException();
		}
		public void refreshNames() {
			player1name.Text = PLAYER1_NAME;
			player2name.Text = PLAYER2_NAME;
			getGUIHandler().Refresh();
		}
		internal void Clear() {
			throw new NotImplementedException();
		}
	}
}
