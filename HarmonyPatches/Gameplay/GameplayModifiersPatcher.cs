using HarmonyLib;
using ReBeat.HarmonyPatches.UI;

namespace ReBeat.HarmonyPatches.Gameplay {
    [HarmonyPatch(typeof(GameplayModifiers))]
    class GameplayModifiersPatcher {
        internal static GameplayModifiers ModifiersInstance;
        [HarmonyPostfix]
        [HarmonyPatch(MethodType.Constructor, typeof(GameplayModifiers.EnergyType), typeof(bool), typeof(bool), typeof(bool), typeof(GameplayModifiers.EnabledObstacleType), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(GameplayModifiers.SongSpeed), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(bool))]
        static void GetModifiersInstance(GameplayModifiers __instance) {
            ModifiersInstance = __instance;
        }
        
        public static float SongSpeedMultiplier { get; private set; }
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameplayModifiers.songSpeedMul), MethodType.Getter)]
        static void SongSpeedMul(ref float __result) {
            if (!Config.Instance.Enabled) return;
            if (__result < 0.9f) __result = 0.75f;
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