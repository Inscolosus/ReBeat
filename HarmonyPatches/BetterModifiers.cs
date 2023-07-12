using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

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

    [HarmonyPatch(typeof(ScoreModel.NoteScoreDefinition), MethodType.Constructor, typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int))]
    static class AngleScoresPatch {
        static void Postfix(ref int ___maxCenterDistanceCutScore, ref int ___maxBeforeCutScore, ref int ___maxAfterCutScore) {
            if (!Config.Instance.Enabled) return;
            ___maxCenterDistanceCutScore = 25;
            ___maxBeforeCutScore = 30;
            ___maxAfterCutScore = 20;
        }
    }


    
    [HarmonyPatch(typeof(GameplayModifiers), "get_notesUniformScale")]
    static class SmallCubesPatch {
        static void Postfix(ref float __result) {
            if (__result < 1f && Config.Instance.Enabled) __result = 100f;
        }
    }
}
