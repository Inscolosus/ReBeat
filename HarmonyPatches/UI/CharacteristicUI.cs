using HarmonyLib;
using UnityEngine;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Tags;
using HMUI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

namespace ReBeat.HarmonyPatches.UI {
	[HarmonyPatch(typeof(BeatmapCharacteristicSegmentedControlController))]
	class CharacteristicUI {
		[HarmonyPatch(nameof(BeatmapCharacteristicSegmentedControlController.Awake))]
		static void Postfix(BeatmapCharacteristicSegmentedControlController __instance) {
			var transform = __instance.transform.parent as RectTransform;
			transform.anchorMax = new Vector2(0.9f, 1f);

			var parent = transform.parent;

			var button = new ClickableImageTag().CreateObject(transform);
			button.AddComponent<ToggleButton>();
			button.GetComponentInChildren<ImageView>().sprite = SongCore.Utilities.Utils.LoadSpriteFromResources("ReBeat.Assets.icon.png");
			button.transform.SetParent(parent.transform, false);

			var buttonTransform = button.transform as RectTransform;
			buttonTransform.anchorMin = new Vector2(0.92f, 0.47f);
			buttonTransform.anchorMax = new Vector2(0.92f, 0.47f);
			buttonTransform.sizeDelta = new Vector2(5f, 5f);
		}

		[HarmonyPatch(nameof(BeatmapCharacteristicSegmentedControlController.SetData))]
		static void Prefix(ref IReadOnlyList<IDifficultyBeatmapSet> difficultyBeatmapSets) {
			Plugin.Log.Info("Setting data");
			difficultyBeatmapSets = difficultyBeatmapSets.Where(x => (Config.Instance.Enabled && x.beatmapCharacteristic.serializedName.StartsWith("ReBeat_")) || (!Config.Instance.Enabled && !x.beatmapCharacteristic.serializedName.StartsWith("ReBeat_"))).ToList();
		}

		[HarmonyPostfix]
		[HarmonyPatch(nameof(BeatmapCharacteristicSegmentedControlController.SetData))]
		static void FixWidth(BeatmapCharacteristicSegmentedControlController __instance) {
			Plugin.Log.Info("Setting data");
			foreach (var image in __instance.GetComponentsInChildren<ImageView>()) {
				image.rectTransform.sizeDelta = new Vector2(10f, 4f);
			}
		}
	}

	class ToggleButton : MonoBehaviour {
		private ClickableImage _clickableImage;
		
		void Awake() {
			_clickableImage = GetComponent<ClickableImage>();
			_clickableImage.OnClickEvent += Click;
		}

		void Click(PointerEventData eventData) {
			Config.Instance.Enabled = !Config.Instance.Enabled;
			ResetModifiers.GsvcInstance.RefreshContent();
			var view = transform.parent.parent.GetComponent<StandardLevelDetailViewController>();
			view._standardLevelDetailView.SetContent(view._beatmapLevel, view._playerDataModel.playerData.lastSelectedBeatmapDifficulty, view._playerDataModel.playerData.lastSelectedBeatmapCharacteristic, view._playerDataModel.playerData);
		}

		void Update() {
			_clickableImage.DefaultColor = Config.Instance.Enabled ? _clickableImage.HighlightColor : Color.white;
		}
	}
}