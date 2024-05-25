using HarmonyLib;

namespace BeatSaber5.HarmonyPatches.UI {
    [HarmonyPatch(typeof(GameplaySetupViewController), nameof(GameplaySetupViewController.RefreshContent))]
    class HideModifiersPanel {
        [HarmonyPrefix]
        static void J(ref bool ____showModifiers) {
            if (Config.Instance.Enabled) ____showModifiers = false;
        }
    }
}