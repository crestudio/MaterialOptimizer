﻿#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;

/*
 * VRSuya MaterialOptimizer TextureReplacer
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace com.vrsuya.materialoptimizer {

	[ExecuteInEditMode]
	[AddComponentMenu("VRSuya Texture Replacer")]
	public class TextureReplacer : MonoBehaviour {

		[Serializable]
		public struct TextureExpression {
			public Texture2D BeforeTexture;
			public Texture2D AfterTexture;

			public TextureExpression(Texture2D ExistTexture, Texture2D NewTexture) {
				BeforeTexture = ExistTexture;
				AfterTexture = NewTexture;
			}
		};

		[SerializeField]
		public List<TextureExpression> AvatarTextureList = new List<TextureExpression>();
		private List<TextureExpression> TargetTextureList = new List<TextureExpression>();

		public GameObject AvatarGameObject = null;
		public Material[] AvatarMaterials = new Material[0];

		public int UndoGroupIndex;

		// 컴포넌트 최초 로드시 동작
		void OnEnable() {
			RefreshAvatarProprety();
			return;
		}

		/// <summary>
		/// 본 프로그램의 메인 세팅 로직입니다.
		/// </summary>
		public void ChangeAvatarTextures() {
			Undo.IncrementCurrentGroup();
			Undo.SetCurrentGroupName("VRSuya Texture Replacer");
			UndoGroupIndex = Undo.GetCurrentGroup();
			TargetTextureList = CleanupAvatarTextureList();
			if (AvatarMaterials.Length > 0 && TargetTextureList.Count > 0) ChangeTexture2Ds();
			return;
		}

		/// <summary>주어진 아바타가 사용하고 있는 머테리얼, 텍스쳐를 추출합니다.</summary>
		public void RefreshAvatarProprety() {
			if (!AvatarGameObject) {
				AvatarGameObject = this.gameObject;
			}
			AvatarMaterials = GetAvatarMaterials(AvatarGameObject);
			AvatarTextureList = GetAvatarTextures(AvatarGameObject);
		}

		/// <summary>주어진 GameObject가 사용하고 있는 텍스쳐를 추출합니다.</summary>
		/// <returns>GameObject가 사용하고 있는 Texture2D 리스트</returns>
		private List<TextureExpression> GetAvatarTextures(GameObject TargetGameObject) {
			AssetProcessor AssetProcessorInstance = new AssetProcessor();
			Texture2D[] newAvatarTexture2Ds = AssetProcessorInstance.AddAvatarTextures(TargetGameObject);
			List<TextureExpression> newAvatarTextureList = new List<TextureExpression>();
			for (int Index = 0; Index < newAvatarTexture2Ds.Length;	Index++) {
				newAvatarTextureList.Add(new TextureExpression(newAvatarTexture2Ds[Index], newAvatarTexture2Ds[Index]));
			}
			return newAvatarTextureList;
		}

		/// <summary>주어진 GameObject가 사용하고 있는 머테리얼를 추출합니다.</summary>
		/// <returns>GameObject가 사용하고 있는 머테리얼 어레이</returns>
		private Material[] GetAvatarMaterials(GameObject TargetGameObject) {
			AssetProcessor AssetProcessorInstance = new AssetProcessor();
			Material[] newAvatarMaterials = AssetProcessorInstance.GetAvatarMaterials(TargetGameObject);
			return newAvatarMaterials;
		}

		/// <summary>주어진 Texture2D 리스트에서 서로 다른 값만 추출합니다.</summary>
		/// <returns>서로 다른 값을 가진 Texture2D 리스트</returns>
		private List<TextureExpression> CleanupAvatarTextureList() {
			List<TextureExpression> newTargetTextureList = new List<TextureExpression>();
			foreach (TextureExpression TargetExpression in AvatarTextureList) {
				if (TargetExpression.BeforeTexture != TargetExpression.AfterTexture) {
					newTargetTextureList.Add(TargetExpression);
				}
			}
			return newTargetTextureList;
		}

		/// <summary>주어진 머테리얼에서 규칙에 맞춰서 Texture2D를 변경합니다.</summary>
		private void ChangeTexture2Ds() {
			int ChangedCount = 0;
			Texture2D[] TargetTexture2Ds = TargetTextureList.Select(TargetTexture => TargetTexture.BeforeTexture).ToArray();
			foreach (Material TargetMaterial in AvatarMaterials) {
				if (TargetMaterial) {
					Shader TargetShader = TargetMaterial.shader;
					int PropertyCount = ShaderUtil.GetPropertyCount(TargetShader);
					for (int Index = 0; Index < PropertyCount; Index++) {
						if (ShaderUtil.GetPropertyType(TargetShader, Index) == ShaderUtil.ShaderPropertyType.TexEnv) {
							string PropertyName = ShaderUtil.GetPropertyName(TargetShader, Index);
							Texture ExistMaterialTexture = TargetMaterial.GetTexture(PropertyName);
							if (ExistMaterialTexture is Texture2D) {
								if (Array.Exists(TargetTexture2Ds, TargetTexture => ExistMaterialTexture == TargetTexture)) {
									Undo.RecordObject(TargetMaterial, "Change Texture2D");
									Texture2D newTexture2D = TargetTextureList
										.Where(TargetTextureExpression => ExistMaterialTexture == TargetTextureExpression.BeforeTexture)
										.Select(TargetTextureExpression => TargetTextureExpression.AfterTexture).ToArray()[0];
									TargetMaterial.SetTexture(PropertyName, newTexture2D);
									EditorUtility.SetDirty(TargetMaterial);
									Undo.CollapseUndoOperations(UndoGroupIndex);
									ChangedCount++;
								}
							}
						}
					}
				}
			}
			Debug.Log("[Texture Replacer] 주어진 머테리얼에서 텍스쳐를 총 " + ChangedCount + "건을 교체하였습니다!");
			return;
		}
	}
}
#endif