using System.Collections.Generic;

/*
 * VRSuya VRSuya MaterialOptimizer TextureReplacer Editor
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace com.vrsuya.materialoptimizer {

	public class LanguageHelper : TextureReplacerEditor {

		/// <summary>요청한 값을 설정된 언어에 맞춰 값을 반환합니다.</summary>
		/// <returns>요청한 String의 현재 설정된 언어 버전</returns>
		internal static string GetContextString(string RequestContext) {
			string ReturnContext = RequestContext;
			switch (LanguageIndex) {
				case 0:
					if (String_English.ContainsKey(RequestContext)) {
						ReturnContext = String_English[RequestContext];
					}
					break;
				case 1:
					if (String_Korean.ContainsKey(RequestContext)) {
						ReturnContext = String_Korean[RequestContext];
					}
					break;
				case 2:
					if (String_Japanese.ContainsKey(RequestContext)) {
						ReturnContext = String_Japanese[RequestContext];
					}
					break;
			}
			return ReturnContext;
		}

		// 영어 사전 데이터
		private static Dictionary<string, string> String_English = new Dictionary<string, string>() {
			{ "String_Language", "Language" },
			{ "String_TargetAvatar", "Target Avatar" },
			{ "String_TargetMaterial", "Materials in use" },
			{ "String_TargetTexture", "Textures in use" },
			{ "String_Before", "Existing Textures" },
			{ "String_After", "Textures to Change" },
			{ "String_Show", "Show" },
			{ "String_Hide", "Hide" },
			{ "String_Refresh", "Refresh" },
			{ "String_Replace", "Replace Texture" },
			{ "String_Undo", "Undo" },
			{ "String_Save", "Save" },
			{ "String_Null", "Clearing the item will remove the texture from the material" },

			{ "NO_DATA", "The texture cannot be found in the specified object" }
		};

		// 한국어 사전 데이터
		private static Dictionary<string, string> String_Korean = new Dictionary<string, string>() {
			{ "String_Language", "언어" },
			{ "String_TargetAvatar", "대상 아바타" },
			{ "String_TargetMaterial", "사용 중인 머테리얼" },
			{ "String_TargetTexture", "사용 중인 텍스쳐" },
			{ "String_Before", "기존 텍스쳐" },
			{ "String_After", "변경할 텍스쳐" },
			{ "String_Show", "표시" },
			{ "String_Hide", "숨기기" },
			{ "String_Refresh", "새로 고침" },
			{ "String_Replace", "텍스쳐 교체" },
			{ "String_Undo", "실행 취소" },
			{ "String_Save", "저장" },
			{ "String_Null", "항목을 비우면 해당 텍스쳐를 머테리얼에서 제거합니다" },

			{ "NO_DATA", "해당 오브젝트에서 텍스쳐를 찾을 수 없습니다" }
		};

		// 일본어 사전 데이터
		private static Dictionary<string, string> String_Japanese = new Dictionary<string, string>() {
			{ "String_Language", "言語" },
			{ "String_TargetAvatar", "対象アバター" },
			{ "String_TargetMaterial", "使用中のマテリアル" },
			{ "String_TargetTexture", "使用中のテクスチャ" },
			{ "String_Before", "既存のテクスチャ" },
			{ "String_After", "変更するテクスチャ" },
			{ "String_Show", "表示" },
			{ "String_Hide", "非表示" },
			{ "String_Refresh", "更新" },
			{ "String_Replace", "テクスチャ交換" },
			{ "String_Undo", "元に戻す" },
			{ "String_Save", "保存" },
			{ "String_Null", "項目をクリアすると、該当テクスチャがマテリアルから削除されます" },

			{ "NO_DATA", "該当オブジェクトでテクスチャを見つけることができません" }
		};
	}
}
