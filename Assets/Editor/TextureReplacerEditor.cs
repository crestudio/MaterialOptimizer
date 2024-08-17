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

		void OnEnable() {
			SerializedAvatarTextureList = serializedObject.FindProperty("AvatarTextureList");

			SerializedAvatarGameObject = serializedObject.FindProperty("AvatarGameObject");
			SerializedAvatarMaterials = serializedObject.FindProperty("AvatarMaterials");
		}

        public override void OnInspectorGUI() {
			TextureReplacer newTextureReplacer = (TextureReplacer)target;
			serializedObject.Update();
			EditorGUILayout.PropertyField(SerializedAvatarGameObject, new GUIContent("대상 오브젝트"));
			EditorGUILayout.PropertyField(SerializedAvatarMaterials, new GUIContent("아바타 머테리얼"));
			EditorGUILayout.LabelField("변경할 텍스쳐", EditorStyles.boldLabel);
			for (int Index = 0; Index < SerializedAvatarTextureList.arraySize; Index++) {
				SerializedProperty TextureProperty = SerializedAvatarTextureList.GetArrayElementAtIndex(Index);
				SerializedProperty BeforeProperty = TextureProperty.FindPropertyRelative("BeforeTexture");
				SerializedProperty AfterProperty = TextureProperty.FindPropertyRelative("AfterTexture");
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PropertyField(BeforeProperty, new GUIContent(string.Empty));
				EditorGUILayout.LabelField("▶", GUILayout.Width(15));
				EditorGUILayout.PropertyField(AfterProperty, new GUIContent(string.Empty));
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.EndHorizontal();
			if (GUILayout.Button("아바타 머테리얼 다시 가져오기")) {
				(target as TextureReplacer).RefreshAvatarProprety();
				Repaint();
			}
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			serializedObject.ApplyModifiedProperties();
			if (GUILayout.Button("텍스쳐 교체하기")) {
				(target as TextureReplacer).ChangeAvatarTextures();
				Repaint();
			}
		}
    }
}

