using HarmonyLib;
using UnityEngine;

namespace BeatSaber5.HarmonyPatches.Score {
    [HarmonyPatch(typeof(SaberSwingRating))]
    class SwingRating {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(SaberSwingRating.BeforeCutStepRating))]
        static bool BeforeCutRating(float angleDiff, float normalDiff, ref float __result) {
            __result = angleDiff * (1f - Mathf.Clamp((normalDiff - 75f) / 15f, 0f, 1f)) /
                       Config.Instance.BeforeCutAngle;
            return false;
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(nameof(SaberSwingRating.BeforeCutStepRating))]
        static bool AfterCutRating(float angleDiff, float normalDiff, ref float __result) {
            __result = angleDiff * (1f - Mathf.Clamp((normalDiff - 75f) / 15f, 0f, 1f)) /
                       Config.Instance.AfterCutAngle;
            return false;
        }
    }
}