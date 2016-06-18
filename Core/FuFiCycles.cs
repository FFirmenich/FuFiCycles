using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using static System.Math;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;
using System.Diagnostics;
using System;

namespace Fusee.FuFiCycles.Core {
	[FuseeApplication(Name = "FuFiCycles", Description = "A FuFi Production")]
	public class FuFiCycles : RenderCanvas {
		private List<Cycle> cycles = new List<Cycle>();

		// angle variables
		private static float _angleHorz = M.PiOver6 * 2.0f, _angleVert = -M.PiOver6 * 0.5f, _angleVelHorz, _angleVelVert, _angleRoll, _angleRollInit, _zoomVel, _zoom;
		private static float2 _offset;
		private static float2 _offsetInit;

		private const float RotationSpeed = 7;
		private const float Damping = 0.8f;

		private SceneContainer _scene;
		private float4x4 _sceneScale;
		private float4x4 _projection;
		private bool _twoTouchRepeated;

		private bool _keys;

		private Dictionary<float3, int> _cyclePositions = new Dictionary<float3, int>();

		private int _mapSize = 0;
		public static float[,] _mapMirror;

		private Renderer _renderer;

		// Init is called on startup. 
		public override void Init() {
			// Load the scene
			_scene = AssetStorage.Get<SceneContainer>("Land.fus");
			_sceneScale = float4x4.CreateScale(0.04f);

			// Instantiate our self-written renderer
			_renderer = new Renderer(RC);

			// Add two cycles to the list
			cycles.Add(new Cycle(1));
			//cycles.Add(new Cycle(2));

			// set Map Size
			//MeshComponent ground = _scene.Children.FindNodes(c => c.Name == "Ground").First()?.GetMesh();
			_mapSize = 5000;
			_mapMirror = new float[_mapSize, _mapSize];

			// Set the clear color for the backbuffer
			RC.ClearColor = new float4(1, 1, 1, 1);
		}

		// RenderAFrame is called once a frame
		public override void RenderAFrame() {
			// Clear the backbuffer
			RC.Clear(ClearFlags.Color | ClearFlags.Depth);

			// Mouse and keyboard movement
			if (Keyboard.LeftRightAxis != 0 || Keyboard.UpDownAxis != 0) {
				_keys = true;
			}

			var curDamp = (float)System.Math.Exp(-Damping * DeltaTime);

			// Zoom & Roll
			if (Touch.TwoPoint) {
				if (!_twoTouchRepeated) {
					_twoTouchRepeated = true;
					_angleRollInit = Touch.TwoPointAngle - _angleRoll;
					_offsetInit = Touch.TwoPointMidPoint - _offset;
				}
				_zoomVel = Touch.TwoPointDistanceVel * -0.01f;
				_angleRoll = Touch.TwoPointAngle - _angleRollInit;
				_offset = Touch.TwoPointMidPoint - _offsetInit;
			} else {
				_twoTouchRepeated = false;
				_zoomVel = Mouse.WheelVel * -0.5f;
				_angleRoll *= curDamp * 0.8f;
				_offset *= curDamp * 0.8f;
			}

			// UpDown / LeftRight rotation
			if (Mouse.LeftButton) {
				_keys = false;
				_angleVelHorz = -RotationSpeed * Mouse.XVel * 0.000002f;
				_angleVelVert = -RotationSpeed * Mouse.YVel * 0.000002f;
			} else if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint) {
				_keys = false;
				float2 touchVel;
				touchVel = Touch.GetVelocity(TouchPoints.Touchpoint_0);
				_angleVelHorz = -RotationSpeed * touchVel.x * 0.000002f;
				_angleVelVert = -RotationSpeed * touchVel.y * 0.000002f;
			} else {
				if (_keys) {
					_angleVelHorz = -RotationSpeed * Keyboard.LeftRightAxis * 0.002f;
					_angleVelVert = -RotationSpeed * Keyboard.UpDownAxis * 0.002f;
				} else {
					_angleVelHorz *= curDamp;
					_angleVelVert *= curDamp;
				}
			}

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
			_renderer.View = mtxCam * mtxRot * _sceneScale;
			var mtxOffset = float4x4.CreateTranslation(2 * _offset.x / Width, -2 * _offset.y / Height, 0);
			RC.Projection = mtxOffset * _projection;

			// Render
			// render Scene
			_renderer.Traverse(_scene.Children);

			// render Cycles with their walls
			for(int i = 0; i < cycles.Count; i++) {
				cycles[i].renderAFrame(_renderer);
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