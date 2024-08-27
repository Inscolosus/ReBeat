using HarmonyLib;
using ReBeat.HarmonyPatches.UI;

namespace ReBeat.HarmonyPatches.Gameplay {
    [HarmonyPatch(typeof(GameplayModifiers))]
    class GameplayModifiersPatcher {
        public static float SongSpeedMultiplier { get; private set; }
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameplayModifiers.songSpeedMul), MethodType.Getter)]
        static void SongSpeedMul(float __result) {
            if (!Config.Instance.Enabled) return;
            SongSpeedMultiplier = __result;
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameplayModifiers.cutAngleTolerance), MethodType.Getter)]
        static void CutAngleTolerance(ref float __result) {
            if (!Config.Instance.Enabled) return;
            __result = Modifiers.instance.ProMode ? 37.5f : 
                Modifiers.instance.EasyMode ? 52.5f : 45f;
        }
    }
}