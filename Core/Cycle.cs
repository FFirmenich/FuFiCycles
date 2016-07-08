using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using static System.Math;
using static Fusee.Engine.Core.Input;
using System.Diagnostics;
using System;


namespace Fusee.FuFiCycles.Core {
	public class Cycle {
		// ccene and model vars
		private SceneContainer sceneContainer;
		private SceneNodeContainer sceneNodeContainer;
		private TransformComponent frontwheel;
		private TransformComponent backwheel;

		// speed, position etc.
		private int id;
		private float speed;
		private float3 position;
		private bool collided = false;
		private Direction direction;

		public Cycle(int id, SceneContainer _cycle) {
			setId(id);
			sceneContainer = _cycle;
			SceneNodeContainer originalSceneNodeContainer = _cycle.Children.FindNodes(c => c.Name == "cycle").First();

			sceneNodeContainer = new SceneNodeContainer();
			sceneNodeContainer.Name = "cycle" + getId();
			sceneNodeContainer.Components = new List<SceneComponentContainer>();

			// create new TransformComponent
			TransformComponent tc = new TransformComponent();
			tc.Name = "tc" + getId();
			tc.Rotation = new float3(0.0f, 0.0f, 0.0f);
			tc.Scale = originalSceneNodeContainer.GetTransform().Scale;
			tc.Translation = new float3(0.0f, 0.0f, 0.0f);

			sceneNodeContainer.Components.Add(tc);
			sceneNodeContainer.Components.Add(originalSceneNodeContainer.GetMesh());

			sceneNodeContainer.Children = new List<SceneNodeContainer>();
			for(int i = 0; i < originalSceneNodeContainer.Children.Count; i++) {
				SceneNodeContainer child = new SceneNodeContainer();
				child.Name = "child" + i;
				child.Components = new List<SceneComponentContainer>();
				for(int j = 0; j < originalSceneNodeContainer.Children[i].Components.Count; j++) {
					SceneComponentContainer comp = originalSceneNodeContainer.Children[i].Components[j];
					comp.Name = "comp" + i + j;
					child.Components.Add(comp);
				}
				sceneNodeContainer.Children.Add(child);
			}

			frontwheel = _cycle.Children.FindNodes(c => c.Name == "wheel_front").First()?.GetTransform();
			backwheel = _cycle.Children.FindNodes(c => c.Name == "wheel_back").First()?.GetTransform();

			scale();

			sceneContainer.Children.Add(getSNC());

			// set speed
			// TODO: let player set speed
			setSpeed(10);
		}

		private void scale() {
			this.sceneNodeContainer.GetTransform().Scale.x = 30;
			this.sceneNodeContainer.GetTransform().Scale.y = 30;
			this.sceneNodeContainer.GetTransform().Scale.z = 30;
		}

		public SceneContainer getSceneContainer() {
			return sceneContainer;
		}

		public SceneNodeContainer getSNC() {
			return sceneNodeContainer;
		}

		public TransformComponent getFrontWheel() {
			return frontwheel;
		}

		public TransformComponent getBackWheel() {
			return backwheel;
		}

		public int getId() {
			return this.id;
		}

		public void setId(int id) {
			this.id = id;
		}

		public float getSpeed() {
			return this.speed;
		}

		public void setSpeed(float speed) {
			this.speed = speed;
		}

		public float3 getPosition() {
			return this.position;
		}

		public void setPosition(float3 position) {
			getSNC().GetTransform().Translation = position;
			this.position = position;
		}

		public bool isCollided() {
			return this.collided;
		}

		public void setCollided() {
			Debug.WriteLine("cycle" + getId() + " kollidiert");
			this.collided = true;
			setSpeed(0);
		}

		public Direction getDirection() {
			return this.direction;
		}

		public void setDirection(float yaw) {
			this.direction = yawToDirection(yaw);
		}

		public Direction yawToDirection(float yaw) {
			// fix wall position
			float val = 0.1f;
			if (yaw < M.PiOver2 + val && yaw > M.PiOver2 - val) {
				return Direction.RIGHT;
			} else if (yaw > -val && yaw < val || yaw < M.TwoPi + val && yaw > M.TwoPi - val) {
				return Direction.FORWARD;
			} else if (yaw < -M.PiOver2 + val && yaw > -M.PiOver2 - val || yaw < M.ThreePiOver2 + val && yaw > M.ThreePiOver2 - val) {
				return Direction.LEFT;
			} else if (yaw > M.Pi - val && yaw < M.Pi + val || yaw > -M.Pi - val && yaw < -M.Pi + val) {
				return Direction.BACKWARD;
			}
			throw (new Exception("no direction found"));
		}
	}
}
