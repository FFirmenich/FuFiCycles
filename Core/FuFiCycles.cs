using System.Collections.Generic;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using static Fusee.Engine.Core.Input;
using System.Linq;
using Fusee.Xene;

namespace Fusee.FuFiCycles.Core {
	[FuseeApplication(Name = "FuFiCycles", Description = "A FuFi Production")]
	public class FuFiCycles : RenderCanvas {
		private List<Player> players = new List<Player>();

		// angle variables
		public static float _angleHorz = 0, _angleVert = -M.PiOver6 * 0.2f, _angleVelHorz, _angleVelVert, _angleRoll, _angleRollInit, _zoomVel, _zoom;
		private static float2 _offset;
		private static float2 _offsetInit;

		public const float RotationSpeed = 7;
		private const float Damping = 8f;

		private SceneContainer _scene = AssetStorage.Get<SceneContainer>("Land.fus");
		private float4x4 _sceneScale;
		private float4x4 _projection;

		private SceneContainer _cycle = AssetStorage.Get<SceneContainer>("Cycle.fus");
		private SceneContainer _wall = AssetStorage.Get<SceneContainer>("Wall.fus");

		private bool _keys;

		private Dictionary<float3, int> _cyclePositions = new Dictionary<float3, int>();

		private int _mapSize = 0;
		public static float[,] _mapMirror;

		private Renderer _renderer;

		// Init is called on startup. 
		public override void Init() {
			// Load the scene
			_sceneScale = float4x4.CreateScale(0.04f);

			// Instantiate our self-written renderer
			_renderer = new Renderer(RC);

			// Add two player to the list
			players.Add(new Player(1, new Cycle(1, _cycle), _wall));
			players.Add(new Player(2, new Cycle(2, _cycle), _wall));
			players[1].getCycle().setPosition(new float3(600,0, 60));

			// remove original cycle from cycle scene
			_cycle.Children.Remove(_cycle.Children.FindNodes(c => c.Name == "cycle").First());

			// set Map Size
			//MeshComponent ground = _scene.Children.FindNodes(c => c.Name == "Ground").First()?.GetMesh();
			_mapSize = 16000;
			_mapMirror = new float[_mapSize, _mapSize];

			// Set the clear color for the backbuffer
			RC.ClearColor = new float4(1, 1, 1, 1);
		}

		// RenderAFrame is called once a frame
		public override void RenderAFrame() {
			// Clear the backbuffer
			RC.Clear(ClearFlags.Color | ClearFlags.Depth);

			// Mouse and keyboard movement
			if (Keyboard.UpDownAxis != 0) {
				_keys = true;
			}
			
			var curDamp = (float)System.Math.Exp(0.1f);
			
			// zoom
			_zoom += _zoomVel;
			// Limit zoom
			if (_zoom < 80)
				_zoom = 80;
			if (_zoom > 2000)
				_zoom = 2000;

			_angleHorz += _angleVelHorz;
			// Wrap-around to keep _angleHorz between -PI and + PI
			_angleHorz = M.MinAngle(_angleHorz);

			_angleVert += _angleVelVert;
			// Limit pitch to the range between [-PI/2, + PI/2]
			_angleVert = M.Clamp(_angleVert, -M.PiOver2, M.PiOver2);

			// Wrap-around to keep _angleRoll between -PI and + PI
			_angleRoll = M.MinAngle(_angleRoll);

			// Create the camera matrix and set it as the current ModelView transformation
			var mtxRot = float4x4.CreateRotationZ(_angleRoll) * float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
			var mtxCam = float4x4.LookAt(0, 20, -_zoom, 0, 0, 0, 0, 1, 0);
			
			_renderer.View = mtxCam * mtxRot * _sceneScale * float4x4.CreateTranslation(-(new float3(players[0].getCycle().getPosition().x, players[0].getCycle().getPosition().y, players[0].getCycle().getPosition().z)));
			var mtxOffset = float4x4.CreateTranslation(2 * _offset.x / Width, -2 * _offset.y / Height, 0);
			RC.Projection = mtxOffset * _projection;

			// Render
			// render Scene
			_renderer.Traverse(_scene.Children);

			// render Cycles with their walls
			for(int i = 0; i < players.Count; i++) {
				if (!players[i].getCycle().isCollided()) {
					players[i].renderAFrame(_renderer);
				}
			}

			// Swap buffers: Show the contents of the backbuffer (containing the currently rerndered farame) on the front buffer.
			Present();
		}
		
		public static float NormRot(float rot) {
			while (rot > M.Pi)
				rot -= M.TwoPi;
			while (rot < -M.Pi)
				rot += M.TwoPi;
			return rot;
		}

		// Is called when the window was resized
		public override void Resize() {
			// Set the new rendering area to the entire new windows size
			RC.Viewport(0, 0, Width, Height);

			// Create a new projection matrix generating undistorted images on the new aspect ratio.
			var aspectRatio = Width / (float)Height;

			// 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
			// Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
			// Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
			_projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 20000);
		}
	}
}