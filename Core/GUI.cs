using System;
using Fusee.Math.Core;
using Fusee.Engine.Core.GUI;
using Fusee.Engine.Core;
using Fusee.Base.Core;
using System.Diagnostics;
using static Fusee.FuFiCycles.Core.GameSettings;

namespace Fusee.FuFiCycles.Core {
	public class GUI {
		public GUIHandler guiHandler;

		public GUIButton _guiFuseeLink;
		// private GUIImage _guiFuseeLogo;
		public FontMap fontMap1;
		private GUIText _guiSubText;
		private GUIText _guiPointsText;
		private GUIText _guiPoints;
		private GUIImage _guiBackground;

		public float score = 0;


		public GUI() {
			//guihandler
			guiHandler = new GUIHandler();
			guiHandler.AttachToContext(INSTANCE.getRC());


			var font1 = AssetStorage.Get<Font>("Roboto-Light.ttf");
			font1.UseKerning = true;
			fontMap1 = new FontMap(font1, 20);

			//_guiBackground = new GUIImage(AssetStorage.Get<ImageData>("himmel.jpg"), -30, -300, -5, 1500, 1000);
			//_guiHandler.Add(_guiBackground);


			_guiPoints = new GUIText("SCORE: 0", fontMap1, 350, 50);
			guiHandler.Add(_guiPoints);

		}

		public void _guiFuseeLink_OnGUIButtonLeave(GUIButton sender, GUIButtonEventArgs mea) {
			_guiFuseeLink.ButtonColor = new float4(0, 0.6f, 0.2f, 0.4f);
			_guiFuseeLink.BorderWidth = 0;
		}

		public void _guiFuseeLink_OnGUIButtonEnter(GUIButton sender, GUIButtonEventArgs mea) {
			_guiFuseeLink.ButtonColor = new float4(200, 100, 100, 130);
			_guiFuseeLink.BorderWidth = 1;
		}

		void _guiFuseeLink_OnGUIButtonDown(GUIButton sender, GUIButtonEventArgs mea) {
			Debug.WriteLine("Button wurde geklickt");
			// TODO: Restart game
			_guiFuseeLink.PosX = -300;
			//OpenLink("http://fusee3d.org");
		}

		/*  private void OpenLink(string v)
		  {
			  throw new NotImplementedException();
		  }*/

		internal void RenderGUI() {
			throw new NotImplementedException();
		}

		public void AddPointsToScore() {
			_guiPoints.Text = "SCORE: " + score;
			guiHandler.Refresh();
		}


		public void RestartButton() {
			_guiFuseeLink = new GUIButton("RESET", fontMap1, 570, 200, 157, 87);
			_guiFuseeLink.ButtonColor = new float4(0, 0.6f, 0.2f, 0.4f);
			_guiFuseeLink.BorderColor = new float4(0, 0.6f, 0.2f, 1);
			_guiFuseeLink.BorderWidth = 0;
			_guiFuseeLink.OnGUIButtonDown += _guiFuseeLink_OnGUIButtonDown;
			_guiFuseeLink.OnGUIButtonEnter += _guiFuseeLink_OnGUIButtonEnter;
			_guiFuseeLink.OnGUIButtonLeave += _guiFuseeLink_OnGUIButtonLeave;
			guiHandler.Add(_guiFuseeLink);
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
