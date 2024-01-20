using HarmonyLib;

namespace BeatSaber5.HarmonyPatches.Gameplay {
    [HarmonyPatch(typeof(GameplayModifiers))]
    class GameplayModifiersData {
        internal static float SongSpeedMultiplier { get; private set; }
        
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameplayModifiers.cutAngleTolerance), MethodType.Getter)]
        static void CutAngleTolerance(ref float __result) {
            __result = Config.Instance.ProMode ? 37.5f : 45f;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameplayModifiers.songSpeedMul), MethodType.Getter)]
        static void SetSpeedForSlowerSong(ref float __result) {
            if (__result < 0.9f) __result = 0.75f;
            SongSpeedMultiplier = __result;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameplayModifiers.notesUniformScale), MethodType.Getter)]
        static void FunnyBigNoteLohl(ref float __result) {
            if (__result < 1f) __result = 3f;
        }
    }
}