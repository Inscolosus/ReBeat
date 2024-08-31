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
            // copy all diffs to our characteristic
            // hide all other characteristics 
            // obv this won't work for maps that have other characteristics (lawless etc.) but it should be fine for now
            
            if (!(level is CustomBeatmapLevel)) return;
            if (!level.beatmapLevelData.difficultyBeatmapSets.Any()) return;
            if (level.beatmapLevelData.difficultyBeatmapSets.Any(x => x.beatmapCharacteristic.serializedName.Contains("ReBeat"))) return;

			var characteristics = new List<IDifficultyBeatmapSet>(level.beatmapLevelData.difficultyBeatmapSets);

			foreach (var difficultyBeatmapSet in level.beatmapLevelData.difficultyBeatmapSets)
            {
	            var chara = difficultyBeatmapSet.beatmapCharacteristic;
	            var newCharaID = $"ReBeat_{chara.serializedName}";
	            var newChara =
		            SongCore.Collections.customCharacteristics.FirstOrDefault(x => x.serializedName == newCharaID);

				if (newChara == null) continue;

                var newSet = new CustomDifficultyBeatmapSet(newChara);

                var beatmaps = new List<CustomDifficultyBeatmap>();
                foreach (var map in difficultyBeatmapSet.difficultyBeatmaps)
                {
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

            if (Config.Instance.Enabled) defaultBeatmapCharacteristic = characteristics[0].beatmapCharacteristic;
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