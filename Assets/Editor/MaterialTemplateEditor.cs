using UnityEngine;
using UnityEditor;

/*
 * VRSuya Material Template Editor
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace com.vrsuya.materialtemplate {

	[CustomEditor(typeof(MaterialTemplate))]
	public class MaterialTemplateEditor : Editor {
		SerializedProperty SerializedTargetGameObject;
		SerializedProperty SerializedTargetMaterials;
		SerializedProperty SerializedTargetTexture2Ds;

		SerializedProperty SerializedUpdatelilToon;
		SerializedProperty SerializedUpdatepoiyomi;
		SerializedProperty SerializedUpdateUnityChanToonShader;

		SerializedProperty SerializedUpdatelilToonLighting;
		SerializedProperty SerializedUpdatelilToonReceiveShadow;

		SerializedProperty SerializedUpdateUTSTextureShared;
		SerializedProperty SerializedUpdateUTSNormalMap;
		SerializedProperty SerializedUpdateUTSBasicShading;
		SerializedProperty SerializedUpdateUTSLightColor;
		SerializedProperty SerializedUpdateUTSEnvironmentalLightingPropertys;

		SerializedProperty SerializedUpdateGPUInstancing;
		SerializedProperty SerializedUpdateGlobalIllumination;

		SerializedProperty SerializedAnalyzeTextures;
		SerializedProperty SerializedUpdatesRGB;
		SerializedProperty SerializedUpdateNormal;
		SerializedProperty SerializedUpdateAlpha;
		SerializedProperty SerializedUpdateMaxTextureSize;
		SerializedProperty SerializedUpdateOverrideStandalone;

		SerializedProperty SerializedReturnString;

		public static bool FoldlilToon = true;
		public static bool Foldpoiyomi = true;
		public static bool FoldUnityChanToonShader = true;
		public static bool FoldTexture = true;

		void OnEnable() {
			SerializedTargetGameObject = serializedObject.FindProperty("TargetGameObject");
			SerializedTargetMaterials = serializedObject.FindProperty("TargetMaterials");
			SerializedTargetTexture2Ds = serializedObject.FindProperty("TargetTexture2Ds");

			SerializedUpdatelilToon = serializedObject.FindProperty("UpdatelilToon");
			SerializedUpdatepoiyomi = serializedObject.FindProperty("Updatepoiyomi");
			SerializedUpdateUnityChanToonShader = serializedObject.FindProperty("UpdateUnityChanToonShader");

			SerializedUpdatelilToonLighting = serializedObject.FindProperty("UpdatelilToonLighting");
			SerializedUpdatelilToonReceiveShadow = serializedObject.FindProperty("UpdatelilToonReceiveShadow");

			SerializedUpdateUTSTextureShared = serializedObject.FindProperty("UpdateUTSTextureShared");
			SerializedUpdateUTSNormalMap = serializedObject.FindProperty("UpdateUTSNormalMap");
			SerializedUpdateUTSBasicShading = serializedObject.FindProperty("UpdateUTSBasicShading");
			SerializedUpdateUTSLightColor = serializedObject.FindProperty("UpdateUTSLightColor");
			SerializedUpdateUTSEnvironmentalLightingPropertys = serializedObject.FindProperty("UpdateUTSEnvironmentalLightingPropertys");

			SerializedUpdateGPUInstancing = serializedObject.FindProperty("UpdateGPUInstancing");
			SerializedUpdateGlobalIllumination = serializedObject.FindProperty("UpdateGlobalIllumination");

			SerializedAnalyzeTextures = serializedObject.FindProperty("AnalyzeTextures");
			SerializedUpdatesRGB = serializedObject.FindProperty("UpdatesRGB");
			SerializedUpdateNormal = serializedObject.FindProperty("UpdateNormal");
			SerializedUpdateAlpha = serializedObject.FindProperty("UpdateAlpha");
			SerializedUpdateMaxTextureSize = serializedObject.FindProperty("UpdateMaxTextureSize");
			SerializedUpdateOverrideStandalone = serializedObject.FindProperty("UpdateOverrideStandalone");

			SerializedReturnString = serializedObject.FindProperty("ReturnString");
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();
			EditorGUILayout.PropertyField(SerializedTargetGameObject, new GUIContent("아바타"));
			EditorGUILayout.PropertyField(SerializedTargetMaterials, new GUIContent("머테리얼"));
			EditorGUILayout.PropertyField(SerializedTargetTexture2Ds, new GUIContent("텍스쳐"));
			if (GUILayout.Button("아바타 텍스쳐 추가")) {
				(target as MaterialTemplate).AddAvatarTextures();
			}
			if (GUILayout.Button("모든 텍스쳐 추가")) {
				(target as MaterialTemplate).AddTexture2Ds();
			}
			if (GUILayout.Button("아바타 DXT1 아닌 텍스쳐 추가")) {
				(target as MaterialTemplate).AddAvatarNotDXT1Textures();
			}
			if (GUILayout.Button("모든 DXT1 아닌 텍스쳐 추가")) {
				(target as MaterialTemplate).AddNotDXT1Textures();
			}
			EditorGUILayout.LabelField("변환을 적용할 쉐이더", EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(SerializedUpdatelilToon, new GUIContent("lilToon"));
			if (GUILayout.Button("lilToon 머테리얼 추가")) {
				(target as MaterialTemplate).AddlilToonMaterials();
			}
			EditorGUILayout.PropertyField(SerializedUpdatepoiyomi, new GUIContent("poiyomi"));
			if (GUILayout.Button("poiyomi 머테리얼 추가")) {
				(target as MaterialTemplate).AddpoiyomiMaterials();
			}
			EditorGUILayout.PropertyField(SerializedUpdateUnityChanToonShader, new GUIContent("UnityChanToonShader"));
			if (GUILayout.Button("UTS 머테리얼 추가")) {
				(target as MaterialTemplate).AddUnityChanToonShaderMaterials();
			}
			EditorGUI.indentLevel--;
			EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
			FoldlilToon = EditorGUILayout.Foldout(FoldlilToon, "lilToon 프로퍼티");
			if (FoldlilToon) {
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(SerializedUpdatelilToonLighting, new GUIContent("라이팅 설정"));
				EditorGUILayout.PropertyField(SerializedUpdatelilToonReceiveShadow, new GUIContent("그림자 영향 설정"));
				EditorGUILayout.PropertyField(SerializedUpdateGPUInstancing, new GUIContent("GPU 인스턴싱 설정"));
				EditorGUILayout.PropertyField(SerializedUpdateGlobalIllumination, new GUIContent("Global Illumination 설정"));
				EditorGUI.indentLevel--;
			}
			Foldpoiyomi = EditorGUILayout.Foldout(Foldpoiyomi, "poiyomi 프로퍼티");
			if (Foldpoiyomi) {
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(SerializedUpdateGPUInstancing, new GUIContent("GPU 인스턴싱 설정"));
				EditorGUILayout.PropertyField(SerializedUpdateGlobalIllumination, new GUIContent("Global Illumination 설정"));
				EditorGUI.indentLevel--;
			}
			FoldUnityChanToonShader = EditorGUILayout.Foldout(FoldUnityChanToonShader, "UnityChanToonShader 프로퍼티");
			if (FoldUnityChanToonShader) {
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(SerializedUpdateUTSTextureShared, new GUIContent("텍스쳐 공유 설정"));
				EditorGUILayout.PropertyField(SerializedUpdateUTSNormalMap, new GUIContent("노멀맵 적용 설정"));
				EditorGUILayout.PropertyField(SerializedUpdateUTSBasicShading, new GUIContent("기본 쉐이딩 설정"));
				EditorGUILayout.PropertyField(SerializedUpdateUTSLightColor, new GUIContent("주광색 영향 설정"));
				EditorGUILayout.PropertyField(SerializedUpdateUTSEnvironmentalLightingPropertys, new GUIContent("환경광 설정"));
				EditorGUILayout.PropertyField(SerializedUpdateGPUInstancing, new GUIContent("GPU 인스턴싱 설정"));
				EditorGUILayout.PropertyField(SerializedUpdateGlobalIllumination, new GUIContent("Global Illumination 설정"));
				EditorGUI.indentLevel--;
			}
			FoldTexture = EditorGUILayout.Foldout(FoldTexture, "텍스쳐 프로퍼티");
			if (FoldTexture) {
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(SerializedAnalyzeTextures, new GUIContent("텍스쳐 분석"));
				GUI.enabled = SerializedAnalyzeTextures.boolValue;
				EditorGUILayout.PropertyField(SerializedUpdatesRGB, new GUIContent("sRGB 텍스쳐 분석"));
				EditorGUILayout.PropertyField(SerializedUpdateNormal, new GUIContent("노멀 텍스쳐 분석"));
				EditorGUILayout.PropertyField(SerializedUpdateAlpha, new GUIContent("알파 텍스쳐 분석"));
				GUI.enabled = true;
				EditorGUILayout.PropertyField(SerializedUpdateMaxTextureSize, new GUIContent("최대 해상도 설정"));
				EditorGUILayout.PropertyField(SerializedUpdateOverrideStandalone, new GUIContent("오버라이드 설정"));
				EditorGUI.indentLevel--;
			}
			if (!string.IsNullOrEmpty(SerializedReturnString.stringValue)) {
				EditorGUILayout.HelpBox(SerializedReturnString.stringValue, MessageType.Info);
			}
			serializedObject.ApplyModifiedProperties();
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			if (GUILayout.Button("업데이트")) {
				(target as MaterialTemplate).UpdateMaterialPropertys();
				Repaint();
			}
		}
	}
}
