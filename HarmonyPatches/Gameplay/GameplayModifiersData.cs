using HarmonyLib;

namespace BeatSaber5.HarmonyPatches.Gameplay {
    [HarmonyPatch(typeof(GameplayModifiers))]
    class GameplayModifiersData {
        internal static float SongSpeedMultiplier { get; private set; }
        
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameplayModifiers.cutAngleTolerance), MethodType.Getter)]
        static void CutAngleTolerance(ref float __result) {
            __result = Config.Instance.ProMode ? 37.5f : 
                Config.Instance.EasyMode ? 52.5f : 45f;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameplayModifiers.songSpeedMul), MethodType.Getter)]
        static void SetSpeedForSlowerSong(ref float __result) {
            if (__result < 0.9f) __result = 0.75f;
            SongSpeedMultiplier = __result;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(GameplayModifiers.notesUniformScale), MethodType.Getter)]
        static bool DisableSmallNotes(ref float __result) { // rip FunnyBigNoteLohl
            Config.Instance.SameColor = __result < 1f;
            if (__result < 1f) {
                __result = 1f;
                return false;
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(GameplayModifiers.noArrows), MethodType.Getter)]
        static bool DisableNa(ref bool __result) {
            Config.Instance.EasyMode = __result;
            __result = false;
            return false;
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(nameof(GameplayModifiers.proMode), MethodType.Getter)]
        static bool DisableProMode(ref bool __result) {
            Config.Instance.ProMode = __result;
            __result = false;
            return false;
        }
    }
}