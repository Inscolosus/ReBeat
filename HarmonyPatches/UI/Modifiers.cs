using HarmonyLib;
using TMPro;

namespace BeatSaber5.HarmonyPatches.UI {
    [HarmonyPatch(typeof(GameplayModifierToggle))]
    public class Modifiers {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameplayModifierToggle.Start))]
        static void CustomModifiers(ref TextMeshProUGUI ____nameText) {
            switch (____nameText.text) {
                case "No Arrows": ____nameText.text = "Easy Mode"; break;
                case "Small Notes (beta)": ____nameText.text = "Same Color"; break;
            }
        }
    }
}