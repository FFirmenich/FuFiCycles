using System;
using Fusee.Math.Core;
using Fusee.Engine.Core.GUI;
using Fusee.Engine.Core;
using Fusee.Base.Core;
using System.Diagnostics;
using static Fusee.FuFiCycles.Core.GameSettings;
using Fusee.Base.Common;
using Fusee.Engine.Common;

namespace Fusee.FuFiCycles.Core {
	public class GUIMenu {
		public GUIHandler guiHandler;

		public GUIPanel panel;

		public GUIButton continueButton;
		public GUIButton newMatchButton;
		public GUIButton exitButton;
		private GUIText _guiSubText;
		private GUIText _guiPointsText;
		private GUIText headline;
		private GUIImage _guiBackground;

		public float score = 0;

		//colors
		private static float4 grey = new float4(0.30196078431372549019607843137255f, 0.30196078431372549019607843137255f, 0.30196078431372549019607843137255f, 1);
		private static float4 panelcolor = new float4(grey.x, grey.y, grey.z, 0.6f);
		private static float4 buttonColor = new float4(0, 0.6f, 1, 0.9f);
		private static float4 buttonColorHover = new float4(0, 0.6f, 1, 1);


		public GUIMenu() {
			guiHandler = new GUIHandler();
			guiHandler.AttachToContext(INSTANCE.getRC());
			INSTANCE.getRC().Viewport(0, 0, INSTANCE.Width, INSTANCE.Height);

			// image
			guiHandler.Add(new GUIImage(AssetStorage.Get<ImageData>("MenuBackground.jpg"), 0, 0, -5, INSTANCE.Width, INSTANCE.Height));

			// panel
			int paddingV = INSTANCE.Width / 10;
			int paddingH = INSTANCE.Height / 10;
			int panelWidth = (INSTANCE.Width - (paddingV * 2));
			int panelHeight = (INSTANCE.Height - (paddingH * 2));
			int panelCenterH = (INSTANCE.Width - (paddingV * 2)) / 2;
			panel = new GUIPanel("PAUSE", new FontMap(INSTANCE.roboto, INSTANCE.roboto.PixelHeight), paddingV, paddingH, panelWidth, panelHeight);
			panel.PanelColor = panelcolor;
			guiHandler.Add(panel);

			// add continue button
			continueButton = new GUIButton("CONTINUE", new FontMap(INSTANCE.roboto, INSTANCE.roboto.PixelHeight), panelCenterH - panelWidth / 4, (panel.ChildElements.Count * 200) + 250, panelWidth / 2, 100);
			continueButton.ButtonColor = buttonColor;
			continueButton.BorderColor = grey;
			continueButton.BorderWidth = 2;
			continueButton.OnGUIButtonEnter += continueButton_OnGUIButtonEnter;
			continueButton.OnGUIButtonLeave += continueButton_OnGUIButtonLeave;
			continueButton.OnGUIButtonDown += continueButton_OnGUIButtonDown;
			panel.ChildElements.Add(continueButton);
			guiHandler.Refresh();

			// add new match button
			newMatchButton = new GUIButton("NEW MATCH", new FontMap(INSTANCE.roboto, INSTANCE.roboto.PixelHeight), panelCenterH - panelWidth / 4, (panel.ChildElements.Count * 200) + 250, panelWidth / 2, 100);
			newMatchButton.ButtonColor = buttonColor;
			newMatchButton.BorderColor = grey;
			newMatchButton.BorderWidth = 2;
			newMatchButton.OnGUIButtonEnter += newMatchButton_OnGUIButtonEnter;
			newMatchButton.OnGUIButtonLeave += newMatchButton_OnGUIButtonLeave;
			newMatchButton.OnGUIButtonDown += newMatchButton_OnGUIButtonDown;
			panel.ChildElements.Add(newMatchButton);
			guiHandler.Refresh();

			// add exit button
			exitButton = new GUIButton("EXIT", new FontMap(INSTANCE.roboto, INSTANCE.roboto.PixelHeight), panelCenterH - panelWidth / 4, (panel.ChildElements.Count * 200) + 250, panelWidth / 2, 100);
			exitButton.ButtonColor = buttonColor;
			exitButton.BorderColor = grey;
			exitButton.BorderWidth = 2;
			exitButton.OnGUIButtonEnter += exitButton_OnGUIButtonEnter;
			exitButton.OnGUIButtonLeave += exitButton_OnGUIButtonLeave;
			exitButton.OnGUIButtonDown += exitButton_OnGUIButtonDown;
			panel.ChildElements.Add(exitButton);
			guiHandler.Refresh();
		}

		public void continueButton_OnGUIButtonEnter(GUIButton sender, GUIButtonEventArgs mea) {
			INSTANCE.SetCursor(CursorType.Hand);
			continueButton.ButtonColor = buttonColorHover;
		}

		public void continueButton_OnGUIButtonLeave(GUIButton sender, GUIButtonEventArgs mea) {
			INSTANCE.SetCursor(CursorType.Standard);
			continueButton.ButtonColor = buttonColor;
		}

		void continueButton_OnGUIButtonDown(GUIButton sender, GUIButtonEventArgs mea) {
			INSTANCE.SetCursor(CursorType.Standard);
			GameSettings.SHOWMENU = false;
			INSTANCE.Resize();
		}

		public void newMatchButton_OnGUIButtonEnter(GUIButton sender, GUIButtonEventArgs mea) {
			INSTANCE.SetCursor(CursorType.Hand);
			newMatchButton.ButtonColor = buttonColorHover;
		}

		public void newMatchButton_OnGUIButtonLeave(GUIButton sender, GUIButtonEventArgs mea) {
			INSTANCE.SetCursor(CursorType.Standard);
			newMatchButton.ButtonColor = buttonColor;
		}

		void newMatchButton_OnGUIButtonDown(GUIButton sender, GUIButtonEventArgs mea) {
			INSTANCE.SetCursor(CursorType.Standard);
			GameSettings.SHOWMENU = false;
			INSTANCE.Resize();
		}

		public void exitButton_OnGUIButtonEnter(GUIButton sender, GUIButtonEventArgs mea) {
			INSTANCE.SetCursor(CursorType.Hand);
			exitButton.ButtonColor = buttonColorHover;
		}

		public void exitButton_OnGUIButtonLeave(GUIButton sender, GUIButtonEventArgs mea) {
			INSTANCE.SetCursor(CursorType.Standard);
			exitButton.ButtonColor = buttonColor;
		}

		void exitButton_OnGUIButtonDown(GUIButton sender, GUIButtonEventArgs mea) {
			INSTANCE.SetCursor(CursorType.Standard);
			INSTANCE.CloseGameWindow();
		}

		internal void RenderGUI() {
			throw new NotImplementedException();
		}

		internal void Clear() {
			throw new NotImplementedException();
		}

		public GUIHandler getGUIHandler() {
			return this.guiHandler;
		}
	}
}
