using HarmonyLib;

namespace BeatSaber5.HarmonyPatches.Score {
    [HarmonyPatch(typeof(CutScoreBuffer))]
    class CenterCutScore {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(CutScoreBuffer.Init))]
        static void SetCenterCutScore(NoteCutInfo noteCutInfo, ref int ____centerDistanceCutScore) {
            if (!Config.Instance.Enabled) return;
            float sectorSize = 0.6f / 29f;
            float cutDistanceToCenter = noteCutInfo.cutDistanceToCenter;

            float[] sectors = Config.Instance.ProMode ? new[] { 4.5f, 8.5f, 11.5f, 13.5f, 14.5f } : 
                Config.Instance.EasyMode ? new[] { 7.5f, 10.5f, 12.5f, 13.5f, 14.5f } : 
                new[] { 6.5f, 9.5f, 11.5f, 13.5f, 14.5f };

            ____centerDistanceCutScore = cutDistanceToCenter < sectorSize * sectors[0] ? 50 :
                cutDistanceToCenter < sectorSize * sectors[1] ? 44 :
                cutDistanceToCenter < sectorSize * sectors[2] ? 36 :
                cutDistanceToCenter < sectorSize * sectors[3] ? 22 :
                cutDistanceToCenter < sectorSize * sectors[4] ? 10 : 0;
        }
    }
}