using HarmonyLib;
using IPA.Utilities;

namespace ReBeat.HarmonyPatches.UI {
    [HarmonyPatch(typeof(GameplaySetupViewController))]
    class ResetModifiers {
        internal static GameplaySetupViewController GsvcInstance;
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameplaySetupViewController.Setup))]
        static void GetGsvcInstance(GameplaySetupViewController __instance) {
            GsvcInstance = __instance;
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(nameof(GameplaySetupViewController.RefreshContent))]
        static void LoadSavedModifiers(ref bool ____showModifiers, ref GameplayModifiersPanelController ____gameplayModifiersPanelController) {
            ____showModifiers = !Config.Instance.Enabled;
            if (!Config.loadMods || Config.modifiers is null) return;
            
            var m = Config.modifiers;
            ____gameplayModifiersPanelController.gameplayModifiers.SetField("_energyType", m.energyType);
            ____gameplayModifiersPanelController.gameplayModifiers.SetField("_noFailOn0Energy", m.noFailOn0Energy);
            ____gameplayModifiersPanelController.gameplayModifiers.SetField("_instaFail", m.instaFail);
            ____gameplayModifiersPanelController.gameplayModifiers.SetField("_failOnSaberClash", m.failOnSaberClash);
            ____gameplayModifiersPanelController.gameplayModifiers.SetField("_enabledObstacleType", m.enabledObstacleType);
            ____gameplayModifiersPanelController.gameplayModifiers.SetField("_noBombs", m.noBombs);
            ____gameplayModifiersPanelController.gameplayModifiers.SetField("_fastNotes", m.fastNotes);
            ____gameplayModifiersPanelController.gameplayModifiers.SetField("_strictAngles", m.strictAngles);
            ____gameplayModifiersPanelController.gameplayModifiers.SetField("_disappearingArrows", m.disappearingArrows);
            ____gameplayModifiersPanelController.gameplayModifiers.SetField("_songSpeed", m.songSpeed);
            ____gameplayModifiersPanelController.gameplayModifiers.SetField("_noArrows", m.noArrows);
            ____gameplayModifiersPanelController.gameplayModifiers.SetField("_ghostNotes", m.ghostNotes);
            ____gameplayModifiersPanelController.gameplayModifiers.SetField("_proMode", m.proMode);
            ____gameplayModifiersPanelController.gameplayModifiers.SetField("_zenMode", m.zenMode);
            ____gameplayModifiersPanelController.gameplayModifiers.SetField("_smallCubes", m.smallCubes);

            Config.loadMods = false;
        }
    }
}