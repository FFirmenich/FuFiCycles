using System.Collections.Generic;
using System.Linq;
using Fusee.Engine.Common;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using static System.Math;
using static Fusee.Engine.Core.Input;
using System.Diagnostics;
using System;
using static Fusee.FuFiCycles.Core.GameSettings;
using System.Threading;

namespace Fusee.FuFiCycles.Core {

	public class Player {
		private byte player_id;
		private String player_name;
		private Cycle cycle;
		public InputKeys input_keys;

		// vars for Rendering
		public float4x4 projection;
		private SceneNodeContainer _wallSNC;
		private TransformComponent _cycleTransform;
		private TransformComponent _cycleWall;
		public float _angleHorz = 0;
		private short horzAngleTicker;
		byte ticksPerAngleChange = 10;
		bool firstWallDrawn = false;

		// Wall Sizes
		public static float WALL_WIDTH = 20.0f;
		public static float WALL_HEIGHT = 0.5f;

		public Player (byte id) {
			setPlayerId(id);

			setCycle(new Cycle(getPlayerId()));

			_wallSNC = INSTANCE.getSceneContainers()["wall"].Children.FindNodes(c => c.Name == "wall").First();

			switch (getPlayerId()) {
				case 1:
					input_keys = new InputKeys(KeyCodes.Left, KeyCodes.Right, KeyCodes.Up, KeyCodes.Down);
					break;
				case 2:
					input_keys = new InputKeys(KeyCodes.A, KeyCodes.D, KeyCodes.W, KeyCodes.S);
					break;
				default:
					Debug.WriteLine("ACHTUNG: Spieler 3 aufwärts haben keine Keys zugeordnet.");
					break;
			}

			// set horizontal angle from cycle direction
			switch(getCycle().getDirection()) {
				case Direction.RIGHT:
					_angleHorz = M.PiOver2;
					break;
				case Direction.FORWARD:
					_angleHorz = 0;
					break;
				case Direction.LEFT:
					_angleHorz = M.ThreePiOver2;
					break;
				case Direction.BACKWARD:
					_angleHorz = M.Pi;
					break;
				default:
					throw (new Exception("no direction found"));
			}
		}

		~Player() {
			INSTANCE.getSceneContainers()["cycle"].Children.Remove(getCycle().getSNC());
		}

		//Get-Methods
		public byte getPlayerId() {
			return this.player_id;
		}

		public String getPlayerName() {
			return this.player_name;
		}

		public InputKeys getInputKeys() {
			return this.input_keys;
		}

		//Set-Methods
		private void setPlayerId(byte id) {
			this.player_id = id;
		}

		public void setPlayerName(String name) {
			this.player_name = name;
		}
		public Cycle getCycle() {
			return this.cycle;
		}
		public void setCycle(Cycle cycle) {
			this.cycle = cycle;
		}

		public bool checkForDirectionChange() {
			if(getCycle().isCollided()) {
				return false;
			}
			bool directionChanged = false;
			// Cycle Rotation
			float cycleYaw = getCycle().getSNC().GetTransform().Rotation.y;
			if (INSTANCE.keyboardKeys.ingameKeys[input_keys.getKeyLeft()].isPressed()) {
				INSTANCE.keyboardKeys.ingameKeys[input_keys.getKeyLeft()].setUnpressed();
				horzAngleTicker += ticksPerAngleChange;
				cycleYaw -= M.PiOver2;
				cycleYaw = FuFiCycles.NormRot(cycleYaw);
				directionChanged = true;
				getCycle().setDirection(cycleYaw);
			} else if (INSTANCE.keyboardKeys.ingameKeys[input_keys.getKeyRight()].isPressed()) {
				INSTANCE.keyboardKeys.ingameKeys[input_keys.getKeyRight()].setUnpressed();
				horzAngleTicker -= ticksPerAngleChange;
				cycleYaw += M.PiOver2;
				cycleYaw = FuFiCycles.NormRot(cycleYaw);
				directionChanged = true;
				getCycle().setDirection(cycleYaw);
			}
			changeHorzAngle();
			return directionChanged;
		}

		public void renderAFrame(Renderer _renderer) {
			bool directionChanged = checkForDirectionChange();
			// Wrap-around to keep _angleHorz between -PI and + PI
			_angleHorz = M.MinAngle(_angleHorz);

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
			if(!INSTANCE.getLastMatch().getLastRound().isPaused()) {
				getCycle().setPosition(getCycle().getSNC().GetTransform().Translation + new float3((float)Sin(getCycle().getDirection().getYaw()), 0, (float)Cos(getCycle().getDirection().getYaw())) * getCycle().getSpeed());

				// Wheels
				getCycle().getFrontWheel().Rotation += new float3(getCycle().getSpeed() * 0.008f, 0, 0);
				getCycle().getBackWheel().Rotation += new float3(getCycle().getSpeed() * 0.008f, 0, 0);

				//Write Position into Array and throw crash if cycle collides with a wall or map border
				int x = (int)System.Math.Floor(getCycle().getPosition().x + 0.5);
				int z = (int)System.Math.Floor(getCycle().getPosition().z + 0.5);
				try {
					// loop through all positions since last frame
					for (int i = 0; i < getCycle().getSpeed(); i++) {
						int x2 = x;
						int z2 = z;

						switch (getCycle().getDirection()) {
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

						if (INSTANCE.getLastMatch().getLastRound().getMapMirror()[x2, z2] == 0) {
							INSTANCE.getLastMatch().getLastRound().getMapMirror()[x2, z2] = getPlayerId();
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
				if (directionChanged || INSTANCE.getLastMatch().getLastRound().getFirstFrame()) {
					_cycleWall = getWall(x, z);
					fixWallEdges();
					firstWallDrawn = true;
				}

				// draw wall
				prepareWall();

			}

			// render Scene
			renderView(_renderer);
		}

		private void prepareWall() {
			if(_cycleWall == null) {
				return;
			}
			if (_cycleWall.Translation.y == -150) {
				_cycleWall.Translation.y = 0;
			}

			// draw wall itself
			switch (getCycle().getDirection()) {
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

		private TransformComponent getWall(int x, int z) {
			// fix unwanted spaces between walls after direction has changed
			switch (getCycle().getDirection()) {
				case Direction.RIGHT:
					x -= (int)(getCycle().getSpeed());
					break;
				case Direction.FORWARD:
					z -= (int)(getCycle().getSpeed());
					break;
				case Direction.LEFT:
					x += (int)(getCycle().getSpeed());
					break;
				case Direction.BACKWARD:
					z += (int)(getCycle().getSpeed());
					break;
			}
			

			//create new wall
			SceneNodeContainer w = new SceneNodeContainer();
			w.Name = "wall" + x + z;
			w.Components = new List<SceneComponentContainer>();

			// create new TransformComponent
			TransformComponent tc = new TransformComponent();
			tc.Name = "tc" + x + z;
			tc.Rotation = new float3(0.0f, 0.0f, 0.0f);
			tc.Scale = new float3(WALL_WIDTH, WALL_HEIGHT, WALL_WIDTH);
			tc.Translation = new float3(x, 0.0f, z);

			w.Components.Add(tc);
			w.Components.Add(_wallSNC?.GetMaterial());
			w.Components.Add(_wallSNC?.GetMesh());

			// add new wall to wall scene
			INSTANCE.getSceneContainers()["wall"].Children.Add(w);

			// set wall color
			MaterialComponent newcolor = new MaterialComponent();
			newcolor.Diffuse = new MatChannelContainer();
			newcolor.Diffuse.Color = getCycle().getColor();

			w.Components[1] = newcolor;

			// return new wall
			return w.GetTransform();
		}

		// Is called when the window was resized
		public void resize() {
			var aspectRatio = (INSTANCE.Width / 2) / (float)INSTANCE.Height;

			// 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
			// Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
			// Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
			setProjection(float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 20000));
		}

		public void setProjection(float4x4 projection) {
			this.projection = projection;
		}

		public float4x4 getProjection() {
			return this.projection;
		}

		public void renderView(Renderer _renderer) {
			try {
				_renderer.Traverse(INSTANCE.getSceneContainers()["cycle"].Children);
			} catch(InvalidOperationException ioe) {
				Debug.WriteLine(ioe.StackTrace);
			}
			_renderer.Traverse(INSTANCE.getSceneContainers()["wall"].Children);
		}

		/// <summary>
		/// fix Empty Edges between walls
		/// </summary>
		private void fixWallEdges() {
			switch (getCycle().getDirection()) {
				case Direction.RIGHT:
					_cycleWall.Scale.x -= WALL_WIDTH * 2;
					break;
				case Direction.FORWARD:
					_cycleWall.Scale.z -= WALL_WIDTH * 2;
					break;
				case Direction.LEFT:
					_cycleWall.Scale.x -= WALL_WIDTH * 2;
					break;
				case Direction.BACKWARD:
					_cycleWall.Scale.z -= WALL_WIDTH * 2;
					break;
			}
		}
		private bool isKeyDown(KeyCodes key) {
			if (Keyboard.GetKey(key)) {
				return true;
			}
			return false;
		}
		private void changeHorzAngle() {
			if (horzAngleTicker > 0) {
				_angleHorz += M.PiOver2 / ticksPerAngleChange;
				horzAngleTicker--;
			}
			if(horzAngleTicker < 0) {
				_angleHorz -= M.PiOver2 / ticksPerAngleChange;
				horzAngleTicker++;
			}
		}
		public bool gotFirstWall() {
			return firstWallDrawn;
		}
	}
}
