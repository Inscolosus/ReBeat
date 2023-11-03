using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaber5.HarmonyPatches {
    [HarmonyPatch(typeof(ScoreMultiplierCounter), nameof(ScoreMultiplierCounter.ProcessMultiplierEvent))]
    static class ComboPatch {
        static bool Prefix(ref bool __result, ref int ____multiplier, ref int ____multiplierIncreaseProgress) {
            if (!Config.Instance.Enabled) return true;
            __result = false;

            ____multiplier = 1;
            ____multiplierIncreaseProgress = 0;

            return false;
        }
    }



    [HarmonyPatch(typeof(ScoreMultiplierUIController), nameof(ScoreMultiplierUIController.Update))]
    static class ThresholdColorPatch {
        static void Postfix(ref Image ____multiplierProgressImage) {
            ____multiplierProgressImage.color = ____multiplierProgressImage.fillAmount >= 0.75f ? Color.cyan : Color.white;
        }
    }
}
