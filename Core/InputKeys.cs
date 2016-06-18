using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.FuFiCycles.Core {
	class InputKeys {
		private String key_left;
		private String key_right;

		public InputKeys(String left, String right) {
			setKeyLeft(left);
			setKeyRight(right);
		}

		public String getKeyLeft() {
			return this.key_left;
		}

		public String getKeyRight() {
			return this.key_right;
		}

		public void setKeyLeft(String input_key) {
			this.key_left = input_key;
		}

		public void setKeyRight(String input_key) {
			this.key_right = input_key;
		}
	}
}
