using Fusee.Engine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.FuFiCycles.Core {
	class InputKeys {
		private KeyCodes key_left;
		private KeyCodes key_right;

		public InputKeys(KeyCodes left, KeyCodes right) {
			setKeyLeft(left);
			setKeyRight(right);
		}

		public KeyCodes getKeyLeft() {
			return this.key_left;
		}

		public KeyCodes getKeyRight() {
			return this.key_right;
		}

		public void setKeyLeft(KeyCodes input_key) {
			this.key_left = input_key;
		}

		public void setKeyRight(KeyCodes input_key) {
			this.key_right = input_key;
		}
	}
}
