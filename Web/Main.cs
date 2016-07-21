using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using System.Diagnostics;
using System.Linq;
using static Fusee.FuFiCycles.Core.GameSettings;

namespace Fusee.FuFiCycles.Web {
	public class FuFiCycles {
		public static void Main() {
			// Inject Fusee.Engine.Base InjectMe dependencies
			IO.IOImp = new Fusee.Base.Imp.Web.IOImp();

			var fap = new Fusee.Base.Imp.Web.WebAssetProvider();
			fap.RegisterTypeHandler(
				new AssetHandler {
					ReturnedType = typeof(Font),
					Decoder = delegate (string id, object storage) {
						if (Path.GetExtension(id).ToLower().Contains("ttf"))
							return new Font {
								_fontImp = new Fusee.Base.Imp.Web.FontImp(storage)
							};
						return null;
					},
					Checker = delegate (string id) {
						return Path.GetExtension(id).ToLower().Contains("ttf");
					}
				});
			fap.RegisterTypeHandler(
				new AssetHandler {
					ReturnedType = typeof(SceneContainer),
					Decoder = delegate (string id, object storage) {
						if (Path.GetExtension(id).ToLower().Contains("fus")) {
							var ser = new Serializer();
							return ser.Deserialize(IO.StreamFromFile("Assets/" + id, FileMode.Open), null, typeof(SceneContainer)) as SceneContainer;
						}
						return null;
					},
					Checker = delegate (string id) {
						return Path.GetExtension(id).ToLower().Contains("fus");
					}
				});
			AssetStorage.RegisterProvider(fap);

			var app = new Fusee.FuFiCycles.Core.FuFiCycles();

			// Inject Fusee.Engine InjectMe dependencies (hard coded)
			app.CanvasImplementor = new Fusee.Engine.Imp.Graphics.Web.RenderCanvasImp();
			app.ContextImplementor = new Fusee.Engine.Imp.Graphics.Web.RenderContextImp(app.CanvasImplementor);
			Input.AddDriverImp(new Fusee.Engine.Imp.Graphics.Web.RenderCanvasInputDriverImp(app.CanvasImplementor));
			// app.AudioImplementor = new Fusee.Engine.Imp.Sound.Web.AudioImp();
			// app.NetworkImplementor = new Fusee.Engine.Imp.Network.Web.NetworkImp();
			// app.InputDriverImplementor = new Fusee.Engine.Imp.Input.Web.InputDriverImp();
			// app.VideoManagerImplementor = ImpFactory.CreateIVideoManagerImp();

			// Start the app
			app.Run();
		}
		public static void setPlayerName(int id, string name) {
			if(id == 1) {
				PLAYER1_NAME = name;
				Debug.WriteLine("SET PLAYER1_NAME TO: " + PLAYER1_NAME);
			} else if(id == 2) {
				PLAYER2_NAME = name;
				Debug.WriteLine("SET PLAYER2_NAME TO: " + PLAYER2_NAME);
			}
			INSTANCE.getIngameGui().refreshNames();
		}

		public static void setCycleColor(int id, byte c1, byte c2, byte c3) {
			if (id == 1) {
				CYCLE1_COLOR = new float3(c1, c2, c3);
				Debug.WriteLine("SET CYCLE1_COLOR TO: " + CYCLE1_COLOR);
			} else if (id == 2) {
				CYCLE2_COLOR = new float3(c1, c2, c3);
				Debug.WriteLine("SET CYCLE2_COLOR TO: " + CYCLE2_COLOR);
			}
		}

		public static void setSpeed(int speed) {
			if (speed >= 1 && speed <= 100) {
				SPEED = speed;
				Debug.WriteLine("SET SPEED TO " + SPEED);
				for(int i = 0; i < INSTANCE.getLastMatch().getPlayers().Count(); i++) {
					if(!INSTANCE.getLastMatch().getPlayers()[i].getCycle().isCollided()) {
						INSTANCE.getLastMatch().getPlayers()[i].getCycle().setSpeed(SPEED);
					}
				}
			}
		}

		public static void setWEB() {
			WEB = true;
			Debug.WriteLine(WEB);
		}
	}
}
