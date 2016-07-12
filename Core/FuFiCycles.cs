using System.Collections.Generic;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using System.Linq;
using Fusee.Xene;
using static Fusee.FuFiCycles.Core.GameSettings;

namespace Fusee.FuFiCycles.Core {
	[FuseeApplication(Name = "FuFiCycles", Description = "A FuFi Production", Width = 1920, Height = 1080)]
	public class FuFiCycles : RenderCanvas {
		// Keyboard Keys
		public KeyboardKeys keyboardKeys;

		// GUI
		private GUI _gui;

		// playerlist
		private List<Player> players = new List<Player>();

		// scene containers
		private Dictionary<string, SceneContainer> sceneContainers = new Dictionary<string, SceneContainer>();

		// vars for Rendering
		private Renderer _renderer;
		public float _angleVert = -M.PiOver6 * 0.2f, _angleVelVert, _angleRoll, _angleRollInit, _zoom;
		private static float2 _offset;
		private static float2 _offsetInit;
		public const float RotationSpeed = 7;
		private const float Damping = 8f;
		public bool _firstFrame = true;

		// Init is called on startup. 
		public override void Init() {
			INSTANCE = this;

			// Init Keyboard
			keyboardKeys = new KeyboardKeys();

			// Init GUI
			_gui = new GUI();

			addSceneContainers();
			setMapSize();

			newRound();

			// Instantiate our self-written renderer
			_renderer = new Renderer();

			// Add players to the list
			for (int i = 0; i < PLAYER_QUANTITY; i++) {
				players.Add(new Player(i+1));
			}

			// remove original cycle from cycle scene
			sceneContainers["cycle"].Children.Remove(sceneContainers["cycle"].Children.FindNodes(c => c.Name == "cycle").First());

			// Set the clear color for the backbuffer
			RC.ClearColor = new float4(1, 1, 1, 1);
		}

		public override void RenderAFrame() {
			// Clear the backbuffer
			RC.Clear(ClearFlags.Color | ClearFlags.Depth);

			// refresh Keyboard Inputs
			keyboardKeys.renderAFrame();
			
			var curDamp = (float)System.Math.Exp(0.1f);

			// zoom
			_zoom = 150;

			_angleVert += _angleVelVert;
			// Limit pitch to the range between [-PI/2, + PI/2]
			_angleVert = M.Clamp(_angleVert, -M.PiOver2, M.PiOver2);

			// Wrap-around to keep _angleRoll between -PI and + PI
			_angleRoll = M.MinAngle(_angleRoll);

			renderPlayers();
			renderMiniMap();

			// render gui
			_gui.getGUIHandler().RenderGUI();

			// Swap buffers: Show the contents of the backbuffer (containing the currently rerndered frame) on the front buffer.
			Present();

			// after first frame set _firstFrame var false
			if (_firstFrame) {
				_firstFrame = false;
			}
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
			for (int i = 0; i < players.Count; i++) {
				players[i].resize();
			}
			// refresh GUI
			_gui.getGUIHandler().Refresh();
		}

		/// <summary>
		///  Inits all variables for a new round
		/// </summary>
		public void newRound() {
			int newRoundId = ROUNDS.Count + 1;
			ROUNDS.Add(new Round(newRoundId));
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
			for (int i = 0; i < players.Count; i++) {
				players[i].renderView(_renderer);
			}
			_renderer.Traverse(sceneContainers["land"].Children);
		}
		/// <summary>
		///  renders the View for all players
		/// </summary>
		private void renderPlayers() {
			for (int i = 0; i < players.Count; i++) {
				var mtxRot = float4x4.CreateRotationZ(_angleRoll) * float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(players[i]._angleHorz);
				var mtxCam = float4x4.LookAt(0, 20, -_zoom, 0, 0, 0, 0, 1, 0);
				var mtxOffset = float4x4.CreateTranslation(2 * _offset.x / Width, -2 * _offset.y / Height, 0);
				RC.Projection = mtxOffset * players[i].getProjection();
				_renderer.View = mtxCam * mtxRot * SCENE_SCALE * float4x4.CreateTranslation(-(new float3(players[i].getCycle().getPosition().x, players[i].getCycle().getPosition().y, players[i].getCycle().getPosition().z)));
				switch (players[i].getPlayerId()) {
					case 1:
						RC.Viewport(0, 0, (Width / 2), Height);
						break;
					case 2:
						RC.Viewport((Width / 2), 0, (Width / 2), Height);
						break;
					default:
						break;
				}
				players[i].renderAFrame(_renderer);
				_renderer.Traverse(sceneContainers["landLines"].Children);
			}
		}
		/// <summary>
		///  Set the map size according to the boundingbox of the grounds mesh
		/// </summary>
		private void setMapSize() {
			MeshComponent ground = sceneContainers["landLines"].Children.FindNodes(c => c.Name == "Ground").First()?.GetMesh();
			MAP_SIZE = (int)ground.BoundingBox.Size.x;
		}
		/// <summary>
		///  Add SceneContainers to Dictionary sceneContainers
		/// </summary>
		private void addSceneContainers() {
			sceneContainers.Add("land", AssetStorage.Get<SceneContainer>("Land.fus"));
			sceneContainers.Add("landLines", AssetStorage.Get<SceneContainer>("Land_Lines.fus"));
			sceneContainers.Add("cycle", AssetStorage.Get<SceneContainer>("Cycle.fus"));
			sceneContainers.Add("wall", AssetStorage.Get<SceneContainer>("Wall.fus"));
		}
	}
}