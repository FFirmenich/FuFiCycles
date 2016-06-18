using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using System.Diagnostics;

namespace Fusee.FuFiCycles.Core {

	class Cycle {
		private int player_id;
		private String player_name;
		private float3 position;
		private float speed;
		private float3 color;
		private TransformComponent cycle;
		public InputKeys input_keys;

		public Cycle (int id) {
			setPlayerId(id);

			if(id == 1) {
				input_keys = new InputKeys("A", "D");
			} else if (id == 2) {
				input_keys = new InputKeys("LEFT", "RIGHT");
			} else {
				Debug.WriteLine("ACHTUNG: Spieler 3 aufwärts haben keine Keys zugeordnet.");
			}
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
	}
}
