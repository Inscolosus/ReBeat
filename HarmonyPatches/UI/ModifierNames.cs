using System.Collections.Generic;
using HarmonyLib;
using HMUI;
using IPA.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaber5.HarmonyPatches.UI {
    [HarmonyPatch(typeof(GameplayModifierToggle))]
    public class ModifierNames {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameplayModifierToggle.Start))]
        static void CustomModifiers(ref TextMeshProUGUI ____nameText, ref Image ____icon) {
            if (!Config.Instance.Enabled) return;
            switch (____nameText.text) {
                case "No Arrows": ____nameText.text = "Easy Mode"; break;
                case "Small Notes (beta)": ____nameText.text = "Same Color"; break;
            }
        }
    }


    [HarmonyPatch(typeof(GameplaySetupViewController), nameof(GameplaySetupViewController.RefreshContent))]
    class ReplaceModsTest {
        [HarmonyPrefix]
        static void J(ref bool ____showModifiers) {
            if (Config.Instance.Enabled) ____showModifiers = false;
        }
    }
}