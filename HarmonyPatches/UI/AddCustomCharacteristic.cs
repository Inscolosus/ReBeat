using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using IPA.Utilities;
using ReBeat.HarmonyPatches.BeamapData;
using UnityEngine;

namespace ReBeat.HarmonyPatches.UI {
    [HarmonyPatch(typeof(StandardLevelDetailView))]
    class AddCustomCharacteristic {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(StandardLevelDetailView.SetContent), typeof(BeatmapLevel), typeof(BeatmapDifficultyMask), typeof(HashSet<BeatmapCharacteristicSO>), typeof(BeatmapDifficulty), typeof(BeatmapCharacteristicSO), typeof(PlayerData))]
        static void AddCharacteristic(BeatmapLevel level, ref BeatmapCharacteristicSO defaultBeatmapCharacteristic) {
	        if (!level.levelID.StartsWith("custom_level")) {
		        CharacteristicUI.IsCustomLevel = false;
		        Config.Instance.Enabled = false;
		        ResetModifiers.GsvcInstance.RefreshContent();
		        return;
	        }
	        BeamapData.BeatmapData.SongLength = level.songDuration;
	        
	        CharacteristicUI.IsCustomLevel = true;
            if (!level.GetBeatmapKeys().Any()) return;
            if (level.GetBeatmapKeys().Any(x => x.beatmapCharacteristic.serializedName.Contains("ReBeat"))) return;

            var newKeys = new Dictionary<(BeatmapCharacteristicSO, BeatmapDifficulty), BeatmapBasicData>();
            foreach (var key in level.GetBeatmapKeys()) {
	            newKeys.Add((key.beatmapCharacteristic, key.difficulty), level.GetDifficultyBeatmapData(key.beatmapCharacteristic, key.difficulty));
            }

            BeatmapCharacteristicSO firstChar = null;
            foreach (var chara in level.GetCharacteristics()) {
	            var newCharaId = $"ReBeat_{chara.serializedName}";
	            var newChara =
		            SongCore.Collections.customCharacteristics.FirstOrDefault(x => x.serializedName == newCharaId);
	            if (newChara is null) continue;
	            if (firstChar is null) firstChar = newChara;

	            foreach (var diff in level.GetDifficulties(chara)) {
		            newKeys.Add((newChara, diff), level.GetDifficultyBeatmapData(chara, diff));
	            }
            }

            level.GetType().GetField("beatmapBasicData", BindingFlags.Instance | BindingFlags.Public)
	            ?.SetValue(level, newKeys);
            level.GetType().GetField("_beatmapKeysCache", BindingFlags.Instance | BindingFlags.NonPublic)
	            ?.SetValue(level, null);
            level.GetType().GetField("_characteristicsCache", BindingFlags.Instance | BindingFlags.NonPublic)
	            ?.SetValue(level, null);

            
            if (!Config.Instance.Enabled || firstChar is null) return;
            defaultBeatmapCharacteristic = firstChar;
        }
    }
    
    [HarmonyPatch(typeof(FileSystemBeatmapLevelData))]
    class PatchBeatmapFile {
	    [HarmonyPrefix]
	    [HarmonyPatch("GetDifficultyBeatmap")]
	    static void ResetCharacteristic(ref BeatmapKey beatmapKey, FileSystemBeatmapLevelData __instance) {
		    if (!beatmapKey.beatmapCharacteristic.serializedName.StartsWith("ReBeat_")) return;

		    Type expectedType = typeof(FileSystemBeatmapLevelData);
		    Type type = __instance.GetType() == expectedType ? expectedType : __instance.GetType().BaseType;
		    if (type != expectedType) Plugin.Log.Error($"Unrecognized filesystem data type {__instance.GetType()} {__instance.GetType().Assembly.FullName}");
		    
		    var difficultyBeatmaps =
			    (Dictionary<(BeatmapCharacteristicSO, BeatmapDifficulty), FileDifficultyBeatmap>)type
				    .GetField("_difficultyBeatmaps", BindingFlags.Instance | BindingFlags.NonPublic)
				    .GetValue(__instance);
		    
		    string normalCharName = beatmapKey.beatmapCharacteristic.serializedName.Substring(7);
		    var diff = beatmapKey.difficulty;
		    var entryForNormalCharacteristic = difficultyBeatmaps.FirstOrDefault(x => x.Key.Item1.serializedName == normalCharName && x.Key.Item2 == diff);

		    beatmapKey = new BeatmapKey(beatmapKey.levelId, entryForNormalCharacteristic.Key.Item1, beatmapKey.difficulty);
	    }
    }
}