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
using static Fusee.FuFiCycles.Core.GameSettings;

namespace Fusee.FuFiCycles.Core {
	public class KeyboardKeys {
		// KeyboardKey List
		public Dictionary<KeyCodes, KeyboardKey> keys = new Dictionary<KeyCodes, KeyboardKey>();
		public Dictionary<KeyCodes, KeyboardKey> ingameKeys = new Dictionary<KeyCodes, KeyboardKey>();
		public Dictionary<KeyCodes, KeyboardKey> menuKeys = new Dictionary<KeyCodes, KeyboardKey>();

		public KeyboardKeys() {
			/// Player Keys
			// WASD
			ingameKeys.Add(KeyCodes.W, new KeyboardKey());
			ingameKeys.Add(KeyCodes.A, new KeyboardKey());
			ingameKeys.Add(KeyCodes.S, new KeyboardKey());
			ingameKeys.Add(KeyCodes.D, new KeyboardKey());
			// Up Left Down Right
			ingameKeys.Add(KeyCodes.Up, new KeyboardKey());
			ingameKeys.Add(KeyCodes.Left, new KeyboardKey());
			ingameKeys.Add(KeyCodes.Down, new KeyboardKey());
			ingameKeys.Add(KeyCodes.Right, new KeyboardKey());
			/// Special Keys
			keys.Add(KeyCodes.Escape, new KeyboardKey());
			keys.Add(KeyCodes.Enter, new KeyboardKey());
			keys.Add(KeyCodes.C, new KeyboardKey());
			keys.Add(KeyCodes.N, new KeyboardKey());
		}
		public void renderAFrame() {
			renderKeys();
			if (SHOWMENU) {
				renderMenuKeys();
			} else if(!INSTANCE.getLastMatch().getLastRound().isPaused()) {
				renderIngameKeys();
			}
		}
		public void renderKeys() {
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
		public void renderIngameKeys() {
			foreach (KeyValuePair<KeyCodes, KeyboardKey> entry in ingameKeys) {
				if (Keyboard.GetKey(entry.Key)) {
					if (!entry.Value.isDown()) {
						entry.Value.setPressed();
					}
				} else {
					entry.Value.setNotDown();
				}
			}
		}
		public void renderMenuKeys() {
			foreach (KeyValuePair<KeyCodes, KeyboardKey> entry in menuKeys) {
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
