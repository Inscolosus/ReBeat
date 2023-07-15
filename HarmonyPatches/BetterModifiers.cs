using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace BeatSaber5.HarmonyPatches {
    [HarmonyPatch(typeof(GameplayModifiers), "get_cutAngleTolerance")]
    static class StrictAnglesPatch {
        static void Postfix(ref float __result) {
            if (Config.Instance.Enabled) {
                __result = __result < 50f ? Config.Instance.Example : 45f;
            }
        }
    }



    [HarmonyPatch(typeof(CutScoreBuffer), nameof(CutScoreBuffer.Init))]
    static class CenterDistanceCutScorePatch {
        static void Postfix(NoteCutInfo noteCutInfo, ref int ____centerDistanceCutScore) {
            if (!Config.Instance.Enabled) return;

            float sectorSize = 0.6f / 29f;
            float cutDistanceToCenter = noteCutInfo.cutDistanceToCenter;

            ____centerDistanceCutScore = cutDistanceToCenter < sectorSize * 4.5 ? 25 :
                cutDistanceToCenter < sectorSize * 8.5 ? 20 :
                cutDistanceToCenter < sectorSize * 11.5 ? 15 :
                cutDistanceToCenter < sectorSize * 13.5 ? 10 :
                cutDistanceToCenter < sectorSize * 14.5 ? 5 : 0;
        }
    }

    [HarmonyPatch(typeof(ScoreModel), nameof(ScoreModel.GetNoteScoreDefinition))]
    static class AngleScoresPatch {
        private static readonly Dictionary<NoteData.ScoringType, ScoreModel.NoteScoreDefinition> NewScoreDefinitions = new Dictionary<NoteData.ScoringType, ScoreModel.NoteScoreDefinition>
        {
            {
                NoteData.ScoringType.Ignore,
                null
            },
            {
                NoteData.ScoringType.NoScore,
                new ScoreModel.NoteScoreDefinition(0, 0, 0, 0, 0, 0)
            },
            {
                NoteData.ScoringType.Normal,
                new ScoreModel.NoteScoreDefinition(25, 0, 30, 0, 20, 0)
            },
            {
                NoteData.ScoringType.SliderHead,
                new ScoreModel.NoteScoreDefinition(25, 0, 30, 20, 20, 0)
            },
            {
                NoteData.ScoringType.SliderTail,
                new ScoreModel.NoteScoreDefinition(25, 30, 30, 0, 20, 0)
            },
            {
                NoteData.ScoringType.BurstSliderHead,
                new ScoreModel.NoteScoreDefinition(25, 0, 30, 0, 0, 0)
            },
            {
                NoteData.ScoringType.BurstSliderElement,
                new ScoreModel.NoteScoreDefinition(0, 0, 0, 0, 0, 20)
            }
        };

        static void Postfix(NoteData.ScoringType scoringType, ref ScoreModel.NoteScoreDefinition __result) {
            if (!Config.Instance.Enabled) return;

            __result = NewScoreDefinitions[scoringType];
        }
    }



    [HarmonyPatch(typeof(ScoreMultiplierCounter), nameof(ScoreMultiplierCounter.ProcessMultiplierEvent))]
    static class ComboPatch {
        static bool Prefix(ref bool __result, ScoreMultiplierCounter.MultiplierEventType multiplierEventType, ref int ____multiplier, ref int ____multiplierIncreaseProgress, ref int ____multiplierIncreaseMaxProgress) {
            if (!Config.Instance.Enabled) return true;
            __result = false;

            int multiplierMax = 1073741824; //2^30
            int threshold = ____multiplierIncreaseMaxProgress == 2 ? 1 : (____multiplierIncreaseMaxProgress) / 4 * 3;

            if (multiplierEventType == ScoreMultiplierCounter.MultiplierEventType.Positive) {
                if (____multiplier < multiplierMax) {
                    if (____multiplierIncreaseProgress < ____multiplierIncreaseMaxProgress) {
                        ____multiplierIncreaseProgress++;
                        __result = true;
                    }
                    if (____multiplierIncreaseProgress >= ____multiplierIncreaseMaxProgress) {
                        ____multiplier *= 2;
                        ____multiplierIncreaseProgress = 0;
                        ____multiplierIncreaseMaxProgress = ____multiplier * 2;
                        __result = true;
                    }
                }
            }
            else if (multiplierEventType == ScoreMultiplierCounter.MultiplierEventType.Negative) {
                if (____multiplierIncreaseProgress > threshold) {
                    ____multiplierIncreaseProgress = 0;
                    __result = true;
                }
                else if (____multiplier > 1) {
                    ____multiplier /= 2;
                    ____multiplierIncreaseMaxProgress = ____multiplier * 2;
                    ____multiplierIncreaseProgress = ____multiplier; // maxProgress / 2
                    __result = true;
                }
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(ScoreController), nameof(ScoreController.DespawnScoringElement))]
    static class AccScorePatch {
        public static int TotalCutScore;
        public static int TotalNotes;

        static void Postfix(ScoringElement scoringElement) {
            if (!Config.Instance.Enabled) return;

            TotalCutScore += scoringElement.cutScore;
            TotalNotes++;
        }
    }

    [HarmonyPatch(typeof(ScoreController), nameof(ScoreController.Start))]
    static class ScoreControllerStartPatch {
        static void Postfix() {
            if (!Config.Instance.Enabled) return;

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


    [HarmonyPatch(typeof(GameplayModifiers), "get_notesUniformScale")]
    static class SmallCubesPatch {
        static void Postfix(ref float __result) {
            if (__result < 1f && Config.Instance.Enabled) __result = 3f;
        }
    }
}
