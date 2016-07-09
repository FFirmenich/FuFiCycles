using System.Collections.Generic;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using static Fusee.Engine.Core.Input;
using System.Linq;
using Fusee.Xene;
using System.Diagnostics;
using System;

namespace Fusee.FuFiCycles.Core {
	public class KeyboardKeys {
		// KeyboardKey List
		public Dictionary<KeyCodes, KeyboardKey> keys = new Dictionary<KeyCodes,KeyboardKey>();

		public KeyboardKeys() {
			// WASD
			keys.Add(KeyCodes.W, new KeyboardKey());
			keys.Add(KeyCodes.A, new KeyboardKey());
			keys.Add(KeyCodes.S, new KeyboardKey());
			keys.Add(KeyCodes.D, new KeyboardKey());
			// Up Left Down Right
			keys.Add(KeyCodes.Up, new KeyboardKey());
			keys.Add(KeyCodes.Left, new KeyboardKey());
			keys.Add(KeyCodes.Down, new KeyboardKey());
			keys.Add(KeyCodes.Right, new KeyboardKey());
		}

		public void renderAFrame() {
			foreach (KeyValuePair<KeyCodes, KeyboardKey> entry in keys) {
				if (Keyboard.GetKey(entry.Key)) {
					if (!entry.Value.isDown()) {
						entry.Value.setPressed();
					}
				} else {
					entry.Value.setNotDown();
				}
			}
		}
	}
}
