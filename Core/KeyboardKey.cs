using Fusee.Engine.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Fusee.FuFiCycles.Core {

	public class KeyboardKey {
		private bool down = false;
		private bool pressed = false;
		public void setDown() {
			this.down = true;
		}
		public void setNotDown() {
			this.down = false;
		}
		//
		// Is Key Down
		//
		public bool isDown() {
			return this.down;
		}
		public void setPressed() {
			this.pressed = true;
			setDown();
		}
		public void setUnpressed() {
			this.pressed = false;
		}
		//
		// Is Key pressed down right in this frame
		//
		public bool isPressed() {
			return this.pressed;
		}
	}
}
