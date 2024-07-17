using System.Collections.Generic;
using HarmonyLib;

namespace ReBeat.HarmonyPatches.UI {
    [HarmonyPatch(typeof(GameplaySetupViewController))]
    class HideModifiersPanel {
        internal static GameplaySetupViewController GsvcInstance;
        
        [HarmonyPrefix]
        [HarmonyPatch(nameof(GameplaySetupViewController.RefreshContent))]
        static void HideGameplayModifiers(ref bool ____showModifiers) {
            ____showModifiers = !Config.Instance.Enabled;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(GameplaySetupViewController.Setup))]
        static void SetInstance(GameplaySetupViewController __instance) {
            GsvcInstance = __instance;
        }
    }
}