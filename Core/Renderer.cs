using System.Collections.Generic;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;

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
			LookupTexture("Sphere.jpg");

			_shaderEffects["effect2"] = new ShaderEffect(
				new[] {
					new EffectPassDeclaration {
						VS = AssetStorage.Get<string>("VertexShader2.vert"),
						PS = AssetStorage.Get<string>("PixelShader2.frag"),
						StateSet = new RenderStateSet {
							ZEnable = true,
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
}
