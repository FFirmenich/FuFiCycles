using System;
using Fusee.Math.Core;
using Fusee.Engine.Core.GUI;
using Fusee.Base.Core;
using static Fusee.FuFiCycles.Core.GameSettings;
using Fusee.Base.Common;
using Fusee.Engine.Common;

namespace Fusee.FuFiCycles.Core {
	public class GUIMenu : GUI {
		public GUIPanel panel;

		public GUIButton continueButton;
		public GUIButton newMatchButton;
		public GUIButton exitButton;
		private GUIText _guiSubText;
		private GUIText _guiPointsText;
		private GUIText headline;
		private GUIImage _guiBackground;

		public float score = 0;

		private bool continueButtonActive = false;

		//colors
		private static float4 grey = new float4(0.30196078431372549019607843137255f, 0.30196078431372549019607843137255f, 0.30196078431372549019607843137255f, 1);
		private static float4 panelcolor = new float4(grey.x, grey.y, grey.z, 0.6f);
		private static float4 buttonColor = new float4(0, 0.6f, 1, 0.9f);
		private static float4 buttonColorHover = new float4(0, 0.6f, 1, 1);
		private static float4 buttonColorDeactivated = new float4(grey.x, grey.y, grey.z, 0.8f);

		//paneldimens
		private static int paddingV = INSTANCE.Width / 10;
		private static int paddingH = INSTANCE.Height / 10;
		private static int panelWidth = (INSTANCE.Width - (paddingV * 2));
		private static int panelHeight = (INSTANCE.Height - (paddingH * 2));
		private static int panelCenterH = (INSTANCE.Width - (paddingV * 2)) / 2;


		public GUIMenu() {
			// image
			getGUIHandler().Add(new GUIImage(AssetStorage.Get<ImageData>("MenuBackground.jpg"), 0, 0, -5, INSTANCE.Width, INSTANCE.Height));

			// panel
			panel = new GUIPanel("PAUSE", getRoboto(), paddingV, paddingH, panelWidth, panelHeight);
			panel.PanelColor = panelcolor;
			getGUIHandler().Add(panel);

			addContinueButton();
			addNewMatchButton();
			addExitButton();
		}
		public void setViewport() {
			INSTANCE.getRC().Viewport(0, 0, INSTANCE.Width, INSTANCE.Height);
		}
		public void addContinueButton() {
			continueButton = new GUIButton("CONTINUE", getRoboto(), panelCenterH - panelWidth / 4, (panel.ChildElements.Count * 200) + 250, panelWidth / 2, 100);
			continueButton.ButtonColor = buttonColorDeactivated;
			continueButton.BorderColor = grey;
			continueButton.BorderWidth = 2;
			continueButton.OnGUIButtonEnter += continueButton_OnGUIButtonEnter;
			continueButton.OnGUIButtonLeave += continueButton_OnGUIButtonLeave;
			continueButton.OnGUIButtonDown += continueButton_OnGUIButtonDown;
			panel.ChildElements.Add(continueButton);
			getGUIHandler().Refresh();
		}
		public void activateContinueButton() {
			continueButton.ButtonColor = buttonColor;
			continueButtonActive = true;
		}
		public void deActivateContinueButton() {
			continueButton.ButtonColor = buttonColorDeactivated;
			continueButtonActive = false;
		}
		private void addNewMatchButton() {
			newMatchButton = new GUIButton("NEW MATCH", getRoboto(), panelCenterH - panelWidth / 4, (panel.ChildElements.Count * 200) + 250, panelWidth / 2, 100);
			newMatchButton.ButtonColor = buttonColor;
			newMatchButton.BorderColor = grey;
			newMatchButton.BorderWidth = 2;
			newMatchButton.OnGUIButtonEnter += newMatchButton_OnGUIButtonEnter;
			newMatchButton.OnGUIButtonLeave += newMatchButton_OnGUIButtonLeave;
			newMatchButton.OnGUIButtonDown += newMatchButton_OnGUIButtonDown;
			panel.ChildElements.Add(newMatchButton);
			getGUIHandler().Refresh();
		}
		private void addExitButton() {
			if(!WEB) {
				exitButton = new GUIButton("EXIT", getRoboto(), panelCenterH - panelWidth / 4, (panel.ChildElements.Count * 200) + 250, panelWidth / 2, 100);
				exitButton.ButtonColor = buttonColor;
				exitButton.BorderColor = grey;
				exitButton.BorderWidth = 2;
				exitButton.OnGUIButtonEnter += exitButton_OnGUIButtonEnter;
				exitButton.OnGUIButtonLeave += exitButton_OnGUIButtonLeave;
				exitButton.OnGUIButtonDown += exitButton_OnGUIButtonDown;
				panel.ChildElements.Add(exitButton);
				getGUIHandler().Refresh();
			}
		}
		public void continueButton_OnGUIButtonEnter(GUIButton sender, GUIButtonEventArgs mea) {
			if(!continueButtonActive) {
				return;
			}
			INSTANCE.SetCursor(CursorType.Hand);
			continueButton.ButtonColor = buttonColorHover;
		}
		public void continueButton_OnGUIButtonLeave(GUIButton sender, GUIButtonEventArgs mea) {
			if (!continueButtonActive) {
				return;
			}
			INSTANCE.SetCursor(CursorType.Standard);
			continueButton.ButtonColor = buttonColor;
		}
		public void continueButton_OnGUIButtonDown(GUIButton sender, GUIButtonEventArgs mea) {
			if (!continueButtonActive) {
				return;
			}
			INSTANCE.SetCursor(CursorType.Standard);
			SHOWMENU = false;
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
		public void newMatchButton_OnGUIButtonDown(GUIButton sender, GUIButtonEventArgs mea) {
			INSTANCE.SetCursor(CursorType.Standard);
			INSTANCE.getIngameGui().removeWinner();
			INSTANCE.getIngameGui().removeTimers();
			INSTANCE.newMatch();
			SHOWMENU = false;
			activateContinueButton();
			INSTANCE.Resize();
		}
		public void exitButton_OnGUIButtonEnter(GUIButton sender, GUIButtonEventArgs mea) {
			if(!WEB) {
				INSTANCE.SetCursor(CursorType.Hand);
				exitButton.ButtonColor = buttonColorHover;
			}
		}
		public void exitButton_OnGUIButtonLeave(GUIButton sender, GUIButtonEventArgs mea) {
			if (!WEB) {
				INSTANCE.SetCursor(CursorType.Standard);
				exitButton.ButtonColor = buttonColor;
			}
		}
		public void exitButton_OnGUIButtonDown(GUIButton sender, GUIButtonEventArgs mea) {
			if (!WEB) {
				INSTANCE.SetCursor(CursorType.Standard);
				INSTANCE.CloseGameWindow();
			}
		}
		internal void RenderGUI() {
			throw new NotImplementedException();
		}
		internal void Clear() {
			throw new NotImplementedException();
		}
	}
}
