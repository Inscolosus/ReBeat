using HarmonyLib;
using IPA.Utilities;

namespace BeatSaber5.HarmonyPatches.Score {
    [HarmonyPatch(typeof(GameplayModifiersPanelController))]
    class SetModifiersMultiplier {
        private static GameplayModifiersModelSO model;
        
        [HarmonyPrefix]
        [HarmonyPatch(nameof(GameplayModifiersPanelController.Awake))]
        static void SetMultipliers(ref GameplayModifiersPanelController __instance) {
            model = __instance.GetField<GameplayModifiersModelSO, GameplayModifiersPanelController>("_gameplayModifiersModel");
            
            model.GetField<GameplayModifierParamsSO, GameplayModifiersModelSO>("_slowerSong").SetField("_multiplier", -0.5f);
            model.GetField<GameplayModifierParamsSO, GameplayModifiersModelSO>("_fasterSong").SetField("_multiplier", 0.07f);
            model.GetField<GameplayModifierParamsSO, GameplayModifiersModelSO>("_superFastSong").SetField("_multiplier", 0.15f);
            model.GetField<GameplayModifierParamsSO, GameplayModifiersModelSO>("_smallCubes").SetField("_multiplier", 0.07f);
            model.GetField<GameplayModifierParamsSO, GameplayModifiersModelSO>("_ghostNotes").SetField("_multiplier", 0.05f);
        }

        internal static void ResetMultipliers() {
            model.GetField<GameplayModifierParamsSO, GameplayModifiersModelSO>("_slowerSong").SetField("_multiplier", -0.3f);
            model.GetField<GameplayModifierParamsSO, GameplayModifiersModelSO>("_fasterSong").SetField("_multiplier", 0.08f);
            model.GetField<GameplayModifierParamsSO, GameplayModifiersModelSO>("_superFastSong").SetField("_multiplier", 0.1f);
            model.GetField<GameplayModifierParamsSO, GameplayModifiersModelSO>("_smallCubes").SetField("_multiplier", 0f);
            model.GetField<GameplayModifierParamsSO, GameplayModifiersModelSO>("_ghostNotes").SetField("_multiplier", 0.11f);
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