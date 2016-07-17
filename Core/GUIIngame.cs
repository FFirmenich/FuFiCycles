using System;
using Fusee.Math.Core;
using Fusee.Engine.Core.GUI;
using Fusee.Engine.Core;
using Fusee.Base.Core;
using System.Diagnostics;
using static Fusee.FuFiCycles.Core.GameSettings;
using System.Linq;

namespace Fusee.FuFiCycles.Core {
	public class GUIIngame {
		public GUIHandler guiHandler;

		public GUIButton menuButton;
		// private GUIImage _guiFuseeLogo;
		public FontMap fontMap1;
		private GUIText _guiSubText;
		private GUIText _guiPointsText;
		private GUIText scorePlayer1;
		private GUIText scorePlayer2;
		private GUIText tps;
		private GUIText timer1;
		private GUIText timer2;
		private GUIImage _guiBackground;

		public float score = 0;

		//colors
		private static float4 white = new float4(1, 1, 1, 1);


		public GUIIngame() {
			guiHandler = new GUIHandler();
			guiHandler.AttachToContext(INSTANCE.getRC());

			INSTANCE.roboto.UseKerning = true;
			fontMap1 = new FontMap(INSTANCE.roboto, 20);

			//scorePlayer1 = new GUIText("SCORE: 0", fontMap1, 350, 50);
			//guiHandler.Add(scorePlayer1);

			//scorePlayer2 = new GUIText("SCORE: 0", fontMap1, 350, 50);
			//guiHandler.Add(scorePlayer2);

			tps = new GUIText("TPS: 0", fontMap1, 50, 50);
			tps.TextColor = white;
			guiHandler.Add(tps);
		}

		public void refresh() {
			tps.Text = "TPS: " + MATCHS.Last().getCurrentRound().getTps();
			INSTANCE.getRC().Viewport(0, 0, INSTANCE.Width, INSTANCE.Height);
			if (MATCHS.Last().getCurrentRound().getAbseconds() <= 5) {
				byte cntr = (byte) (6 - MATCHS.Last().getCurrentRound().getAbseconds());
				if (cntr > 5) {
					cntr = 5;
				}
				timer1.Text = (cntr).ToString();
				timer2.Text = (cntr).ToString();
			} else {
				guiHandler.Remove(timer1);
				guiHandler.Remove(timer2);
			}
			guiHandler.Refresh();
		}

		public void addTimers() {
			timer1 = new GUIText("3", fontMap1, (INSTANCE.Width / 4) - 5, (INSTANCE.Height / 2) - 150);
			timer1.TextColor = white;
			guiHandler.Add(timer1);

			timer2 = new GUIText("3", fontMap1, ((INSTANCE.Width / 4) * 3) - 5, (INSTANCE.Height / 2) - 150);
			timer2.TextColor = white;
			guiHandler.Add(timer2);
		}

		internal void RenderGUI() {
			throw new NotImplementedException();
		}

		public void AddPointsToScore() {
			scorePlayer1.Text = "SCORE: " + score;
			guiHandler.Refresh();
		}

		internal void Clear() {
			throw new NotImplementedException();
		}

		public GUIHandler getGUIHandler() {
			return this.guiHandler;
		}
	}
}
