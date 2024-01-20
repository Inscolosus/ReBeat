using HarmonyLib;

namespace BeatSaber5.HarmonyPatches.Score {
    [HarmonyPatch(typeof(ScoreMultiplierCounter))]
    static class DisableComboMultiplier {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(ScoreMultiplierCounter.ProcessMultiplierEvent))]
        static bool CancelMultiplierEvent(ref bool __result) {
            __result = false;
            return false;
        }
    }
}
