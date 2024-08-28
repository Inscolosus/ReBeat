using HarmonyLib;

namespace ReBeat.HarmonyPatches.Score {
    [HarmonyPatch(typeof(ScoreSaber.Plugin))]
    class DisableScoresaberSubmission {
        [HarmonyPatch(nameof(ScoreSaber.Plugin.ScoreSubmission), MethodType.Getter)]
        static bool Prefix(bool __result) {
            if (!Config.Instance.Enabled) return true;
            __result = false;
            return false;
        }
    }
}