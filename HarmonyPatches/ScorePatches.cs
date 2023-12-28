using System;
using System.Collections.Generic;
using System.Globalization;
using HarmonyLib;
using IPA.Utilities;
using TMPro;

namespace BeatSaber5.HarmonyPatches {
    [HarmonyPatch(typeof(ScoreController), nameof(ScoreController.DespawnScoringElement))]
    static class AccScorePatch {
        public static int TotalCutScore;
        public static int TotalNotes;

        static void Postfix(ScoringElement scoringElement) {
            if (scoringElement.noteData.gameplayType == NoteData.GameplayType.Bomb) return;

            TotalCutScore += scoringElement.cutScore;
            TotalNotes++;
        }
    }



    [HarmonyPatch(typeof(ScoreController), nameof(ScoreController.Start))]
    static class ScoreControllerStartPatch {
        public static ScoreController Controller = null;
        static void Postfix(ScoreController __instance) {
            Controller = __instance;

            AccScorePatch.TotalCutScore = 0;
            AccScorePatch.TotalNotes = 0;
        }
    }



    [HarmonyPatch(typeof(RelativeScoreAndImmediateRankCounter), "get_relativeScore")]
    static class ScoreDisplayPatch {
        static bool Prefix(ref float __result) {
            float relativeScore = AccScorePatch.TotalCutScore / (AccScorePatch.TotalNotes * 75f);
            __result = AccScorePatch.TotalNotes == 0 ? 1 : relativeScore;
            return false;
        }
    }



    [HarmonyPatch(typeof(ScoreController), nameof(ScoreController.LateUpdate))]
    static class ScorePatch {
        // I think this needs to be a transpiler
        private static string msg = "";
        static void Postfix(ref int ____multipliedScore, ref int ____modifiedScore,
            ref int ____immediateMaxPossibleMultipliedScore, ref int ____immediateMaxPossibleModifiedScore,
            ref GameplayModifiersModelSO ____gameplayModifiersModel,
            ref List<GameplayModifierParamsSO> ____gameplayModifierParams,
            ref IGameEnergyCounter ____gameEnergyCounter, ref Action<int, int> ___scoreDidChangeEvent) {
            if (ScoreControllerStartPatch.Controller == null) return;

            double acc = ((double)AccScorePatch.TotalCutScore / ((double)AccScorePatch.TotalNotes*75d))*100d;
            int noteCount = TotalNotesPatch.CuttableNotesCount;
            int misses = EnergyPatch.TotalMisses;
            int maxCombo = EnergyPatch.HighestCombo;

            double missCountCurve = noteCount / (50 * Math.Pow(misses, 2) + noteCount) * ((50d * noteCount + 1) / (50d * noteCount)) - 1 / (50d * noteCount);
            double maxComboCurve = Math.Pow(noteCount / ((1 - Math.Sqrt(0.5)) * maxCombo - noteCount), 2) - 1;
            //const double j = 1d / 1020734678369717893d;
            double accCurve = (19.0444 * Math.Tan((Math.PI / 133d) * acc - 4.22) + 35.5) * 0.01; // rip j

            int score = AccScorePatch.TotalCutScore == 0 || AccScorePatch.TotalNotes == 0 ? 0 : (int)(1_000_000d * ((missCountCurve * 0.3) + (maxComboCurve * 0.3) + (accCurve * 0.4)) * ((double)AccScorePatch.TotalNotes / (double)noteCount));

            if (!msg.Equals($"{acc} {noteCount} {misses} {maxCombo} | {missCountCurve} {maxComboCurve} {accCurve} | {score}") && Config.Instance.ScoreDebug) {
                msg = $"{acc} {noteCount} {misses} {maxCombo} | {missCountCurve} {maxComboCurve} {accCurve} | {score}";
                Plugin.Log.Debug(msg);
            }

            ____multipliedScore = score;
            ____immediateMaxPossibleMultipliedScore = (int)(score / (1_000_000d * ((double)AccScorePatch.TotalNotes / (double)noteCount)));

            float totalMultiplier = ____gameplayModifiersModel.GetTotalMultiplier(____gameplayModifierParams, ____gameEnergyCounter.energy);
            ____modifiedScore = ScoreModel.GetModifiedScoreForGameplayModifiersScoreMultiplier(____multipliedScore, totalMultiplier);
            ____immediateMaxPossibleModifiedScore = ScoreModel.GetModifiedScoreForGameplayModifiersScoreMultiplier(____immediateMaxPossibleMultipliedScore, totalMultiplier);

            Action<int, int> action = ___scoreDidChangeEvent;
            if (action == null) return;
            action(____multipliedScore, ____modifiedScore);
        }
    }


    [HarmonyPatch(typeof(RelativeScoreAndImmediateRankCounter), "get_relativeScore")]
    static class RelativeScoreDisplayPatch {
        static void Postfix(ref float __result) {
            if (Config.Instance.ShowComboPercent) {
                __result = (float)AccScorePatch.TotalCutScore / (AccScorePatch.TotalNotes * 75f);
            }
        }
    }

    [HarmonyPatch(typeof(BeatmapDataLoader), nameof(BeatmapDataLoader.GetBeatmapDataBasicInfoFromSaveData))]
    static class TotalNotesPatch {
        internal static int CuttableNotesCount;
        static void Postfix(BeatmapSaveDataVersion3.BeatmapSaveData beatmapSaveData) {
            CuttableNotesCount = beatmapSaveData.colorNotes.Count;
        }
    }



    [HarmonyPatch(typeof(RankModel), nameof(RankModel.GetRankForScore))]
    static class RankPatch {
        static bool Prefix(ref RankModel.Rank __result) {
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
