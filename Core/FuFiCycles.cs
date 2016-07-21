using System.Collections.Generic;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using System.Linq;
using Fusee.Xene;
using static Fusee.FuFiCycles.Core.GameSettings;
using System.Diagnostics;
using System;

namespace Fusee.FuFiCycles.Core {
	[FuseeApplication(Name = "FuFiCycles", Description = "A FuFi Production", Width = 1920, Height = 1080)]
	public class FuFiCycles : RenderCanvas {
		// Matchs
		private List<Match> matchs = new List<Match>();

		// Keyboard Keys
		public KeyboardKeys keyboardKeys;

		// GUI
		public Font roboto = AssetStorage.Get<Font>("Roboto-Light.ttf");
		private GUIIngame ingameGui;
		private GUIMenu menuGui;

		// scene containers
		private Dictionary<string, SceneContainer> sceneContainers = new Dictionary<string, SceneContainer>();

		// vars for Rendering
		private Renderer _renderer;
		public float _angleVert = -M.PiOver6 * 0.2f;
		public float _angleRoll;
		public float _angleRollInit;
		private static float2 _offset;
		private static float2 _offsetInit;

		// Init is called on startup. 
		public override void Init() {
			enterFullscreen();

			INSTANCE = this;

			// Init Keyboard
			keyboardKeys = new KeyboardKeys();

			// Init GUIs
			setIngameGui(new GUIIngame());
			setMenuGui(new GUIMenu());

			// Set _angleRoll once
			_angleRoll = M.MinAngle(_angleRoll);
			
			// Instantiate our self-written renderer
			_renderer = new Renderer();

			// Set the clear color for the backbuffer
			RC.ClearColor = new float4(1, 1, 1, 1);
		}
		public override void RenderAFrame() {
			// Clear the backbuffer
			RC.Clear(ClearFlags.Color | ClearFlags.Depth);

			// refresh Keyboard Inputs
			keyboardKeys.renderAFrame();

			if (SHOWMENU) {
				tickMenu();
			} else {
				tickIngame();
			}

			Present();
		}
		public static float NormRot(float rot) {
			while (rot > M.Pi)
				rot -= M.TwoPi;
			while (rot < -M.Pi)
				rot += M.TwoPi;
			return rot;
		}
		public RenderContext getRC() {
			return RC;
		}
		public Dictionary<string, SceneContainer> getSceneContainers() {
			return this.sceneContainers;
		}
		/// <summary>
		///  Is called when the window was resized
		/// </summary>
		public override void Resize() {
			if (SHOWMENU) {
				getMenuGui().getGUIHandler().Refresh();
			} else {
				if (getMatchs() != null && getMatchs().Any()) {
					if (getLastMatch() != null) {
						if (getLastMatch().getPlayers() != null && getLastMatch().getPlayers().Any()) {
							for (int i = 0; i < getLastMatch().getPlayers().Count(); i++) {
								getLastMatch().getPlayers()[i].resize();
							}
						}
					}
				}
				if (getLastMatch().getPlayers() != null) {
					for (int i = 0; i < getLastMatch().getPlayers().Count(); i++) {
						getLastMatch().getPlayers()[i].resize();
					}
				}
				getIngameGui().getGUIHandler().Refresh();
			}
		}
		/// <summary>
		///  Inits all variables for a new match
		/// </summary>
		public void newMatch() {
			if (getMatchs() != null) {
				if (getMatchs().Count > 0) {
					if (getLastMatch() != null) {
						getLastMatch().nullVars();
					}
				}
			}
			getMatchs().Add(new Match());
		}
		/// <summary>
		/// renders the mini map at the top center of the screen
		/// </summary>
		private void renderMiniMap() {
			if (!SHOW_MINIMAP) {
				return;
			}
			RC.Projection = float4x4.CreateOrthographic(MAP_SIZE * 2, MAP_SIZE * 2, 0.01f, 20);
			_renderer.View = float4x4.CreateRotationX(-M.PiOver2) * float4x4.CreateTranslation(0, -10, 0);
			RC.Viewport((Width / 2) - (Width / 4), Height - (Width / 3), Width / 3, Width / 3);
			for (int i = 0; i < getLastMatch().getPlayers().Count; i++) {
				getLastMatch().getPlayers()[i].renderView(_renderer);
			}
			_renderer.Traverse(sceneContainers["land"].Children);
		}
		/// <summary>
		///  renders the View for all players
		/// </summary>
		private void renderPlayers() {
			int zoom = 100;

			for (int i = 0; i < getLastMatch().getPlayers().Count; i++) {
				var mtxRot = float4x4.CreateRotationZ(_angleRoll) * float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(getLastMatch().getPlayers()[i]._angleHorz);
				var mtxCam = float4x4.LookAt(0, 20, -zoom, 0, 0, 0, 0, 1, 0);
				var mtxOffset = float4x4.CreateTranslation(2 * _offset.x / Width, -2 * _offset.y / Height, 0);
				RC.Projection = mtxOffset * getLastMatch().getPlayers()[i].getProjection();
				_renderer.View = mtxCam * mtxRot * SCENE_SCALE * float4x4.CreateTranslation(-(new float3(getLastMatch().getPlayers()[i].getCycle().getPosition().x, getLastMatch().getPlayers()[i].getCycle().getPosition().y, getLastMatch().getPlayers()[i].getCycle().getPosition().z)));
				switch (getLastMatch().getPlayers()[i].getPlayerId()) {
					case 1:
						RC.Viewport((Width / 2), 0, (Width / 2), Height);
						break;
					case 2:
						RC.Viewport(0, 0, (Width / 2), Height);
						break;
					default:
						break;
				}
				getLastMatch().getPlayers()[i].renderAFrame(_renderer);
				_renderer.Traverse(sceneContainers["landLines"].Children);
			}
		}
		/// <summary>
		///  Set the map size according to the boundingbox of the grounds mesh
		/// </summary>
		public void setMapSize() {
			MeshComponent ground = sceneContainers["landLines"].Children.FindNodes(c => c.Name == "Ground").First()?.GetMesh();
			MAP_SIZE = (ushort)ground.BoundingBox.Size.x;
		}
		/// <summary>
		///  Add SceneContainers to Dictionary sceneContainers
		/// </summary>
		public void addSceneContainers() {
			sceneContainers.Add("land", AssetStorage.Get<SceneContainer>("Land.fus"));
			sceneContainers.Add("landLines", AssetStorage.Get<SceneContainer>("Land_Lines.fus"));
			sceneContainers.Add("cycle", AssetStorage.Get<SceneContainer>("Cycle.fus"));
			sceneContainers.Add("wall", AssetStorage.Get<SceneContainer>("Wall.fus"));
		}
		/// <summary>
		/// set view from windowed to fullscreen
		/// </summary>
		private void enterFullscreen() {
			Fullscreen = true;
		}
		/// <summary>
		/// set view from fullscreen to windowed screen
		/// </summary>
		private void exitFullscreen() {
			Fullscreen = false;
		}
		/// <summary>
		/// move the original cycle under the ground to hide it
		/// </summary>
		public void hideOriginalCycle() {
			getSceneContainers()["cycle"].Children.FindNodes(c => c.Name == "cycle").First().GetTransform().Translation.y = -500;
			getSceneContainers()["cycle"].Children.FindNodes(c => c.Name == "cycle").First().GetTransform().Translation.z = MAP_SIZE / 2;
			getSceneContainers()["cycle"].Children.FindNodes(c => c.Name == "cycle").First().GetTransform().Translation.x = MAP_SIZE / 2;
		}
		/// <summary>
		/// Is called to pause the game and show the menu
		/// </summary>
		private void enterMenu() {
			SHOWMENU = true;
			getMenuGui().setViewport();
		}
		public void tickMenu() {
			getMenuGui().getGUIHandler().RenderGUI();

			if (keyboardKeys.keys[KeyCodes.Escape].isPressed()) {
				keyboardKeys.keys[KeyCodes.Escape].setUnpressed();
				//exitFullscreen();
				CloseGameWindow();
			}
			if (keyboardKeys.keys[KeyCodes.C].isPressed()) {
				keyboardKeys.keys[KeyCodes.C].setUnpressed();
				getMenuGui().continueButton_OnGUIButtonDown(null, null);
			}
			if (keyboardKeys.keys[KeyCodes.N].isPressed()) {
				keyboardKeys.keys[KeyCodes.N].setUnpressed();
				getMenuGui().newMatchButton_OnGUIButtonDown(null, null);
			}
		}
		public void tickIngame() {
			renderPlayers();
			renderMiniMap();
			getIngameGui().refresh();
			getIngameGui().getGUIHandler().RenderGUI();
			getLastMatch().tick();

			if (keyboardKeys.keys[KeyCodes.Escape].isPressed()) {
				keyboardKeys.keys[KeyCodes.Escape].setUnpressed();
				enterMenu();
			}
			if (getLastMatch().isEnded() && keyboardKeys.keys[KeyCodes.Enter].isPressed()) {
				keyboardKeys.keys[KeyCodes.Enter].setUnpressed();
				enterMenu();
			}
		}
		public void setIngameGui(GUIIngame ingameGui) {
			this.ingameGui = ingameGui;
		}
		public GUIIngame getIngameGui() {
			return this.ingameGui;
		}
		public void setMenuGui(GUIMenu menuGui) {
			this.menuGui = menuGui;
		}
		public GUIMenu getMenuGui() {
			return this.menuGui;
		}
		public List<Match> getMatchs() {
			return matchs;
		}
		public Match getLastMatch() {
			return getMatchs()[getMatchs().Count() - 1];
		}
	}
}