﻿using HarmonyLib;

namespace ReBeat.HarmonyPatches.UI {
    [HarmonyPatch(typeof(RelativeScoreAndImmediateRankCounter))]
    class RelativeScore {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(RelativeScoreAndImmediateRankCounter.relativeScore), MethodType.Getter)]
        static bool SetRelativeScore(ref float __result) {
            if (!Config.Instance.Enabled) return true;
            float relativeScore = Config.Instance.ShowComboPercent ? (float)Score.ScoreController.CurrentScore / (float)Score.ScoreController.CurrentMaxScore : 
                Score.ScoreController.TotalCutScore / (Score.ScoreController.TotalNotes * 100f);
            
            __result = Score.ScoreController.TotalNotes == 0 ? 1 : relativeScore;
            return false;
        }
    }
}