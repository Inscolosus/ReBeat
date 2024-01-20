using HarmonyLib;

namespace BeatSaber5.HarmonyPatches.UI {
    [HarmonyPatch(typeof(RankModel))]
    class DisplayRank {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(RankModel.GetRankForScore))]
        static bool Prefix(ref RankModel.Rank __result) {
            float relativeScore = Score.ScoreController.TotalCutScore / (Score.ScoreController.TotalNotes * 100f);

            if (relativeScore == 1f || Score.ScoreController.TotalNotes == 0) __result = RankModel.Rank.SSS;
            if (relativeScore > 0.9) { __result = RankModel.Rank.SS; return false; }
            if (relativeScore > 0.8) { __result = RankModel.Rank.S; return false; }
            if (relativeScore > 0.65) { __result = RankModel.Rank.A; return false; }
            if (relativeScore > 0.5) { __result = RankModel.Rank.B; return false; }
            if (relativeScore > 0.35) { __result = RankModel.Rank.C; return false; }
            if (relativeScore > 0.2) { __result = RankModel.Rank.D; return false; }
            __result = RankModel.Rank.E; return false;
        }
    }
}