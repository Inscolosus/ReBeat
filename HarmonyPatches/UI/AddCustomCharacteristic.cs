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
        static void AddCharacteristic(IBeatmapLevel level, ref BeatmapCharacteristicSO defaultBeatmapCharacteristic) {
	        if (!(level is CustomBeatmapLevel)) {
		        CharacteristicUI.IsCustomLevel = false;
		        Config.Instance.Enabled = false;
		        ResetModifiers.GsvcInstance.RefreshContent();
		        return;
	        }
	        CharacteristicUI.IsCustomLevel = true;
            if (!level.beatmapLevelData.difficultyBeatmapSets.Any()) return;
            if (level.beatmapLevelData.difficultyBeatmapSets.Any(x => x.beatmapCharacteristic.serializedName.Contains("ReBeat"))) return;

			var characteristics = new List<IDifficultyBeatmapSet>(level.beatmapLevelData.difficultyBeatmapSets);

			foreach (var difficultyBeatmapSet in level.beatmapLevelData.difficultyBeatmapSets) {
	            var chara = difficultyBeatmapSet.beatmapCharacteristic;
	            var newCharaID = $"ReBeat_{chara.serializedName}";
	            var newChara =
		            SongCore.Collections.customCharacteristics.FirstOrDefault(x => x.serializedName == newCharaID);

				if (newChara == null) continue;

                var newSet = new CustomDifficultyBeatmapSet(newChara);

                var beatmaps = new List<CustomDifficultyBeatmap>();
                foreach (var map in difficultyBeatmapSet.difficultyBeatmaps) {
	                CustomDifficultyBeatmap m = (CustomDifficultyBeatmap)map;
	                
	                beatmaps.Add(new CustomDifficultyBeatmap(map.level, newSet, map.difficulty, map.difficultyRank,
		                map.noteJumpMovementSpeed, map.noteJumpStartBeatOffset, map.level.beatsPerMinute, m.beatmapSaveData,
		                m.beatmapDataBasicInfo));
                }
                
                newSet.SetCustomDifficultyBeatmaps(beatmaps.ToArray());

				characteristics.Add(newSet);
			}
			
			
            level.beatmapLevelData.GetType().GetField("_difficultyBeatmapSets",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                ?.SetValue(level.beatmapLevelData, characteristics.ToArray());

            if (!Config.Instance.Enabled) return;
	        defaultBeatmapCharacteristic = characteristics[0].beatmapCharacteristic;
        }
    }
}