using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using IPA.Utilities;
using UnityEngine;

namespace ReBeat.HarmonyPatches.UI {
    [HarmonyPatch(typeof(StandardLevelDetailView))]
    class AddCustomCharacteristic {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(StandardLevelDetailView.SetContent))]
        static void AddCharacteristic(IBeatmapLevel level) {
            // copy all diffs to our characteristic
            // hide all other characteristics 
            // obv this won't work for maps that have other characteristics (lawless etc.) but it should be fine for now
            
            if (!(level is CustomBeatmapLevel)) return;
            if (!level.beatmapLevelData.difficultyBeatmapSets.Any()) return;
            if (level.beatmapLevelData.difficultyBeatmapSets.Any(x => x.beatmapCharacteristic.serializedName.Contains("ReBeat"))) return;
            if (level.beatmapLevelData.difficultyBeatmapSets.All(x => x.beatmapCharacteristic.serializedName != "Standard")) return; // only standard for now

            BeatmapCharacteristicSO rebeatStandardCharacteristic = BeatmapCharacteristicSO.CreateInstance<BeatmapCharacteristicSO>();
            Sprite icon = SongCore.Utilities.Utils.LoadSpriteFromResources("ReBeat.Assets.libertybruh.png");
            rebeatStandardCharacteristic.SetField("_icon", icon);
            rebeatStandardCharacteristic.SetField("_descriptionLocalizationKey", "ReBeat_Standard");
            rebeatStandardCharacteristic.SetField("_characteristicNameLocalizationKey", "ReBeat_Standard");
            rebeatStandardCharacteristic.SetField("_serializedName", "ReBeat_Standard");
            rebeatStandardCharacteristic.SetField("_compoundIdPartName", "ReBeat_Standard");
            rebeatStandardCharacteristic.SetField("_sortingOrder", 0);
            rebeatStandardCharacteristic.SetField("_containsRotationEvents", false);
            rebeatStandardCharacteristic.SetField("_requires360Movement", false);

            CustomDifficultyBeatmapSet rebeatStandardSet = new CustomDifficultyBeatmapSet(rebeatStandardCharacteristic);
            IDifficultyBeatmapSet standardSet = null;
            
            foreach (var difficultyBeatmapSet in level.beatmapLevelData.difficultyBeatmapSets) {
                if (difficultyBeatmapSet.beatmapCharacteristic.serializedName != "Standard") continue;
                standardSet = difficultyBeatmapSet;
                break;
            }
            if (standardSet is null) return;

            var beatmaps = new List<CustomDifficultyBeatmap>();
            foreach (var map in standardSet.difficultyBeatmaps) {
                CustomDifficultyBeatmap m = (CustomDifficultyBeatmap)map;
                beatmaps.Add(new CustomDifficultyBeatmap(map.level, rebeatStandardSet, map.difficulty, map.difficultyRank,
                    map.noteJumpMovementSpeed, map.noteJumpStartBeatOffset, map.level.beatsPerMinute, m.beatmapSaveData,
                    m.beatmapDataBasicInfo));
            }

            rebeatStandardSet.SetCustomDifficultyBeatmaps(beatmaps.ToArray());

            var characteristics = new List<IDifficultyBeatmapSet>(level.beatmapLevelData.difficultyBeatmapSets);
            characteristics.Add(rebeatStandardSet);

            level.beatmapLevelData.GetType().GetField("_difficultyBeatmapSets",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                ?.SetValue(level.beatmapLevelData, characteristics.ToArray());
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(StandardLevelDetailView.RefreshContent))]
        static void CharacteristicSelected(IDifficultyBeatmap ____selectedDifficultyBeatmap) {
            bool rebeat = ____selectedDifficultyBeatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName
                .Contains("ReBeat");
            if (Config.Instance.Enabled == rebeat) return;
            Config.Instance.Enabled = rebeat;
            ResetModifiers.GsvcInstance.RefreshContent();
        }
    }
}