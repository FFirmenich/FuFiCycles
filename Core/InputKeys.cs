using Fusee.Engine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.FuFiCycles.Core {
	public class InputKeys {
		private KeyCodes key_left;
		private KeyCodes key_right;
		private KeyCodes key_up;
		private KeyCodes key_down;

		public InputKeys(KeyCodes left, KeyCodes right, KeyCodes up, KeyCodes down) {
			setKeyLeft(left);
			setKeyRight(right);
			setKeyUp(up);
			setKeyDown(down);
		}

		public KeyCodes getKeyLeft() {
			return this.key_left;
		}

		public KeyCodes getKeyRight() {
			return this.key_right;
		}

		public KeyCodes getKeyUp() {
			return this.key_up;
		}

		public KeyCodes getKeyDown() {
			return this.key_down;
		}

		public void setKeyLeft(KeyCodes input_key) {
			this.key_left = input_key;
		}

		public void setKeyRight(KeyCodes input_key) {
			this.key_right = input_key;
		}
		public void setKeyUp(KeyCodes input_key) {
			this.key_up = input_key;
		}
		public void setKeyDown(KeyCodes input_key) {
			this.key_down = input_key;
		}
	}
}
