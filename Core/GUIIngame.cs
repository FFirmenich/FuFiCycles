using System;
using Fusee.Math.Core;
using Fusee.Engine.Core.GUI;
using Fusee.Engine.Core;
using Fusee.Base.Core;
using System.Diagnostics;
using static Fusee.FuFiCycles.Core.GameSettings;

namespace Fusee.FuFiCycles.Core {
	public class GUIIngame {
		public GUIHandler guiHandler;

		public GUIButton menuButton;
		// private GUIImage _guiFuseeLogo;
		public FontMap fontMap1;
		private GUIText _guiSubText;
		private GUIText _guiPointsText;
		private GUIText _guiPoints;
		private GUIImage _guiBackground;

		public float score = 0;


		public GUIIngame() {
			guiHandler = new GUIHandler();
			guiHandler.AttachToContext(INSTANCE.getRC());

			INSTANCE.roboto.UseKerning = true;
			fontMap1 = new FontMap(INSTANCE.roboto, 20);

			_guiPoints = new GUIText("SCORE: 0", fontMap1, 350, 50);
			guiHandler.Add(_guiPoints);

			addMenuButton();
		}

		public void menuButton_OnGUIButtonLeave(GUIButton sender, GUIButtonEventArgs mea) {
			menuButton.ButtonColor = new float4(0, 0.6f, 0.2f, 0.4f);
			menuButton.BorderWidth = 0;
		}

		public void menuButton_OnGUIButtonEnter(GUIButton sender, GUIButtonEventArgs mea) {
			menuButton.ButtonColor = new float4(200, 100, 100, 130);
			menuButton.BorderWidth = 1;
		}

		void menuButton_OnGUIButtonDown(GUIButton sender, GUIButtonEventArgs mea) {
			Debug.WriteLine("Button wurde geklickt");
			menuButton.PosX = -300;
		}

		internal void RenderGUI() {
			throw new NotImplementedException();
		}

		public void AddPointsToScore() {
			_guiPoints.Text = "SCORE: " + score;
			guiHandler.Refresh();
		}


		public void addMenuButton() {
			menuButton = new GUIButton("MENU", fontMap1, 0, 0, 200, 100);
			menuButton.ButtonColor = new float4(0, 0.6f, 0.2f, 0.4f);
			menuButton.BorderColor = new float4(0, 0.6f, 0.2f, 1);
			menuButton.BorderWidth = 2;
			menuButton.OnGUIButtonDown += menuButton_OnGUIButtonDown;
			menuButton.OnGUIButtonEnter += menuButton_OnGUIButtonEnter;
			menuButton.OnGUIButtonLeave += menuButton_OnGUIButtonLeave;
			guiHandler.Add(menuButton);
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
