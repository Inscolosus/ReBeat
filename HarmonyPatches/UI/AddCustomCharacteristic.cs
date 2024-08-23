using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using IPA.Utilities;
using TMPro;
using UnityEngine;
using Zenject;

namespace ReBeat.HarmonyPatches.UI {
    [HarmonyPatch(typeof(StandardLevelDetailView))]
    class AddCustomCharacteristic {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(StandardLevelDetailView.SetContent))]
        static void AddCharacteristic(IBeatmapLevel level) {
            // copy all diffs to our characteristic
            // hide all other characteristics 
            // obv this won't work for maps that have other characteristics (lawless etc.) but it should be fine for now
            Plugin.Log.Info(level.GetType().ToString());
            
            if (!Config.Instance.Enabled) return;
            if (!(level is CustomBeatmapLevel)) return;
            if (!level.beatmapLevelData.difficultyBeatmapSets.Any()) return;
            if (level.beatmapLevelData.difficultyBeatmapSets.Any(x => x.beatmapCharacteristic.serializedName.Contains("ReBeat"))) return;
            if (level.beatmapLevelData.difficultyBeatmapSets.All(x => x.beatmapCharacteristic.serializedName != "Standard")) return; // this'll be removed later

            BeatmapCharacteristicSO characteristic = BeatmapCharacteristicSO.CreateInstance<BeatmapCharacteristicSO>();
            Sprite icon = SongCore.Utilities.Utils.LoadSpriteFromResources("ReBeat.Assets.libertybruh.png");
            characteristic.SetField("_icon", icon);
            characteristic.SetField("_descriptionLocalizationKey", "ReBeat_Standard");
            characteristic.SetField("_characteristicNameLocalizationKey", "ReBeat_Standard");
            characteristic.SetField("_serializedName", "ReBeat_Standard");
            characteristic.SetField("_compoundIdPartName", "ReBeat_Standard");
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

            var beatmaps = new List<CustomDifficultyBeatmap>();
            foreach (var map in standardSet.difficultyBeatmaps) {
                CustomDifficultyBeatmap m = (CustomDifficultyBeatmap)map;
                beatmaps.Add(new CustomDifficultyBeatmap(map.level, set, map.difficulty, map.difficultyRank, map.noteJumpMovementSpeed, map.noteJumpStartBeatOffset, map.level.beatsPerMinute, m.beatmapSaveData, m.beatmapDataBasicInfo));
            }

            set.SetCustomDifficultyBeatmaps(beatmaps.ToArray());

            var characteristics = new List<IDifficultyBeatmapSet>(level.beatmapLevelData.difficultyBeatmapSets);
            characteristics.Add(set);

            level.beatmapLevelData.GetType().GetField("_difficultyBeatmapSets",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                ?.SetValue(level.beatmapLevelData, characteristics.ToArray());
        }

        internal static bool IsRebeat;
        [HarmonyPostfix]
        [HarmonyPatch(nameof(StandardLevelDetailView.RefreshContent))]
        static void CharacteristicSelected(IDifficultyBeatmap ____selectedDifficultyBeatmap) {
            bool rebeat = ____selectedDifficultyBeatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName
                .Contains("ReBeat");
            if (IsRebeat == rebeat) return;
            IsRebeat = rebeat;
            Test3.GsvcInstance.RefreshContent();
        }
        
    }
    
    // TODO: move to file
    [HarmonyPatch(typeof(MenuTransitionsHelper))]
    class LevelStart {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(MenuTransitionsHelper.StartStandardLevel), typeof(string), typeof(IDifficultyBeatmap),
            typeof(IPreviewBeatmapLevel), typeof(OverrideEnvironmentSettings), typeof(ColorScheme),
            typeof(GameplayModifiers), typeof(PlayerSpecificSettings), typeof(PracticeSettings), typeof(string),
            typeof(bool), typeof(bool), typeof(Action), typeof(Action<DiContainer>),
            typeof(Action<StandardLevelScenesTransitionSetupDataSO, LevelCompletionResults>),
            typeof(Action<LevelScenesTransitionSetupDataSO, LevelCompletionResults>))]
        static void CheckCharacteristic(IDifficultyBeatmap difficultyBeatmap) {
            BS_Utils.Gameplay.ScoreSubmission.ProlongedDisableSubmission("ReBeat");
            if (!(difficultyBeatmap is CustomDifficultyBeatmap diff)) return;

            if (diff.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName.Contains("ReBeat")) {
                if (Config.Instance.Enabled) BS_Utils.Gameplay.ScoreSubmission.RemoveProlongedDisable("ReBeat");
            }
            else if (!Config.Instance.Enabled) BS_Utils.Gameplay.ScoreSubmission.RemoveProlongedDisable("ReBeat");
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(MenuTransitionsHelper.StartStandardLevel), typeof(string), typeof(IDifficultyBeatmap),
            typeof(IPreviewBeatmapLevel), typeof(OverrideEnvironmentSettings), typeof(ColorScheme),
            typeof(GameplayModifiers), typeof(PlayerSpecificSettings), typeof(PracticeSettings), typeof(string),
            typeof(bool), typeof(bool), typeof(Action), typeof(Action<DiContainer>),
            typeof(Action<StandardLevelScenesTransitionSetupDataSO, LevelCompletionResults>),
            typeof(Action<LevelScenesTransitionSetupDataSO, LevelCompletionResults>))]
        static void SetMods(ref GameplayModifiers gameplayModifiers) {
            if (!Config.Instance.Enabled) return;
            var g = typeof(GameplayModifiers);
            const BindingFlags b = BindingFlags.Instance | BindingFlags.NonPublic;
            var m = Modifiers.instance;
            
            // ReSharper disable PossibleNullReferenceException
            g.GetField("_energyType", b).SetValue(gameplayModifiers, GameplayModifiers.EnergyType.Battery);
            g.GetField("_noFailOn0Energy", b).SetValue(gameplayModifiers, m.NoFail);
            g.GetField("_instaFail", b).SetValue(gameplayModifiers, m.OneLife);
            g.GetField("_failOnSaberClash", b).SetValue(gameplayModifiers, false);
            g.GetField("_enabledObstacleType", b).SetValue(gameplayModifiers, m.NoWalls
                    ? GameplayModifiers.EnabledObstacleType.NoObstacles
                    : GameplayModifiers.EnabledObstacleType.All);
            g.GetField("_noBombs", b).SetValue(gameplayModifiers, m.NoBombs);
            g.GetField("_fastNotes", b).SetValue(gameplayModifiers, false);
            g.GetField("_strictAngles", b).SetValue(gameplayModifiers, false);
            g.GetField("_disappearingArrows", b).SetValue(gameplayModifiers, m.DisappearingArrows);
            g.GetField("_songSpeed", b).SetValue(gameplayModifiers,
                m.SuperFastSong ? GameplayModifiers.SongSpeed.SuperFast :
                m.FasterSong ? GameplayModifiers.SongSpeed.Faster :
                m.SlowerSong ? GameplayModifiers.SongSpeed.Slower : GameplayModifiers.SongSpeed.Normal);
            g.GetField("_noArrows", b).SetValue(gameplayModifiers, true);//;m.NoArrows);
            g.GetField("_ghostNotes", b).SetValue(gameplayModifiers, m.GhostNotes);
            g.GetField("_proMode", b).SetValue(gameplayModifiers, m.ProMode);
            g.GetField("_zenMode", b).SetValue(gameplayModifiers, false);
            g.GetField("_smallCubes", b).SetValue(gameplayModifiers, m.SmallNotes);
        }
    }

    [HarmonyPatch(typeof(GameplaySetupViewController))]
    class Test3 {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(GameplaySetupViewController.RefreshContent))]
        static void Brubb(ref bool ____showModifiers) {
            ____showModifiers = !AddCustomCharacteristic.IsRebeat;
        }

        internal static GameplaySetupViewController GsvcInstance;
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameplaySetupViewController.Setup))]
        static void Ddeez(GameplaySetupViewController __instance) {
            GsvcInstance = __instance;
        }
    }
}