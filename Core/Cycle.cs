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

	class Cycle {
		private int player_id;
		private String player_name;
		private float3 position;
		private float speed;
		private float3 color;
		private TransformComponent cycle;
		public InputKeys input_keys;

		// vars for Rendering
		private SceneContainer _cycle = AssetStorage.Get<SceneContainer>("Cycle.fus");
		private SceneContainer _wall = AssetStorage.Get<SceneContainer>("Wall.fus");

		private TransformComponent _cycleTransform;
		private TransformComponent _cycleWheelR;
		private TransformComponent _cycleWheelL;

		private TransformComponent _cycleWall;
		private List<SceneNodeContainer> _walls = new List<SceneNodeContainer>();

		private bool _firstFrame = true;
		private bool aPressed = false;
		private bool dPressed = false;

		public Cycle (int id) {
			setPlayerId(id);

			switch(id) {
				case 1:
					input_keys = new InputKeys(KeyCodes.A, KeyCodes.D);
					break;
				case 2:
					input_keys = new InputKeys(KeyCodes.Left, KeyCodes.Right);
					break;
				default:
					Debug.WriteLine("ACHTUNG: Spieler 3 aufwärts haben keine Keys zugeordnet.");
					break;
			}

			// Initialize Cycle-TransformComponents
			_cycleTransform = _cycle.Children.FindNodes(c => c.Name == "cycle").First()?.GetTransform();
			_cycleWheelR = _cycle.Children.FindNodes(c => c.Name == "wheel_back").First()?.GetTransform();
			_cycleWheelL = _cycle.Children.FindNodes(c => c.Name == "wheel_front").First()?.GetTransform();

			// set speed
			// TODO: set speed at another place in code
			setSpeed(5);
		}

		//Get-Methods
		public int getPlayerId() {
			return this.player_id;
		}

		public String getPlayerName() {
			return this.player_name;
		}

		public float3 getPosition() {
			return this.position;
		}

		public float getSpeed() {
			return this.speed;
		}

		public float3 getColor() {
			return this.color;
		}

		public TransformComponent getCycle() {
			return this.cycle;
		}

		//Set-Methods
		private void setPlayerId(int id) {
			this.player_id = id;
		}

		public void setPlayerName(String name) {
			this.player_name = name;
		}

		public void setPosition(float3 position) {
			this.position = position;
		}

		public void setSpeed(float speed) {
			this.speed = speed;
		}

		public void setColor(float3 color) {
			this.color = color;
		}

		public void setCycle(TransformComponent cycle) {
			this.cycle = cycle;
		}

		public void renderAFrame(Renderer _renderer) {
			// Resize cycle in first frame for whole game
			if (_firstFrame) {
				_cycleTransform.Scale.x = 30;
				_cycleTransform.Scale.y = 30;
				_cycleTransform.Scale.z = 30;
			}

			bool directionChanged = false;

			// Cycle Rotation
			float cycleYaw = _cycleTransform.Rotation.y;
			if (Keyboard.GetKey(input_keys.getKeyLeft())) {
				if (aPressed == false) {
					cycleYaw = cycleYaw - M.PiOver2;
					directionChanged = true;
				}
				aPressed = true;
			} else {
				aPressed = false;
			}
			if (Keyboard.GetKey(input_keys.getKeyRight())) {
				if (dPressed == false) {
					cycleYaw = cycleYaw + M.PiOver2;
					directionChanged = true;
				}
				dPressed = true;
			} else {
				dPressed = false;
			}
			cycleYaw = FuFiCycles.NormRot(cycleYaw);
			float3 cyclePos = _cycleTransform.Translation + new float3((float)Sin(cycleYaw), 0, (float)Cos(cycleYaw)) * getSpeed();
			_cycleTransform.Rotation = new float3(0, cycleYaw, 0);
			_cycleTransform.Translation = cyclePos;

			// Wheels
			_cycleWheelR.Rotation += new float3(getSpeed() * 0.008f, 0, 0);
			_cycleWheelL.Rotation += new float3(getSpeed() * 0.008f, 0, 0);

			//Write Position into Array and throw crash if cycle collides with a wall or map border
			int x = (int)System.Math.Floor(cyclePos.x + 0.5);
			int z = (int)System.Math.Floor(cyclePos.z + 0.5);
			try {
				if (FuFiCycles._mapMirror[x, z] == 0) {
					FuFiCycles._mapMirror[x, z] = getPlayerId();
				} else {
					// If value at _mapMirror[x, z] isn't 0, there is already a wall
					collision();
				}
			} catch (IndexOutOfRangeException e) {
				// If Index is out of Range a Cycle has collided with the border of the map
				collision();
			}

			// get new wall if direction has changed
			if (directionChanged || _firstFrame) {
				_cycleWall = getWall(x, z);
			}

			// draw wall
			prepareWall(cycleYaw);

			_renderer.Traverse(_cycle.Children);
			_renderer.Traverse(_wall.Children);

			// after first frame set _firstFrame var false
			if (_firstFrame) {
				_firstFrame = false;
			}
		}

		private void collision() {
			Debug.WriteLine("collision");
			setSpeed(0);
		}

		private void prepareWall(float cycleYaw) {

			// if wall is under ground, move it up
			// TODO: check if countdown is finished and game started
			if (_cycleWall.Translation.y == -150) {
				_cycleWall.Translation.y = 0;
			}

			// draw wall itself
			float val = 0.1f;
			if (cycleYaw < M.PiOver2 + val && cycleYaw > M.PiOver2 - val) {
				_cycleWall.Translation.x += getSpeed() / 2;
				_cycleWall.Scale.x = _cycleWall.Scale.x - getSpeed();
			} else if (cycleYaw > -val && cycleYaw < val) {
				_cycleWall.Translation.z += getSpeed() / 2;
				_cycleWall.Scale.z = _cycleWall.Scale.z - getSpeed();
			} else if (cycleYaw < -M.PiOver2 + val && cycleYaw > -M.PiOver2 - val) {
				_cycleWall.Translation.x -= getSpeed() / 2;
				_cycleWall.Scale.x = _cycleWall.Scale.x - getSpeed();
			} else if (cycleYaw > M.Pi - val && cycleYaw < M.Pi + val || cycleYaw > -M.Pi - val && cycleYaw < -M.Pi + val) {
				_cycleWall.Translation.z -= getSpeed() / 2;
				_cycleWall.Scale.z = _cycleWall.Scale.z - getSpeed();
			}
		}

		private TransformComponent getWall(int x, int z) {
			SceneNodeContainer w = new SceneNodeContainer();
			w.Name = "wall";
			w.Components = new List<SceneComponentContainer>();
			TransformComponent tc = new TransformComponent();
			tc.Translation = new float3(x, 0, z);
			tc.Scale = new float3(1, 1, 1);
			w.Components.Add(tc);
			w.Components.Add(_wall.Children.FindNodes(c => c.Name == "wall").First().GetMesh());

			_wall.Children.Add(w);

			_walls.Add(w);

			return _walls.Last().GetTransform();
		}
	}
}
