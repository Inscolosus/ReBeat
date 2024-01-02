using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace BeatSaber5.HarmonyPatches {
    [HarmonyPatch(typeof(CutScoreBuffer), nameof(CutScoreBuffer.Init))]
    static class CenterDistanceCutScorePatch {
        static void Postfix(NoteCutInfo noteCutInfo, ref int ____centerDistanceCutScore) {
            if (!Config.Instance.Enabled) return;

            float sectorSize = 0.6f / 29f;
            float cutDistanceToCenter = noteCutInfo.cutDistanceToCenter;

            float[] sectors = Config.Instance.ProMode ? new[] { 4.5f, 8.5f, 11.5f, 13.5f, 14.5f } : 
                new[] { 6.5f, 9.5f, 11.5f, 13.5f, 14.5f };

            ____centerDistanceCutScore = cutDistanceToCenter < sectorSize * sectors[0] ? 50 :
                cutDistanceToCenter < sectorSize * sectors[1] ? 40 :
                cutDistanceToCenter < sectorSize * sectors[2] ? 30 :
                cutDistanceToCenter < sectorSize * sectors[3] ? 20 :
                cutDistanceToCenter < sectorSize * sectors[4] ? 10 : 0;
        }
    }



    [HarmonyPatch(typeof(ScoreModel), nameof(ScoreModel.GetNoteScoreDefinition))]
    static class AngleCutScorePatch {
        private static readonly Dictionary<NoteData.ScoringType, ScoreModel.NoteScoreDefinition> NewScoreDefinitions = new Dictionary<NoteData.ScoringType, ScoreModel.NoteScoreDefinition> {
            { NoteData.ScoringType.Ignore, null },
            { NoteData.ScoringType.NoScore,            new ScoreModel.NoteScoreDefinition(0,  0,  0,  0,  0,  0) },
            { NoteData.ScoringType.Normal,             new ScoreModel.NoteScoreDefinition(25, 0,  30, 0,  20, 0) },
            { NoteData.ScoringType.SliderHead,         new ScoreModel.NoteScoreDefinition(25, 0,  30, 20, 20, 0) },
            { NoteData.ScoringType.SliderTail,         new ScoreModel.NoteScoreDefinition(25, 30, 30, 0,  20, 0) },
            { NoteData.ScoringType.BurstSliderHead,    new ScoreModel.NoteScoreDefinition(25, 0,  30, 0,  0,  0) },
            { NoteData.ScoringType.BurstSliderElement, new ScoreModel.NoteScoreDefinition(0,  0,  0,  0,  0,  20) }
        };

        static void Postfix(NoteData.ScoringType scoringType, ref ScoreModel.NoteScoreDefinition __result) {
            if (!Config.Instance.Enabled) return;

            __result = NewScoreDefinitions[scoringType];
        }
    }

    [HarmonyPatch(typeof(SaberSwingRating), nameof(SaberSwingRating.BeforeCutStepRating))]
    static class BeforeCutAnglePatch {
        static bool Prefix(float angleDiff, float normalDiff, ref float __result) {
            __result = angleDiff * (1f - Mathf.Clamp((normalDiff - 75f) / 15f, 0f, 1f)) / Config.Instance.BeforeCutAngle;
            return false;
        }
    }

    [HarmonyPatch(typeof(SaberSwingRating), nameof(SaberSwingRating.AfterCutStepRating))]
    static class AfterCutAnglePatch {
        static bool Prefix(float angleDiff, float normalDiff, ref float __result) {
            __result = angleDiff * (1f - Mathf.Clamp((normalDiff - 75f) / 15f, 0f, 1f)) / Config.Instance.AfterCutAngle;
            return false;
        }
    }
}
