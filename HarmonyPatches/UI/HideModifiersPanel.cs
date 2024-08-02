using System.Collections.Generic;
using HarmonyLib;
using HMUI;

namespace ReBeat.HarmonyPatches.UI {
    [HarmonyPatch(typeof(GameplaySetupViewController))]
    class HideModifiersPanel {
        internal static GameplaySetupViewController GsvcInstance;
        
        [HarmonyPrefix]
        [HarmonyPatch(nameof(GameplaySetupViewController.RefreshContent))]
        static void HideGameplayModifiers(ref bool ____showModifiers){//}ref TextSegmentedControl ____selectionSegmentedControl, List<GameplaySetupViewController.Panel> ____panels, GameplaySetupViewController __instance) {
            ____showModifiers = !Config.Instance.Enabled;
            /*if (!Config.Instance.Enabled) return;
            List<string> list = new List<string>(____panels.Count);
            foreach (var panel in ____panels) {
                if (panel.refreshable is GameplayModifiersPanelController) continue;
                list.Add(panel.title);
            }
            //list.Add("SABERS(SF)");
            ____selectionSegmentedControl.SetTexts(list);*/
        }

        /*[HarmonyPrefix]
        [HarmonyPatch(nameof(GameplaySetupViewController.SetActivePanel))]
        static void SetIndex(ref int panelIdx) {
            if (Config.Instance.Enabled) panelIdx++;
        }
*/
        [HarmonyPrefix]
        [HarmonyPatch(nameof(GameplaySetupViewController.Setup))]
        static void SetInstance(GameplaySetupViewController __instance) {
            GsvcInstance = __instance;
        }
    }
}