using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage;
using HarmonyLib;
using HMUI;
using IPA.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BeatSaber5.HarmonyPatches.Score {
    [HarmonyPatch(typeof(GameplayModifiersPanelController))]
    class SetModifiersMultiplier {
        private static GameplayModifiersModelSO model;

        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameplayModifiersPanelController.Awake))]
        static void UpdateModel(ref GameplayModifiersPanelController __instance, ref ToggleBinder ____toggleBinder) {
            model = __instance.GetField<GameplayModifiersModelSO, GameplayModifiersPanelController>(
                "_gameplayModifiersModel");
            Plugin.Log.Info("gmpc awake");
            //____toggleBinder.ClearBindings();

            if (Config.Instance.Enabled) {
                SetMultipliers();
            }
            else {
                ResetMultipliers();
            }

            /*foreach (var toggle in ____gameplayModifierToggles) {
                toggle.Start();
            }*/
        }

        internal static void SetMultipliers() {
            model.GetField<GameplayModifierParamsSO, GameplayModifiersModelSO>("_slowerSong")
                .SetField("_multiplier", -0.5f);
            model.GetField<GameplayModifierParamsSO, GameplayModifiersModelSO>("_fasterSong")
                .SetField("_multiplier", 0.07f);
            model.GetField<GameplayModifierParamsSO, GameplayModifiersModelSO>("_superFastSong")
                .SetField("_multiplier", 0.15f);
            model.GetField<GameplayModifierParamsSO, GameplayModifiersModelSO>("_smallCubes")
                .SetField("_multiplier", 0.07f);
            model.GetField<GameplayModifierParamsSO, GameplayModifiersModelSO>("_ghostNotes")
                .SetField("_multiplier", 0.05f);
            //model.GetField<GameplayModifierParamsSO, GameplayModifiersModelSO>("_proMode").SetField("_multiplier", 0.12f);
            // TODO: pro mode
            // TODO: make easy mode -0.4
            // TODO: make all base game mods except ss, fs, sfs, nf 0
        }

        internal static void ResetMultipliers() {
            model.GetField<GameplayModifierParamsSO, GameplayModifiersModelSO>("_slowerSong")
                .SetField("_multiplier", -0.3f);
            model.GetField<GameplayModifierParamsSO, GameplayModifiersModelSO>("_fasterSong")
                .SetField("_multiplier", 0.08f);
            model.GetField<GameplayModifierParamsSO, GameplayModifiersModelSO>("_superFastSong")
                .SetField("_multiplier", 0.1f);
            model.GetField<GameplayModifierParamsSO, GameplayModifiersModelSO>("_smallCubes")
                .SetField("_multiplier", 0f);
            model.GetField<GameplayModifierParamsSO, GameplayModifiersModelSO>("_ghostNotes")
                .SetField("_multiplier", 0.11f);
        }
        /* no bombs -0.1
         * no walls -0.05
         * na -0.3
         * gn 0.11
         * da 0.07
         * zen mode -1
         * ss -0.3
         * fs .08
         * super fast 0.1
         */
    }
}