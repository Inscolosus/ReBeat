using System;
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
        [HarmonyPatch(nameof(StandardLevelDetailView.SetContent), typeof(BeatmapLevel), typeof(BeatmapDifficultyMask), typeof(HashSet<BeatmapCharacteristicSO>), typeof(BeatmapDifficulty), typeof(BeatmapCharacteristicSO), typeof(PlayerData))]
        static void AddCharacteristic(ref BeatmapLevel level, ref BeatmapCharacteristicSO defaultBeatmapCharacteristic) {
            BeatmapCharacteristicSO rebeatStandardCharacteristic = BeatmapCharacteristicSO.CreateInstance<BeatmapCharacteristicSO>();
            Sprite icon = SongCore.Utilities.Utils.LoadSpriteFromResources("ReBeat.Assets.icon.png");
            rebeatStandardCharacteristic.SetField("_icon", icon);
            rebeatStandardCharacteristic.SetField("_descriptionLocalizationKey", "ReBeat_Standard");
            rebeatStandardCharacteristic.SetField("_characteristicNameLocalizationKey", "ReBeat_Standard");
            rebeatStandardCharacteristic.SetField("_serializedName", "ReBeat_Standard");
            rebeatStandardCharacteristic.SetField("_compoundIdPartName", "ReBeat_Standard");
            rebeatStandardCharacteristic.SetField("_sortingOrder", 0);
            rebeatStandardCharacteristic.SetField("_containsRotationEvents", false);
            rebeatStandardCharacteristic.SetField("_requires360Movement", false);

            var beatmapBasicData = level.beatmapBasicData;
            var rebeatCharData = new Dictionary<(BeatmapCharacteristicSO, BeatmapDifficulty), BeatmapBasicData>();
            foreach (var entry in beatmapBasicData) {
                rebeatCharData.Add(entry.Key, entry.Value);
                if (entry.Key.Item1.serializedName != "Standard") continue;
                rebeatCharData.Add((rebeatStandardCharacteristic, entry.Key.Item2), entry.Value);
            }

            typeof(BeatmapLevel).GetField("beatmapBasicData", BindingFlags.Instance | BindingFlags.Public)
                .SetValue(level, rebeatCharData);

            if (Config.Instance.Enabled) defaultBeatmapCharacteristic = rebeatStandardCharacteristic;
        }
        
        
        [HarmonyPostfix]
        [HarmonyPatch(nameof(StandardLevelDetailView.RefreshContent))]
        static void CharacteristicSelected(BeatmapCharacteristicSegmentedControlController ____beatmapCharacteristicSegmentedControlController) {
            bool rebeat = ____beatmapCharacteristicSegmentedControlController.selectedBeatmapCharacteristic.serializedName
                .Contains("ReBeat");
            if (Config.Instance.Enabled == rebeat) return;
            Config.Instance.Enabled = rebeat;

            Plugin.Log.Info("sjdhdkjshsdk");
            // ReSharper disable once PossibleNullReferenceException
            typeof(GameplaySetupViewController).InvokeMember("RefreshContent",
                BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null,
                ResetModifiers.GsvcInstance, Array.Empty<object>());
            //.GetMethod("RefreshContent", BindingFlags.Instance | BindingFlags.NonPublic)
            //.Invoke(ResetModifiers.GsvcInstance, new object[] { });
        }
    }

    [HarmonyPatch(typeof(FileSystemBeatmapLevelData))]
    class PatchBeatmapFile {
        [HarmonyPrefix]
        [HarmonyPatch("GetDifficultyBeatmap")]
        static void ResetCharacteristic(ref BeatmapKey beatmapKey) {
            if (beatmapKey.beatmapCharacteristic.serializedName != "ReBeat_Standard") return;
            beatmapKey = new BeatmapKey(beatmapKey.levelId, standard, beatmapKey.difficulty);
        }
    }
}