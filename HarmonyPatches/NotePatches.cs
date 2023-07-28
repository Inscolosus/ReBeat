using System.Collections.Generic;
using HarmonyLib;
using IPA.Utilities;

namespace BeatSaber5.HarmonyPatches {
    [HarmonyPatch(typeof(GameplayModifiers), "get_proMode")]
    static class ProModePatch {
        static void Prefix(ref bool ____proMode) {
            if (!Config.Instance.Enabled) return;
            ____proMode = true;
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
