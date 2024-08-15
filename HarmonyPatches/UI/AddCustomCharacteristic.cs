using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using IPA.Utilities;
using UnityEngine;

namespace ReBeat.HarmonyPatches.UI {
    [HarmonyPatch(typeof(StandardLevelDetailView))]
    public class AddCustomCharacteristic {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(StandardLevelDetailView.SetContent))]
        static void AddCharacteristic(IBeatmapLevel level) {
            // copy all diffs to our characteristic
            // hide all other characteristics 
            // obv this won't work for maps that have other characteristics (lawless etc.) but it should be fine for now
            
            if (!Config.Instance.Enabled) return;
            if (!level.beatmapLevelData.difficultyBeatmapSets.Any()) return;
            if (level.beatmapLevelData.difficultyBeatmapSets.Any(x => x.beatmapCharacteristic.serializedName == "ReBeat")) return;
            if (level.beatmapLevelData.difficultyBeatmapSets.All(x => x.beatmapCharacteristic.serializedName != "Standard")) return; // this'll be removed later

            BeatmapCharacteristicSO characteristic = BeatmapCharacteristicSO.CreateInstance<BeatmapCharacteristicSO>();
            Sprite icon = SongCore.Utilities.Utils.LoadSpriteFromResources("ReBeat.Assets.libertybruh.png");
            characteristic.SetField("_icon", icon);
            characteristic.SetField("_descriptionLocalizationKey", "ReBeat");
            characteristic.SetField("_characteristicNameLocalizationKey", "ReBeat");
            characteristic.SetField("_serializedName", "ReBeat");
            characteristic.SetField("_compoundIdPartName", "ReBeat");
            characteristic.SetField("_sortingOrder", 0);
            characteristic.SetField("_containsRotationEvents", false);
            characteristic.SetField("_requires360Movement", false);

            CustomDifficultyBeatmapSet set = new CustomDifficultyBeatmapSet(characteristic);
            IDifficultyBeatmapSet standardSet = null;
            
            foreach (var difficultyBeatmapSet in level.beatmapLevelData.difficultyBeatmapSets) {
                if (difficultyBeatmapSet.beatmapCharacteristic.serializedName != "Standard") continue;
                standardSet = difficultyBeatmapSet;
                break;
            }
            if (standardSet is null) return;

            CustomDifficultyBeatmap[] beatmaps = new CustomDifficultyBeatmap[standardSet.difficultyBeatmaps.Count];
            for (int i = 0; i < beatmaps.Length; i++) {
                beatmaps[i] = (CustomDifficultyBeatmap)standardSet.difficultyBeatmaps[i];
            }
            
            set.SetCustomDifficultyBeatmaps(beatmaps);

            var characteristics = new List<IDifficultyBeatmapSet>(level.beatmapLevelData.difficultyBeatmapSets);
            characteristics.Add(set);
            
            FieldInfo fieldInfo = level.beatmapLevelData.GetType().GetField("_difficultyBeatmapSets",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (fieldInfo is null) return;
            fieldInfo.SetValue(level.beatmapLevelData, characteristics.ToArray());
        }
    }
}