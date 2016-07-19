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
		private GUIText timer1;
		private GUIText timer2;
		private GUIText winner;

		public float score = 0;

		//colors
		private static float4 white = new float4(1, 1, 1, 1);


		public GUIIngame() {
			tps = new GUIText("TPS: 0", getRoboto(), 50, 50);
			tps.TextColor = white;
			getGUIHandler().Add(tps);
		}

		public void refresh() {
			tps.Text = "TPS: " + MATCHS.Last().getRounds().Last().getTps();
			INSTANCE.getRC().Viewport(0, 0, INSTANCE.Width, INSTANCE.Height);
			if (MATCHS.Last().getRounds().Last().getAbseconds() <= MATCHS.Last().getRounds().Last().secondsToWait) {
				byte cntr = (byte) ((MATCHS.Last().getRounds().Last().secondsToWait + 1) - MATCHS.Last().getRounds().Last().getAbseconds());
				if (cntr > MATCHS.Last().getRounds().Last().secondsToWait) {
					cntr = MATCHS.Last().getRounds().Last().secondsToWait;
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
			winner = new GUIText("AND THE WINNER IS: PLAYER " + MATCHS.Last().getWinner(), getRoboto(), (INSTANCE.Width / 2) - 130, (INSTANCE.Height / 2) - 150);
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

		public void AddPointsToScore() {
			scorePlayer1.Text = "SCORE: " + score;
			getGUIHandler().Refresh();
		}

		internal void Clear() {
			throw new NotImplementedException();
		}
	}
}
