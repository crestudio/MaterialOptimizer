using System;

using UnityEngine;
using UnityEditor;

/*
 * VRSuya MaterialOptimizer TextureReplacer
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace com.vrsuya.materialoptimizer {

    [CustomEditor(typeof(TextureReplacer))]
    public class TextureReplacerEditor : Editor {

		SerializedProperty SerializedAvatarTextureList;

		SerializedProperty SerializedAvatarGameObject;
		SerializedProperty SerializedAvatarMaterials;

		public static int LanguageIndex = 0;
		public readonly string[] LanguageType = new[] { "English", "한국어", "日本語" };

		void OnEnable() {
			SerializedAvatarTextureList = serializedObject.FindProperty("AvatarTextureList");

			SerializedAvatarGameObject = serializedObject.FindProperty("AvatarGameObject");
			SerializedAvatarMaterials = serializedObject.FindProperty("AvatarMaterials");
		}

        public override void OnInspectorGUI() {
			GUIStyle CenteredStyle = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter };
			GUILayoutOption PropertyWidth = GUILayout.Width((EditorGUIUtility.currentViewWidth - 15f - 50f - 30f) / 2f);
			GUILayoutOption ButtonWidth = GUILayout.Width(50);
			GUILayoutOption ArrowWidth = GUILayout.Width(15);
			TextureReplacer newTextureReplacer = (TextureReplacer)target;
			serializedObject.Update();
			LanguageIndex = EditorGUILayout.Popup(LanguageHelper.GetContextString("String_Language"), LanguageIndex, LanguageType);
			EditorGUILayout.PropertyField(SerializedAvatarGameObject, new GUIContent(LanguageHelper.GetContextString("String_TargetAvatar")));
			GUI.enabled = false;
			EditorGUILayout.PropertyField(SerializedAvatarMaterials, new GUIContent(LanguageHelper.GetContextString("String_TargetMaterial")));
			GUI.enabled = true;
			EditorGUILayout.LabelField(LanguageHelper.GetContextString("String_TargetTexture"), EditorStyles.boldLabel);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(LanguageHelper.GetContextString("String_Before"), CenteredStyle, PropertyWidth);
			EditorGUILayout.LabelField("▶", CenteredStyle, ArrowWidth);
			EditorGUILayout.LabelField(LanguageHelper.GetContextString("String_After"), CenteredStyle, PropertyWidth);
			EditorGUILayout.LabelField(string.Empty, ButtonWidth);
			EditorGUILayout.EndHorizontal();
			if (SerializedAvatarTextureList.arraySize > 0) {
				for (int Index = 0; Index < SerializedAvatarTextureList.arraySize; Index++) {
					SerializedProperty TextureProperty = SerializedAvatarTextureList.GetArrayElementAtIndex(Index);
					SerializedProperty ShowProperty = TextureProperty.FindPropertyRelative("ShowDetails");
					SerializedProperty BeforeProperty = TextureProperty.FindPropertyRelative("BeforeTexture");
					SerializedProperty AfterProperty = TextureProperty.FindPropertyRelative("AfterTexture");
					SerializedProperty MaterialProperty = TextureProperty.FindPropertyRelative("OriginMaterial");
					bool ShowDetailValue = ShowProperty.boolValue;
					EditorGUILayout.BeginHorizontal();
					GUI.enabled = false;
					EditorGUILayout.PropertyField(BeforeProperty, GUIContent.none, PropertyWidth);
					GUI.enabled = true;
					EditorGUILayout.LabelField("▶", CenteredStyle, ArrowWidth);
					EditorGUILayout.PropertyField(AfterProperty, GUIContent.none, PropertyWidth);
					if (GUILayout.Button(ShowDetailValue ? LanguageHelper.GetContextString("String_Hide") : LanguageHelper.GetContextString("String_Show"), ButtonWidth)) {
						ShowProperty.boolValue = !ShowDetailValue;
					}
					EditorGUILayout.EndHorizontal();
					if (ShowProperty.boolValue) {
						for (int MaterialIndex = 0; MaterialIndex < MaterialProperty.arraySize; MaterialIndex++) {
							EditorGUI.indentLevel++;
							EditorGUILayout.BeginHorizontal();
							SerializedProperty OriginMaterialProperty = MaterialProperty.GetArrayElementAtIndex(MaterialIndex);
							SerializedProperty OriginMaterial = OriginMaterialProperty.FindPropertyRelative("OriginMaterial");
							SerializedProperty OriginProperty = OriginMaterialProperty.FindPropertyRelative("PropertyName");
							string[] StringPropertys = new string[OriginProperty.arraySize];
							for (int PropertyIndex = 0; PropertyIndex < OriginProperty.arraySize; PropertyIndex++) {
								SerializedProperty StringProperty = OriginProperty.GetArrayElementAtIndex(PropertyIndex);
								StringPropertys[PropertyIndex] = StringProperty.stringValue;
							}
							GUI.enabled = false;
							EditorGUILayout.PropertyField(OriginMaterial, new GUIContent(string.Empty));
							GUI.enabled = true;
							EditorGUILayout.LabelField(String.Join(Environment.NewLine, StringPropertys));
							EditorGUILayout.EndHorizontal();
							EditorGUI.indentLevel--;
						}
					}
				}
			} else {
				EditorGUILayout.HelpBox(LanguageHelper.GetContextString("NO_DATA"), MessageType.Info);
			}
			EditorGUILayout.HelpBox(LanguageHelper.GetContextString("String_Null"), MessageType.Info);
			if (GUILayout.Button(LanguageHelper.GetContextString("String_Refresh"))) {
				(target as TextureReplacer).RefreshAvatarProprety();
				Repaint();
			}
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			serializedObject.ApplyModifiedProperties();
			if (GUILayout.Button(LanguageHelper.GetContextString("String_Replace"))) {
				(target as TextureReplacer).ChangeAvatarTextures();
				(target as TextureReplacer).RefreshAvatarProprety();
				Repaint();
			}
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button(LanguageHelper.GetContextString("String_Undo"))) {
				Undo.PerformUndo();
				(target as TextureReplacer).RefreshAvatarProprety();
				Repaint();
			}
			if (GUILayout.Button(LanguageHelper.GetContextString("String_Save"))) {
				AssetDatabase.SaveAssets();
			}
			EditorGUILayout.EndHorizontal();
		}
    }
}

