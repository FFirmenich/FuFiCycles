using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using static Fusee.FuFiCycles.Core.GameSettings;


namespace Fusee.FuFiCycles.Core {
	public class Cycle {
		// ccene and model vars
		private SceneNodeContainer sceneNodeContainer;
		private TransformComponent frontwheel;
		private TransformComponent backwheel;

		// speed, position etc.
		private int id;
		private int speed;
		private float3 position;
		private bool collided = false;
		private Direction direction;

		public Cycle(int id) {
			setId(id);
			SceneNodeContainer originalSceneNodeContainer = INSTANCE.getSceneContainers()["cycle"].Children.FindNodes(c => c.Name == "cycle").First();

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

			frontwheel = INSTANCE.getSceneContainers()["cycle"].Children.FindNodes(c => c.Name == "wheel_front").First()?.GetTransform();
			backwheel = INSTANCE.getSceneContainers()["cycle"].Children.FindNodes(c => c.Name == "wheel_back").First()?.GetTransform();

			scale();

			INSTANCE.getSceneContainers()["cycle"].Children.Add(getSNC());

			// set speed
			// TODO: let player set speed
			setSpeed(30);

			// set the cycle to the right position according to its id
			setStartPosition();
		}

		private void scale() {
			this.sceneNodeContainer.GetTransform().Scale.x = 30;
			this.sceneNodeContainer.GetTransform().Scale.y = 30;
			this.sceneNodeContainer.GetTransform().Scale.z = 30;
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

		public int getSpeed() {
			return this.speed;
		}

		public void setSpeed(int speed) {
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

		public void setDirection(Direction direction) {
			this.direction = direction;
			getSNC().GetTransform().Rotation = new float3(0, direction.getYaw(), 0);
		}

		public void setDirection(float yaw) {
			setDirection(DirectionMethods.directionFromYaw(yaw));
		}
		private void setStartPosition() {
			ushort distanceFromWall = (ushort) 500;
			switch (getId()) {
				case 1:
					setPosition(new float3(MAP_SIZE / 2 + 100, 0, distanceFromWall));
					setDirection(Direction.FORWARD);
					break;
				case 2:
					setPosition(new float3(MAP_SIZE / 2 - 100, 0, MAP_SIZE - distanceFromWall));
					setDirection(Direction.BACKWARD);
					break;
				default:
					Debug.WriteLine("ACHTUNG: Spieler 3 aufwärts haben keine Positionen zugeordnet.");
					break;
			}
		}
	}
}
