using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using static System.Math;
using static Fusee.Engine.Core.Input;
using System.Diagnostics;
using System;

namespace Fusee.FuFiCycles.Core {

	class Player {
		private int player_id;
		private String player_name;
		private float3 color;
		private Cycle cycle;
		public InputKeys input_keys;

		// vars for Rendering
		SceneContainer wall;

		private TransformComponent _cycleTransform;

		private SceneNodeContainer _wallSNC;
		private TransformComponent _cycleWall;

		private bool _firstFrame = true;

		private const float RotationSpeed = 7;

		public Player (int id, Cycle cycle, SceneContainer wall) {
			setCycle(cycle);
			setWall(wall);

			setPlayerId(id);

			_wallSNC = getWall().Children.FindNodes(c => c.Name == "wall").First();

			// TODO: let player pick color
			switch (id) {
				case 1:
					input_keys = new InputKeys(KeyCodes.A, KeyCodes.D, KeyCodes.W, KeyCodes.S);
					setColor(new float3(0, 0.9f, 1f));
					break;
				case 2:
					input_keys = new InputKeys(KeyCodes.Left, KeyCodes.Right, KeyCodes.Up, KeyCodes.Down);
					setColor(new float3(0, 1.0f, 0));
					break;
				default:
					Debug.WriteLine("ACHTUNG: Spieler 3 aufwärts haben keine Keys zugeordnet.");
					setColor(new float3(0.2f, 0.2f, 0.2f));
					break;
			}
		}

		//Get-Methods
		public int getPlayerId() {
			return this.player_id;
		}

		public String getPlayerName() {
			return this.player_name;
		}

		public float3 getColor() {
			return this.color;
		}

		public InputKeys getInputKeys() {
			return this.input_keys;
		}

		//Set-Methods
		private void setPlayerId(int id) {
			this.player_id = id;
		}

		public void setPlayerName(String name) {
			this.player_name = name;
		}

		public void setColor(float3 color) {
			this.color = color;
			
			// create new colors
			float intensity = 0.8f;
			MaterialComponent newcolor2 = new MaterialComponent();
			newcolor2.Diffuse = new MatChannelContainer();
			newcolor2.Diffuse.Color = new float3(color.x * intensity, color.y * intensity, color.z * intensity);

			float intensity2 = 0.6f;
			MaterialComponent newcolor3 = new MaterialComponent();
			newcolor3.Diffuse = new MatChannelContainer();
			newcolor3.Diffuse.Color = new float3(color.x * intensity2, color.y * intensity2, color.z * intensity2);

			// change model colors
			getCycle().getSNC().Children[0].Components[1] = newcolor2;
			getCycle().getSNC().Children[1].Components[1] = newcolor3;
		}

		public Cycle getCycle() {
			return this.cycle;
		}

		public void setCycle(Cycle cycle) {
			this.cycle = cycle;
		}

		public SceneContainer getWall() {
			return this.wall;
		}

		public void setWall(SceneContainer wall) {
			this.wall = wall;
		}

		public void renderAFrame(Renderer _renderer) {
			bool directionChanged = false;

			// Cycle Rotation
			float cycleYaw = getCycle().getSNC().GetTransform().Rotation.y;
			if (Keyboard.IsKeyDown(input_keys.getKeyLeft())) {
				if (this.player_id == 1) {
					FuFiCycles._angleHorz += M.PiOver2;
				}
				//FuFiCycles._angleVelHorz = RotationSpeed * M.PiOver4 * 0.002f;
				cycleYaw -= M.PiOver2;
				cycleYaw = FuFiCycles.NormRot(cycleYaw);
				directionChanged = true;
				cycle.setDirection(cycleYaw);
			}

			if (Keyboard.IsKeyDown(input_keys.getKeyRight())) {
				if (this.player_id == 1) {
					FuFiCycles._angleHorz -= M.PiOver2;
					//FuFiCycles._angleVelHorz = -RotationSpeed * M.PiOver4 * 0.002f;
				}
				cycleYaw += M.PiOver2;
				cycleYaw = FuFiCycles.NormRot(cycleYaw);
				directionChanged = true;
				cycle.setDirection(cycleYaw);
			}
			/*
			if (Keyboard.IsKeyDown(input_keys.getKeyUp())) {
				if (this.player_id == 1) {
					FuFiCycles._angleVelVert = -RotationSpeed * -0.02f * 0.002f;
				}
			}

			if (Keyboard.IsKeyDown(input_keys.getKeyDown())) {
				if (this.player_id == 1) {
					FuFiCycles._angleVelVert = -RotationSpeed * 0.02f * 0.002f;
				}
			}*/
			getCycle().setPosition(getCycle().getSNC().GetTransform().Translation + new float3((float)Sin(cycleYaw), 0, (float)Cos(cycleYaw)) * getCycle().getSpeed());
			getCycle().getSNC().GetTransform().Rotation = new float3(0, cycleYaw, 0);
			getCycle().getSNC().GetTransform().Translation = getCycle().getPosition();

			// Wheels
			getCycle().getFrontWheel().Rotation += new float3(getCycle().getSpeed() * 0.008f, 0, 0);
			getCycle().getBackWheel().Rotation += new float3(getCycle().getSpeed() * 0.008f, 0, 0);

			//Write Position into Array and throw crash if cycle collides with a wall or map border
			int x = (int)System.Math.Floor(getCycle().getPosition().x + 0.5);
			int z = (int)System.Math.Floor(getCycle().getPosition().z + 0.5);
			try {
				// loop through all positions since last frame
				for (int i = 0; i < cycle.getSpeed(); i++) {
					int x2 = x;
					int z2 = z;

					switch (cycle.getDirection()) {
						case Direction.RIGHT:
							x2 -= i;
							break;
						case Direction.FORWARD:
							z2 -= i;
							break;
						case Direction.LEFT:
							x2 += i;
							break;
						case Direction.BACKWARD:
							z2 += i;
							break;
					}

					if (FuFiCycles._mapMirror[x2, z2] == 0) {
						FuFiCycles._mapMirror[x2, z2] = getPlayerId();
					} else {
						// If value at _mapMirror[x2, z2] isn't 0, there is already a wall
						getCycle().setCollided();
					}
				}
			} catch (IndexOutOfRangeException e) {
				// If Index is out of Range a Cycle has collided with the border of the map
				getCycle().setCollided();
				Debug.WriteLine(e.Message);
			}
			
			// get new wall if direction has changed
			
			if (directionChanged || _firstFrame) {
				_cycleWall = getWall(x, z, cycleYaw);
			}

			// draw wall
			prepareWall(cycleYaw);
			
			_renderer.Traverse(getCycle().getSceneContainer().Children);
			_renderer.Traverse(_wall.Children);

			// after first frame set _firstFrame var false
			if (_firstFrame) {
				_firstFrame = false;
			}
		}

		private void prepareWall(float cycleYaw) {

			// if wall is under ground, move it up
			// TODO: check if countdown is finished and game started
			if (_cycleWall.Translation.y == -150) {
				_cycleWall.Translation.y = 0;
			}

			// draw wall itself
			switch (cycle.getDirection()) {
				case Direction.RIGHT:
					_cycleWall.Translation.x += getCycle().getSpeed() / 2;
					_cycleWall.Scale.x = _cycleWall.Scale.x - getCycle().getSpeed();
					break;
				case Direction.FORWARD:
					_cycleWall.Translation.z += getCycle().getSpeed() / 2;
					_cycleWall.Scale.z = _cycleWall.Scale.z - getCycle().getSpeed();
					break;
				case Direction.LEFT:
					_cycleWall.Translation.x -= getCycle().getSpeed() / 2;
					_cycleWall.Scale.x = _cycleWall.Scale.x - getCycle().getSpeed();
					break;
				case Direction.BACKWARD:
					_cycleWall.Translation.z -= getCycle().getSpeed() / 2;
					_cycleWall.Scale.z = _cycleWall.Scale.z - getCycle().getSpeed();
					break;
			}
		}

		private TransformComponent getWall(int x, int z, float cycleYaw) {
			switch(cycle.getDirection()) {
				case Direction.RIGHT:
					x -= (int)getCycle().getSpeed();
					break;
				case Direction.FORWARD:
					z -= (int)getCycle().getSpeed();
					break;
				case Direction.LEFT:
					x += (int)getCycle().getSpeed();
					break;
				case Direction.BACKWARD:
					z += (int)getCycle().getSpeed();
					break;
			}

			SceneNodeContainer w = new SceneNodeContainer();
			w.Name = "wall" + x + z;
			w.Components = new List<SceneComponentContainer>();

			// create new TransformComponent
			TransformComponent tc = new TransformComponent();
			tc.Name = "tc" + x + z;
			tc.Rotation = new float3(0.0f, 0.0f, 0.0f);
			tc.Scale = new float3(5.0f, 0.5f, 5.0f);
			tc.Translation = new float3(x, 0.0f, z);

			w.Components.Add(tc);
			w.Components.Add(_wallSNC?.GetMaterial());
			w.Components.Add(_wallSNC?.GetMesh());

			// add new wall to wall scene
			_wall.Children.Add(w);

			// set wall color
			MaterialComponent newcolor = new MaterialComponent();
			newcolor.Diffuse = new MatChannelContainer();
			newcolor.Diffuse.Color = getColor();

			w.Components[1] = newcolor;

			// return new wall
			return w.GetTransform();
		}

	}
}
