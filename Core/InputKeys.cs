using Fusee.Engine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.FuFiCycles.Core {
	public class InputKeys {
		private KeyCodes left;
		private KeyCodes right;
		private KeyCodes up;
		private KeyCodes down;

		public InputKeys(KeyCodes left, KeyCodes right, KeyCodes up, KeyCodes down) {
			setKeyLeft(left);
			setKeyRight(right);
			setKeyUp(up);
			setKeyDown(down);
		}

		public KeyCodes getKeyLeft() {
			return this.left;
		}

		public KeyCodes getKeyRight() {
			return this.right;
		}

		public KeyCodes getKeyUp() {
			return this.up;
		}

		public KeyCodes getKeyDown() {
			return this.down;
		}

		public void setKeyLeft(KeyCodes input_key) {
			this.left = input_key;
		}

		public void setKeyRight(KeyCodes input_key) {
			this.right = input_key;
		}
		public void setKeyUp(KeyCodes input_key) {
			this.up = input_key;
		}
		public void setKeyDown(KeyCodes input_key) {
			this.down = input_key;
		}
	}
}
