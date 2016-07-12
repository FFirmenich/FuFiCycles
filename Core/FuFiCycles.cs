using System.Collections.Generic;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using static Fusee.Engine.Core.Input;
using System.Linq;
using Fusee.Xene;
using System.Diagnostics;

namespace Fusee.FuFiCycles.Core {
	[FuseeApplication(Name = "FuFiCycles", Description = "A FuFi Production", Width = 1920, Height = 1080)]
	public class FuFiCycles : RenderCanvas {
		// Game Settings
		public static bool SHOW_MINIMAP = true;

		// Keyboard Keys
		public KeyboardKeys keyboardKeys;

		// playerlist
		private List<Player> players = new List<Player>();

		// shaders
		private string vertexShader = AssetStorage.Get<string>("VertexShader2.vert");
		private string pixelShader = AssetStorage.Get<string>("PixelShader2.frag");

		// scene containers
		private Dictionary<string, SceneContainer> sceneContainers = new Dictionary<string, SceneContainer>();
		private SceneContainer land = AssetStorage.Get<SceneContainer>("Land.fus");
		private SceneContainer landLines = AssetStorage.Get<SceneContainer>("Land_Lines.fus");
		public float4x4 _sceneScale;
		private SceneContainer _cycle = AssetStorage.Get<SceneContainer>("Cycle.fus");
		private SceneContainer _wall = AssetStorage.Get<SceneContainer>("Wall.fus");

		// cycle positions
		private Dictionary<float3, int> _cyclePositions = new Dictionary<float3, int>();

		// vars for Rendering
		private Renderer _renderer;
		public float _angleVert = -M.PiOver6 * 0.2f, _angleVelVert, _angleRoll, _angleRollInit, _zoom;
		private static float2 _offset;
		private static float2 _offsetInit;
		public const float RotationSpeed = 7;
		private const float Damping = 8f;
		public bool _firstFrame = true;

		private int _mapSize = 0;
		public static float[,] _mapMirror;

		// Init is called on startup. 
		public override void Init() {
			// Init Keyboard
			keyboardKeys = new KeyboardKeys();

			// Add SceneContainers to Dictionary
			sceneContainers.Add("land", land);
			sceneContainers.Add("landLines", landLines);
			sceneContainers.Add("cycle", _cycle);
			sceneContainers.Add("wall", _wall);

			// Set Scene Scale
			_sceneScale = float4x4.CreateScale(0.04f);

			// set Map Size
			//MeshComponent ground = _scene.Children.FindNodes(c => c.Name == "Ground").First()?.GetMesh();
			_mapSize = 16000;
			_mapMirror = new float[_mapSize, _mapSize];

			// Instantiate our self-written renderer
			_renderer = new Renderer(this);

			// Add two player to the list
			int playerNumber = 2;
			for (int i = 0; i < playerNumber; i++) {
				players.Add(new Player(i+1, this));
			}

			// remove original cycle from cycle scene
			_cycle.Children.Remove(_cycle.Children.FindNodes(c => c.Name == "cycle").First());

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
			
			// render Cycles with their walls
			for(int i = 0; i < players.Count; i++) {
				// Create the camera matrix and set it as the current ModelView transformation
				var mtxRot = float4x4.CreateRotationZ(_angleRoll) * float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(players[i]._angleHorz);
				var mtxCam = float4x4.LookAt(0, 20, -_zoom, 0, 0, 0, 0, 1, 0);
				var mtxOffset = float4x4.CreateTranslation(2 * _offset.x / Width, -2 * _offset.y / Height, 0);
				RC.Projection = mtxOffset * players[i].getProjection();
				_renderer.View = mtxCam * mtxRot * _sceneScale * float4x4.CreateTranslation(-(new float3(players[i].getCycle().getPosition().x, players[i].getCycle().getPosition().y, players[i].getCycle().getPosition().z)));
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


			// Setup Minimap
			if(SHOW_MINIMAP) {
				RC.Projection = float4x4.CreateOrthographic(_mapSize * 2, _mapSize * 2, 0.01f, 20);
				_renderer.View = float4x4.CreateRotationX(-M.PiOver2) * float4x4.CreateTranslation(0, -10, 0);
				RC.Viewport((Width / 2) - (Width / 4), Height - (Width / 3), Width / 3, Width / 3);
				for (int i = 0; i < players.Count; i++) {
					players[i].renderView(_renderer);
				}
				_renderer.Traverse(sceneContainers["land"].Children);
			}
			

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

		public string getVertexShader() {
			return vertexShader;
		}

		public string getPixelShader() {
			return pixelShader;
		}

		public Dictionary<string, SceneContainer> getSceneContainers() {
			return this.sceneContainers;
		}

		public int getMapSize() {
			return _mapSize;
		}

		// Is called when the window was resized
		public override void Resize() {
			for (int i = 0; i < players.Count; i++) {
				players[i].resize();
			}
		}
	}
}