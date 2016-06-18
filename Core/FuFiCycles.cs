using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using static System.Math;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;
using System.Diagnostics;

namespace Fusee.FuFiCycles.Core {

	class Renderer : SceneVisitor {
		public ShaderEffect ShaderEffect;

		public RenderContext RC;
		public float4x4 View;
		private Dictionary<MeshComponent, Mesh> _meshes = new Dictionary<MeshComponent, Mesh>();
		private Dictionary<string, ITexture> _textures = new Dictionary<string, ITexture>();
		public Dictionary<string, ShaderEffect> _shaderEffects = new Dictionary<string, ShaderEffect>();
		private CollapsingStateStack<float4x4> _model = new CollapsingStateStack<float4x4>();
		private Mesh LookupMesh(MeshComponent mc) {
			Mesh mesh;
			if (!_meshes.TryGetValue(mc, out mesh)) {
				mesh = new Mesh {
					Vertices = mc.Vertices,
					Normals = mc.Normals,
					UVs = mc.UVs,
					Triangles = mc.Triangles,
				};
				_meshes[mc] = mesh;
			}
			return mesh;
		}
		private ImageData LookupTexture(string textureName) {
			ImageData textureData = AssetStorage.Get<ImageData>(textureName);
			ITexture texture;
			if (!_textures.TryGetValue(textureName, out texture)) {
				texture = RC.CreateTexture(textureData);
				_textures[textureName] = texture;
			}

			return textureData;
		}

		public Renderer(RenderContext rc) {
			RC = rc;
			LookupTexture("Leaves.jpg");
			LookupTexture("Sphere.jpg");
			_shaderEffects["effect1"] = new ShaderEffect(
				new[] {
					new EffectPassDeclaration {
						VS = AssetStorage.Get<string>("VertexShader.vert"),
						PS = AssetStorage.Get<string>("PixelShader.frag"),
						StateSet = new RenderStateSet {
							ZEnable = true,
							CullMode = Cull.Counterclockwise,
						}
					}
				},
				new[] {
					new EffectParameterDeclaration {Name="albedo", Value = float3.One},
					new EffectParameterDeclaration {Name="shininess", Value = 1.0f},
					new EffectParameterDeclaration {Name="specfactor", Value = 1.0f},
					new EffectParameterDeclaration {Name="speccolor", Value = float3.Zero},
					new EffectParameterDeclaration {Name="ambientcolor", Value = float3.Zero},
					new EffectParameterDeclaration {Name="texture", Value = _textures["Leaves.jpg"]},
					new EffectParameterDeclaration {Name="texmix", Value = 0.0f},
				});

			_shaderEffects["effect2"] = new ShaderEffect(
				new[] {
					new EffectPassDeclaration {
						VS = AssetStorage.Get<string>("VertexShader2.vert"),
						PS = AssetStorage.Get<string>("PixelShader2.frag"),
						StateSet = new RenderStateSet {
							ZEnable = true,
							CullMode = Cull.Counterclockwise,
						}
					}
				},

				new[] {
					new EffectParameterDeclaration {Name="albedo", Value = float3.One},
					new EffectParameterDeclaration {Name="shininess", Value = 1.0f},
					new EffectParameterDeclaration {Name="specfactor", Value = 1.0f},
					new EffectParameterDeclaration {Name="speccolor", Value = float3.Zero},
					new EffectParameterDeclaration {Name="ambientcolor", Value = float3.Zero},
					new EffectParameterDeclaration {Name="texture", Value = _textures["Sphere.jpg"]},
					new EffectParameterDeclaration {Name="texmix", Value = 0.0f},
					});

			_shaderEffects["effect1"].AttachToContext(RC);
			_shaderEffects["effect2"].AttachToContext(RC);

			ShaderEffect = _shaderEffects["effect2"];
		}

		protected override void InitState() {
			_model.Clear();
			_model.Tos = float4x4.Identity;
		}
		protected override void PushState() {
			_model.Push();
		}
		protected override void PopState() {
			_model.Pop();
			RC.ModelView = View * _model.Tos;
		}
		[VisitMethod]
		void OnMesh(MeshComponent mesh) {
			ShaderEffect.RenderMesh(LookupMesh(mesh));
			// RC.Render(LookupMesh(mesh));
			
			//ShaderEffect.RenderMesh(LookupMesh(mesh));
			// RC.Render(LookupMesh(mesh));
			//if (CurrentNode.Name.Contains("Tree"))
			//{
			//    ShaderEffectBlack.RenderMesh(LookupMesh(mesh));
			//    ShaderEffect.RenderMesh(LookupMesh(mesh));
			//}else {
			//    ShaderEffect.RenderMesh(LookupMesh(mesh));
			//}
			ShaderEffect currentShader;
			if (_shaderEffects.TryGetValue(CurrentNode.Name, out currentShader)) {
				currentShader.RenderMesh(LookupMesh(mesh));
			} else {
				ShaderEffect.RenderMesh(LookupMesh(mesh));
			}
		}
		[VisitMethod]
		void OnMaterial(MaterialComponent material) {
			if (material.HasDiffuse) {
				ShaderEffect.SetEffectParam("albedo", material.Diffuse.Color);
				if (material.Diffuse.Texture == "Leaves.jpg") {
					ShaderEffect.SetEffectParam("texture", _textures["Leaves.jpg"]);
					ShaderEffect.SetEffectParam("texmix", 1.0f);
				} else {
					ShaderEffect.SetEffectParam("texmix", 0.0f);
				}
			} else {
				ShaderEffect.SetEffectParam("albedo", float3.Zero);
			}
			if (material.HasSpecular) {
				ShaderEffect.SetEffectParam("shininess", material.Specular.Shininess);
				ShaderEffect.SetEffectParam("specfactor", material.Specular.Intensity);
				ShaderEffect.SetEffectParam("speccolor", material.Specular.Color);
			} else {
				ShaderEffect.SetEffectParam("shininess", 0);
				ShaderEffect.SetEffectParam("specfactor", 0);
				ShaderEffect.SetEffectParam("speccolor", float3.Zero);
			}
			if (material.HasEmissive) {
				ShaderEffect.SetEffectParam("ambientcolor", material.Emissive.Color);
			} else {
				ShaderEffect.SetEffectParam("ambientcolor", float3.Zero);
			}
		}
		[VisitMethod]
		void OnTransform(TransformComponent xform) {
			_model.Tos *= xform.Matrix();
			RC.ModelView = View * _model.Tos;
		}
	}


	[FuseeApplication(Name = "FuFiCycles", Description = "A FuFi Production")]
	public class FuFiCycles : RenderCanvas {
		// angle variables
		private static float _angleHorz = M.PiOver6 * 2.0f, _angleVert = -M.PiOver6 * 0.5f,
							 _angleVelHorz, _angleVelVert, _angleRoll, _angleRollInit, _zoomVel, _zoom;
		private static float2 _offset;
		private static float2 _offsetInit;

		private const float RotationSpeed = 7;
		private const float Damping = 0.8f;

		private SceneContainer _scene;
		private float4x4 _sceneScale;
		private float4x4 _projection;
		private bool _twoTouchRepeated;

		private bool _keys;

		private TransformComponent _cycleTransform;
		private TransformComponent _cycleWheelR;
		private TransformComponent _cycleWheelL;
		private TransformComponent _cycleWall;

		private Dictionary<float3, int> _cyclePositions = new Dictionary<float3, int>();
		
		private Renderer _renderer;

		private bool aPressed = false;
		private bool dPressed = false;


		// Init is called on startup. 
		public override void Init() {
			// Load the scene
			_scene = AssetStorage.Get<SceneContainer>("CycleLand.fus");
			_sceneScale = float4x4.CreateScale(0.04f);

			// Instantiate our self-written renderer
			_renderer = new Renderer(RC);

			// Find some transform nodes we want to manipulate in the scene
			_cycleTransform = _scene.Children.FindNodes(c => c.Name == "bike").First()?.GetTransform();
			_cycleWheelR = _scene.Children.FindNodes(c => c.Name == "wheelback").First()?.GetTransform();
			_cycleWheelL = _scene.Children.FindNodes(c => c.Name == "wheelfront").First()?.GetTransform();
			_cycleWall = _scene.Children.FindNodes(c => c.Name == "Wall").First()?.GetTransform();

			// Set the clear color for the backbuffer
			RC.ClearColor = new float4(1, 1, 1, 1);
		}

		// RenderAFrame is called once a frame
		public override void RenderAFrame() {
			// Clear the backbuffer
			RC.Clear(ClearFlags.Color | ClearFlags.Depth);

			// Mouse and keyboard movement
			if (Keyboard.LeftRightAxis != 0 || Keyboard.UpDownAxis != 0) {
				_keys = true;
			}

			var curDamp = (float)System.Math.Exp(-Damping * DeltaTime);

			// Zoom & Roll
			if (Touch.TwoPoint) {
				if (!_twoTouchRepeated) {
					_twoTouchRepeated = true;
					_angleRollInit = Touch.TwoPointAngle - _angleRoll;
					_offsetInit = Touch.TwoPointMidPoint - _offset;
				}
				_zoomVel = Touch.TwoPointDistanceVel * -0.01f;
				_angleRoll = Touch.TwoPointAngle - _angleRollInit;
				_offset = Touch.TwoPointMidPoint - _offsetInit;
			} else {
				_twoTouchRepeated = false;
				_zoomVel = Mouse.WheelVel * -0.5f;
				_angleRoll *= curDamp * 0.8f;
				_offset *= curDamp * 0.8f;
			}

			// UpDown / LeftRight rotation
			if (Mouse.LeftButton) {
				_keys = false;
				_angleVelHorz = -RotationSpeed * Mouse.XVel * 0.000002f;
				_angleVelVert = -RotationSpeed * Mouse.YVel * 0.000002f;
			} else if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint) {
				_keys = false;
				float2 touchVel;
				touchVel = Touch.GetVelocity(TouchPoints.Touchpoint_0);
				_angleVelHorz = -RotationSpeed * touchVel.x * 0.000002f;
				_angleVelVert = -RotationSpeed * touchVel.y * 0.000002f;
			} else {
				if (_keys) {
					_angleVelHorz = -RotationSpeed * Keyboard.LeftRightAxis * 0.002f;
					_angleVelVert = -RotationSpeed * Keyboard.UpDownAxis * 0.002f;
				} else {
					_angleVelHorz *= curDamp;
					_angleVelVert *= curDamp;
				}
			}

			float speed = 10;

			// Wuggy XForm
			float cycleYaw = _cycleTransform.Rotation.y;
			if (Keyboard.GetKey(KeyCodes.A)) {
				if (aPressed == false) {
					cycleYaw = cycleYaw - M.PiOver2;
				}
				aPressed = true;
			} else {
				aPressed = false;
			}
			if (Keyboard.GetKey(KeyCodes.D)) {
				if (dPressed == false) {
					cycleYaw = cycleYaw + M.PiOver2;
				}
				dPressed = true;
			} else {
				dPressed = false;
			}
			cycleYaw = NormRot(cycleYaw);
			float3 cyclePos = _cycleTransform.Translation;
			cyclePos += new float3((float)Sin(cycleYaw), 0, (float)Cos(cycleYaw)) * speed;
			_cycleTransform.Rotation = new float3(0, cycleYaw, 0);
			_cycleTransform.Translation = cyclePos;

			// Wuggy Wheels
			_cycleWheelR.Rotation += new float3(speed * 0.008f, 0, 0);
			_cycleWheelL.Rotation += new float3(speed * 0.008f, 0, 0);

			//fill hashmap of positions
			int reserved;
			if(!_cyclePositions.TryGetValue(cyclePos, out reserved)) {
				_cyclePositions[cyclePos] = 1;
			} else {
				Debug.WriteLine("crash");
			}
			

			// draw wall 
			/*
			_cycleWall.Rotation = new float3(0, cycleYaw, 0);
			float val = 0.1f;
			if (cycleYaw < M.PiOver2 + val && cycleYaw > M.PiOver2 - val) {
				_cycleWall.Translation.x = cyclePos.x - 200;
				_cycleWall.Translation.z = cyclePos.z;
			} else if(cycleYaw > -val && cycleYaw < val) {
				_cycleWall.Translation.x = cyclePos.x;
				_cycleWall.Translation.z = cyclePos.z - 200;
			} else if(cycleYaw < -M.PiOver2 + val && cycleYaw > -M.PiOver2 - val) {
				_cycleWall.Translation.x = cyclePos.x + 200;
				_cycleWall.Translation.z = cyclePos.z;
			} else if(cycleYaw > M.Pi - val && cycleYaw < M.Pi + val) {
				_cycleWall.Translation.x = cyclePos.x;
				_cycleWall.Translation.z = cyclePos.z + 200;
			}
			_cycleWall.Scale.z = _cycleWall.Scale.z - 0.05f;
			_cycleWall.Translation.z = _cycleWall.Translation.z + 5;
			*/

			

			_zoom += _zoomVel;
			// Limit zoom
			if (_zoom < 80)
				_zoom = 80;
			if (_zoom > 2000)
				_zoom = 2000;

			_angleHorz += _angleVelHorz;
			// Wrap-around to keep _angleHorz between -PI and + PI
			_angleHorz = M.MinAngle(_angleHorz);

			_angleVert += _angleVelVert;
			// Limit pitch to the range between [-PI/2, + PI/2]
			_angleVert = M.Clamp(_angleVert, -M.PiOver2, M.PiOver2);

			// Wrap-around to keep _angleRoll between -PI and + PI
			_angleRoll = M.MinAngle(_angleRoll);


			// Create the camera matrix and set it as the current ModelView transformation
			var mtxRot = float4x4.CreateRotationZ(_angleRoll) * float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
			var mtxCam = float4x4.LookAt(0, 20, -_zoom, 0, 0, 0, 0, 1, 0);
			_renderer.View = mtxCam * mtxRot * _sceneScale;
			var mtxOffset = float4x4.CreateTranslation(2 * _offset.x / Width, -2 * _offset.y / Height, 0);
			RC.Projection = mtxOffset * _projection;


			_renderer.Traverse(_scene.Children);

			// Swap buffers: Show the contents of the backbuffer (containing the currently rerndered farame) on the front buffer.
			Present();

		}

		public static float NormRot(float rot) {
			while (rot > M.Pi)
				rot -= M.TwoPi;
			while (rot < -M.Pi)
				rot += M.TwoPi;
			return rot;
		}



		// Is called when the window was resized
		public override void Resize() {
			// Set the new rendering area to the entire new windows size
			RC.Viewport(0, 0, Width, Height);

			// Create a new projection matrix generating undistorted images on the new aspect ratio.
			var aspectRatio = Width / (float)Height;

			// 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
			// Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
			// Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
			_projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 20000);
		}

	}
}