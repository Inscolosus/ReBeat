using System.Collections.Generic;
using HarmonyLib;
using IPA.Utilities;

namespace BeatSaber5.HarmonyPatches {
    [HarmonyPatch(typeof(GameNoteController), nameof(GameNoteController.HandleCut))]
    static class BadCutPatch {
        static bool Prefix(Saber saber, GameNoteController __instance) {
            if (!Config.Instance.Enabled) return false; 
            if (saber.saberType.MatchesColorType(__instance.noteData.colorType)) return true;

            return false;
        }
    }



    [HarmonyPatch(typeof(GameplayModifiers), "get_cutAngleTolerance")]
    static class StrictAnglesPatch {
        static void Postfix(ref float __result) {
            if (Config.Instance.Enabled) {
                __result = __result < 50f ? Config.Instance.Example : 45f;
            }
        }
    }



    [HarmonyPatch(typeof(GameplayModifiers), "get_notesUniformScale")]
    static class SmallCubesPatch {
        static void Postfix(ref float __result) {
            if (__result < 1f && Config.Instance.Enabled) __result = 3f;
        }
    }
}
