using HarmonyLib;
using UnityEngine;

namespace BeatSaber5.HarmonyPatches.Score {
    [HarmonyPatch(typeof(SaberSwingRating))]
    class SwingRating {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(SaberSwingRating.BeforeCutStepRating))]
        static bool BeforeCutRating(float angleDiff, float normalDiff, ref float __result) {
            if (!Config.Instance.Enabled) return true;
            __result = angleDiff * (1f - Mathf.Clamp((normalDiff - 75f) / 15f, 0f, 1f)) / 95f;
            return false;
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(nameof(SaberSwingRating.BeforeCutStepRating))]
        static bool AfterCutRating(float angleDiff, float normalDiff, ref float __result) {
            if (!Config.Instance.Enabled) return true;
            __result = angleDiff * (1f - Mathf.Clamp((normalDiff - 75f) / 15f, 0f, 1f)) / 55f;
            return false;
        }
    }
}