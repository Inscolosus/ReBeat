using System.Collections.Generic;
using HarmonyLib;

namespace BeatSaber5.HarmonyPatches.Score {
    [HarmonyPatch(typeof(ScoreModel))]
    class AngleCutScore {
        private static readonly Dictionary<NoteData.ScoringType, ScoreModel.NoteScoreDefinition> NewScoreDefinitions = new Dictionary<NoteData.ScoringType, ScoreModel.NoteScoreDefinition> {
            { NoteData.ScoringType.Ignore, null },
            { NoteData.ScoringType.NoScore,            new ScoreModel.NoteScoreDefinition(0,  0,  0,  0,  0,  0) },
            { NoteData.ScoringType.Normal,             new ScoreModel.NoteScoreDefinition(50, 0,  30, 0,  20, 0) },
            { NoteData.ScoringType.SliderHead,         new ScoreModel.NoteScoreDefinition(50, 0,  30, 20, 20, 0) },
            { NoteData.ScoringType.SliderTail,         new ScoreModel.NoteScoreDefinition(50, 30, 30, 0,  20, 0) },
            { NoteData.ScoringType.BurstSliderHead,    new ScoreModel.NoteScoreDefinition(50, 0,  30, 0,  0,  0) },
            { NoteData.ScoringType.BurstSliderElement, new ScoreModel.NoteScoreDefinition(0,  0,  0,  0,  0,  20) }
        };

        [HarmonyPostfix]
        [HarmonyPatch(nameof(ScoreModel.GetNoteScoreDefinition))]
        static void ReplaceScoreDefinition(NoteData.ScoringType scoringType, ref ScoreModel.NoteScoreDefinition __result) {
            if (!Config.Instance.Enabled) return;
            __result = NewScoreDefinitions[scoringType];
        }
    }
}