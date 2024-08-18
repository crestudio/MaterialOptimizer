#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

using VRC.SDK3.Avatars.Components;
using static com.vrsuya.materialoptimizer.TextureReplacer;

/*
 * VRSuya MaterialOptimizer AssetProcessor
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace com.vrsuya.materialoptimizer {

	[ExecuteInEditMode]
	[AddComponentMenu("")]
	public class AssetProcessor {

		/// <summary>주어진 아바타에서 텍스쳐들을 가져옵니다.</summary>
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

		/// <summary>주어진 아바타에서 텍스쳐 디테일들을 가져옵니다.</summary>
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

		/// <summary>주어진 아바타에서 머테리얼들을 가져와서 반환합니다.</summary>
		/// <returns>아바타에 포함된 머테리얼 어레이</returns>
		public Material[] GetAvatarMaterials(GameObject TargetGameObject) {
			List<Material> AvatarMaterials = new List<Material>();
			SkinnedMeshRenderer[] AvatarSkinnedMeshRenderers = TargetGameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
			MeshRenderer[] AvatarMeshRenderers = TargetGameObject.GetComponentsInChildren<MeshRenderer>(true);
			Material[] AnimationMaterials = GetAnimationMaterials(TargetGameObject);
			if (AvatarSkinnedMeshRenderers.Length > 0) {
				AvatarMaterials.AddRange(AvatarSkinnedMeshRenderers.SelectMany(AvatarSkinnedMeshRenderer => AvatarSkinnedMeshRenderer.sharedMaterials));
			}
			if (AvatarMeshRenderers.Length > 0) {
				AvatarMaterials.AddRange(AvatarMeshRenderers.SelectMany(AvatarMeshRenderer => AvatarMeshRenderer.sharedMaterials));
			}
			if (AnimationMaterials.Length > 0) {
				AvatarMaterials.AddRange(AnimationMaterials);
			}
			AvatarMaterials = AvatarMaterials.Distinct().ToList();
			AvatarMaterials.Sort((a, b) => string.Compare(a.name, b.name, StringComparison.Ordinal));
			return AvatarMaterials.ToArray();
		}

		/// <summary>주어진 아바타에서 애니메이션에서 사용된 머테리얼들을 가져와서 반환합니다.</summary>
		/// <returns>아바타에 포함된 애니메이션 머테리얼 어레이</returns>
		private Material[] GetAnimationMaterials(GameObject TargetGameObject) {
			List<Material> AnimationMaterials = new List<Material>();
			TargetGameObject.TryGetComponent(typeof(VRCAvatarDescriptor), out Component AvatarDescriptor);
			if (AvatarDescriptor) {
				VRCAvatarDescriptor VRCDescriptor = AvatarDescriptor.GetComponent<VRCAvatarDescriptor>();
				VRCAvatarDescriptor.CustomAnimLayer AvatarFXLayer = Array.Find(VRCDescriptor.baseAnimationLayers, AnimationLayer => AnimationLayer.type == VRCAvatarDescriptor.AnimLayerType.FX);
				if (AvatarFXLayer.animatorController) {
					AnimationClip[] AllAnimationClips = GetFXAnimationClips((AnimatorController)AvatarFXLayer.animatorController);
					foreach (AnimationClip TargetAnimationClip in AllAnimationClips) {
						foreach (EditorCurveBinding Binding in AnimationUtility.GetObjectReferenceCurveBindings(TargetAnimationClip)) {
							if (Binding.type == typeof(SkinnedMeshRenderer) || Binding.type == typeof(MeshRenderer)) {
								ObjectReferenceKeyframe[] ObjectKeyframes = AnimationUtility.GetObjectReferenceCurve(TargetAnimationClip, Binding);
								foreach (var ObjectKeyframe in ObjectKeyframes) {
									if (ObjectKeyframe.value is Material) {
										AnimationMaterials.Add((Material)ObjectKeyframe.value);
									}
								}
							}
						}
					}
				}
			}
			AnimationMaterials = AnimationMaterials.Distinct().ToList();
			AnimationMaterials.Sort((a, b) => string.Compare(a.name, b.name, StringComparison.Ordinal));
			return AnimationMaterials.ToArray();
		}

		/// <summary>모든 FXLayer의 AnimationClip 어레이를 반환합니다.</summary>
		/// <returns>FXLayer의 AnimationClip 어레이</returns>
		private AnimationClip[] GetFXAnimationClips(AnimatorController TargetAnimatorController) {
			List<AnimatorStateMachine> RootStateMachines = TargetAnimatorController.layers.Select(AnimationLayer => AnimationLayer.stateMachine).ToList();
			List<AnimatorStateMachine> AllStateMachines = new List<AnimatorStateMachine>();
			List<AnimatorState> AllAnimatorState = new List<AnimatorState>();
			List<AnimationClip> AllAnimationClips = new List<AnimationClip>();
			foreach (AnimatorStateMachine SubStateMachine in RootStateMachines) {
				AllStateMachines.AddRange(GetAllStateMachines(SubStateMachine));
			}
			foreach (AnimatorStateMachine SubStateMachine in AllStateMachines) {
				AllAnimatorState.AddRange(GetAllStates(SubStateMachine));
			}
			if (AllAnimatorState.Count > 0) {
				List<Motion> AllMotion = AllAnimatorState.Select(State => State.motion).ToList();
				foreach (Motion SubMotion in AllMotion) {
					AllAnimationClips.AddRange(GetAnimationClips(SubMotion));
				}
			}
			AllAnimationClips = AllAnimationClips.Distinct().ToList();
			AllAnimationClips.Sort((a, b) => string.Compare(a.name, b.name, StringComparison.Ordinal));
			return AllAnimationClips.ToArray();
		}

		/// <summary>모든 State 어레이를 반환합니다.</summary>
		/// <returns>State 어레이</returns>
		private AnimatorState[] GetAllStates(AnimatorStateMachine TargetStateMachine) {
			AnimatorState[] States = TargetStateMachine.states.Select(ExistChildState => ExistChildState.state).ToArray();
			if (TargetStateMachine.stateMachines.Length > 0) {
				foreach (var TargetChildStatetMachine in TargetStateMachine.stateMachines) {
					States = States.Concat(GetAllStates(TargetChildStatetMachine.stateMachine)).ToArray();
				}
			}
			return States;
		}

		/// <summary>모든 StateMachine 어레이를 반환합니다.</summary>
		/// <returns>StateMachine 어레이</returns>
		private AnimatorStateMachine[] GetAllStateMachines(AnimatorStateMachine TargetStateMachine) {
			AnimatorStateMachine[] StateMachines = new AnimatorStateMachine[] { TargetStateMachine };
			if (TargetStateMachine.stateMachines.Length > 0) {
				foreach (var TargetChildStateMachine in TargetStateMachine.stateMachines) {
					StateMachines = StateMachines.Concat(GetAllStateMachines(TargetChildStateMachine.stateMachine)).ToArray();
				}
			}
			return StateMachines;
		}

		/// <summary>모든 AnimationClip 어레이를 반환합니다.</summary>
		/// <returns>AnimationClip 어레이</returns>
		private AnimationClip[] GetAnimationClips(Motion TargetMotion) {
			AnimationClip[] MotionAnimationClips = new AnimationClip[0];
			if (TargetMotion is AnimationClip) {
				MotionAnimationClips = MotionAnimationClips.Concat(new AnimationClip[] { (AnimationClip)TargetMotion }).ToArray();
			} else if (TargetMotion is BlendTree ChildBlendTree) {
				foreach (ChildMotion ChildMotion in ChildBlendTree.children) {
					MotionAnimationClips = MotionAnimationClips.Concat(GetAnimationClips(ChildMotion.motion)).ToArray();
				}
			}
			return MotionAnimationClips;
		}

		/// <summary>주어진 머테리얼에서 텍스쳐들을 가져와서 반환합니다.</summary>
		/// <returns>머테리얼에 포함된 텍스쳐 어레이</returns>
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

		/// <summary>주어진 머테리얼에서 텍스쳐들을 가져와서 반환합니다.</summary>
		/// <returns>머테리얼에 포함된 텍스쳐 어레이</returns>
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