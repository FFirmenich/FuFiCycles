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
	class Cycle {
		private SceneContainer sceneContainer;
		private SceneNodeContainer sceneNodeContainer;
		private TransformComponent frontwheel;
		private TransformComponent backwheel;

		public Cycle(int id, SceneContainer _cycle) {
			sceneContainer = _cycle;
			SceneNodeContainer originalSceneNodeContainer = _cycle.Children.FindNodes(c => c.Name == "cycle").First();

			sceneNodeContainer = new SceneNodeContainer();
			sceneNodeContainer.Name = "cycle" + id;
			sceneNodeContainer.Components = new List<SceneComponentContainer>();

			// create new TransformComponent
			TransformComponent tc = new TransformComponent();
			tc.Name = "tc" + id;
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
	}
}
