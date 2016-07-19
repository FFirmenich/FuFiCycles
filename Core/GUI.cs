using Fusee.Base.Core;
using Fusee.Engine.Core;
using Fusee.Engine.Core.GUI;
using static Fusee.FuFiCycles.Core.GameSettings;


namespace Fusee.FuFiCycles.Core {
	public abstract class GUI {
		private GUIHandler guiHandler = new GUIHandler();
		private FontMap roboto = new FontMap(INSTANCE.roboto, 20);

		public GUI() {
			getGUIHandler().AttachToContext(INSTANCE.getRC());
			INSTANCE.roboto.UseKerning = true;
		}

		public GUIHandler getGUIHandler() {
			return guiHandler;
		}

		public FontMap getRoboto() {
			return roboto;
		}
	}
}
