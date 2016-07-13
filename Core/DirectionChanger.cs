using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.FuFiCycles.Core {
	public class DirectionChanger {
		byte ticks;
		byte ticksTotal;
		bool left;
		Player player;

		public DirectionChanger(byte ticks, bool left, Player player) {
			this.ticks = ticks;
			this.ticksTotal = ticks;
			this.left = left;
			this.player = player;
		}

		public void tick() {
			if(ticks > 0) {
				ticks--;
				if(left) {
					player._angleHorz += M.PiOver2 / ticksTotal;
				} else {
					player._angleHorz -= M.PiOver2 / ticksTotal;
				}
			} else {
				player.deleteDirectionChanger(this);
			}
		}
	}
}
