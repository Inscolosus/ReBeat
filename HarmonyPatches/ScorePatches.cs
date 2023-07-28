using HarmonyLib;
using IPA.Utilities;
using TMPro;

namespace BeatSaber5.HarmonyPatches {
    [HarmonyPatch(typeof(ScoreController), nameof(ScoreController.DespawnScoringElement))]
    static class AccScorePatch {
        public static int TotalCutScore;
        public static int TotalNotes;

        static void Postfix(ScoringElement scoringElement) {
            if (!Config.Instance.Enabled || scoringElement.noteData.gameplayType == NoteData.GameplayType.Bomb) return;

            TotalCutScore += scoringElement.cutScore;
            TotalNotes++;
        }
    }



    [HarmonyPatch(typeof(ScoreController), nameof(ScoreController.Start))]
    static class ScoreControllerStartPatch {
        public static ScoreController Controller = null;
        static void Postfix(ScoreController __instance) {
            if (!Config.Instance.Enabled) return;

            Controller = __instance;

            AccScorePatch.TotalCutScore = 0;
            AccScorePatch.TotalNotes = 0;
        }
    }



    [HarmonyPatch(typeof(RelativeScoreAndImmediateRankCounter), "get_relativeScore")]
    static class ScoreDisplayPatch {
        static bool Prefix(ref float __result) {
            if (!Config.Instance.Enabled) return true;

            float relativeScore = AccScorePatch.TotalCutScore / (AccScorePatch.TotalNotes * 75f);
            __result = AccScorePatch.TotalNotes == 0 ? 1 : relativeScore;
            return false;
        }
    }



    [HarmonyPatch(typeof(ScoreUIController), nameof(ScoreUIController.UpdateScore))]
    static class PtsScoreDisplayPatch {
        private static PropertyAccessor<ScoreController, int>.Getter ScoreGetter =
            PropertyAccessor<ScoreController, int>.GetGetter("modifiedScore");

        private static PropertyAccessor<ScoreController, int>.Getter MaxScoreGetter =
            PropertyAccessor<ScoreController, int>.GetGetter("immediateMaxPossibleModifiedScore");

        static void Postfix(ref TextMeshProUGUI ____scoreText) {
            if (!Config.Instance.Enabled || !Config.Instance.ShowComboPercent || ScoreControllerStartPatch.Controller == null) return;

            ____scoreText.text = ((float)ScoreGetter(ref ScoreControllerStartPatch.Controller) / (float)MaxScoreGetter(ref ScoreControllerStartPatch.Controller)).ToString("P");
        }
    }



    [HarmonyPatch(typeof(RankModel), nameof(RankModel.GetRankForScore))]
    static class RankPatch {
        static bool Prefix(ref RankModel.Rank __result) {
            if (!Config.Instance.Enabled) return true;

            float relativeScore = AccScorePatch.TotalCutScore / (AccScorePatch.TotalNotes * 75f);

            if (relativeScore == 1f || AccScorePatch.TotalNotes == 0) __result = RankModel.Rank.SSS;
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
