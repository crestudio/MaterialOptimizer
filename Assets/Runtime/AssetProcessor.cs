#if UNITY_EDITOR
using System;
using System.Linq;

using UnityEditor;
using UnityEngine;

using static com.vrsuya.materialoptimizer.TextureReplacer;

/*
 * VRSuya MaterialOptimizer AssetProcessor
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace com.vrsuya.materialoptimizer {

	[ExecuteInEditMode]
	[AddComponentMenu("")]
	public class AssetProcessor {

		/// <summary>�־��� �ƹ�Ÿ���� �ؽ��ĵ��� �����ɴϴ�.</summary>
		public Texture2D[] AddAvatarTextures(GameObject TargetGameObject) {
			Texture2D[] newAvatarTextures = new Texture2D[0];
			Material[] AvatarMaterials = GetAvatarMaterials(TargetGameObject);
			if (AvatarMaterials.Length > 0) {
				foreach (Material TargetMaterial in AvatarMaterials) {
					newAvatarTextures = newAvatarTextures.Concat(GetMaterialTextures(TargetMaterial)).Distinct().ToArray();
				}
				Array.Sort(newAvatarTextures, (a, b) => string.Compare(a.name, b.name, StringComparison.Ordinal));
			}
			return newAvatarTextures;
		}

		/// <summary>�־��� �ƹ�Ÿ���� �ؽ��� �����ϵ��� �����ɴϴ�.</summary>
		public TextureExpression[] AddAvatarTextureDetails(GameObject TargetGameObject) {
			TextureExpression[] newAvatarTextureExpressions = new TextureExpression[0];
			Material[] AvatarMaterials = GetAvatarMaterials(TargetGameObject);
			if (AvatarMaterials.Length > 0) {
				foreach (Material TargetMaterial in AvatarMaterials) {
					newAvatarTextureExpressions = newAvatarTextureExpressions.Concat(GetMaterialTextureDetails(TargetMaterial)).ToArray();
				}
				Array.Sort(newAvatarTextureExpressions, (a, b) => string.Compare(a.BeforeTexture.name, b.BeforeTexture.name, StringComparison.Ordinal));
			}
			return newAvatarTextureExpressions;
		}

		/// <summary>�־��� �ƹ�Ÿ���� ���׸������ �����ͼ� ��ȯ�մϴ�.</summary>
		/// <returns>�ƹ�Ÿ�� ���Ե� ���׸��� ���</returns>
		public Material[] GetAvatarMaterials(GameObject TargetGameObject) {
			Material[] AvatarMaterials = new Material[0];
			SkinnedMeshRenderer[] AvatarSkinnedMeshRenderers = TargetGameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
			MeshRenderer[] AvatarMeshRenderers = TargetGameObject.GetComponentsInChildren<MeshRenderer>(true);
			if (AvatarSkinnedMeshRenderers.Length > 0) {
				AvatarMaterials = AvatarMaterials.Concat(AvatarSkinnedMeshRenderers.SelectMany(AvatarSkinnedMeshRenderer => AvatarSkinnedMeshRenderer.sharedMaterials).ToArray()).ToArray();
			}
			if (AvatarMeshRenderers.Length > 0) {
				AvatarMaterials = AvatarMaterials.Concat(AvatarMeshRenderers.SelectMany(AvatarMeshRenderer => AvatarMeshRenderer.sharedMaterials).ToArray()).ToArray();
			}
			AvatarMaterials = AvatarMaterials.Distinct().ToArray();
			return AvatarMaterials;
		}

		/// <summary>�־��� ���׸��󿡼� �ؽ��ĵ��� �����ͼ� ��ȯ�մϴ�.</summary>
		/// <returns>���׸��� ���Ե� �ؽ��� ���</returns>
		public Texture2D[] GetMaterialTextures(Material TargetMaterial) {
			Texture2D[] MaterialTexture2Ds = new Texture2D[0];
			if (TargetMaterial) {
				Shader TargetShader = TargetMaterial.shader;
				int PropertyCount = ShaderUtil.GetPropertyCount(TargetShader);
				for (int Index = 0; Index < PropertyCount; Index++) {
					if (ShaderUtil.GetPropertyType(TargetShader, Index) == ShaderUtil.ShaderPropertyType.TexEnv) {
						string PropertyName = ShaderUtil.GetPropertyName(TargetShader, Index);
						Texture MaterialTexture = TargetMaterial.GetTexture(PropertyName);
						if (MaterialTexture is Texture2D) {
							MaterialTexture2Ds = MaterialTexture2Ds.Concat(new Texture2D[] { (Texture2D)MaterialTexture }).ToArray();
						}
					}
				}
			}
			MaterialTexture2Ds = MaterialTexture2Ds.Distinct().ToArray();
			return MaterialTexture2Ds;
		}

		/// <summary>�־��� ���׸��󿡼� �ؽ��ĵ��� �����ͼ� ��ȯ�մϴ�.</summary>
		/// <returns>���׸��� ���Ե� �ؽ��� ���</returns>
		public TextureExpression[] GetMaterialTextureDetails(Material TargetMaterial) {
			TextureExpression[] MaterialTextureExpressions = new TextureExpression[0];
			if (TargetMaterial) {
				Shader TargetShader = TargetMaterial.shader;
				int PropertyCount = ShaderUtil.GetPropertyCount(TargetShader);
				for (int Index = 0; Index < PropertyCount; Index++) {
					if (ShaderUtil.GetPropertyType(TargetShader, Index) == ShaderUtil.ShaderPropertyType.TexEnv) {
						string PropertyName = ShaderUtil.GetPropertyName(TargetShader, Index);
						Texture MaterialTexture = TargetMaterial.GetTexture(PropertyName);
						if (MaterialTexture is Texture2D) {
							MaterialDetail newMaterialDetail = new MaterialDetail() {
								OriginMaterial = TargetMaterial,
								PropertyName = new string[] { PropertyName }
							};
							TextureExpression newTextureExpression = new TextureExpression() {
								ShowDetails = false,
								BeforeTexture = (Texture2D)MaterialTexture,
								AfterTexture = (Texture2D)MaterialTexture,
								OriginMaterial = new MaterialDetail[] { newMaterialDetail }
							};
							MaterialTextureExpressions = MaterialTextureExpressions.Concat(new TextureExpression[] { newTextureExpression }).ToArray();
						}
					}
				}
			}
			return MaterialTextureExpressions;
		}
	}
}
#endif