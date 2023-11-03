using System.Collections.Generic;
using HarmonyLib;

namespace BeatSaber5.HarmonyPatches {
    [HarmonyPatch(typeof(CutScoreBuffer), nameof(CutScoreBuffer.Init))]
    static class CenterDistanceCutScorePatch {
        static void Postfix(NoteCutInfo noteCutInfo, ref int ____centerDistanceCutScore) {
            if (!Config.Instance.Enabled) return;

            float sectorSize = 0.6f / 29f;
            float cutDistanceToCenter = noteCutInfo.cutDistanceToCenter;

            float[] sectors = Config.Instance.ProMode ? new[] { 4.5f, 8.5f, 11.5f, 13.5f, 14.5f } : 
                new[] { 6.5f, 9.5f, 11.5f, 13.5f, 14.5f };

            ____centerDistanceCutScore = cutDistanceToCenter < sectorSize * sectors[0] ? 25 :
                cutDistanceToCenter < sectorSize * sectors[1] ? 20 :
                cutDistanceToCenter < sectorSize * sectors[2] ? 15 :
                cutDistanceToCenter < sectorSize * sectors[3] ? 10 :
                cutDistanceToCenter < sectorSize * sectors[4] ? 5 : 0;
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
}
