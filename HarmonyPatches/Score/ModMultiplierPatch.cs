using BeatSaber5.HarmonyPatches.UI;
using HarmonyLib;

namespace BeatSaber5.HarmonyPatches.Score {
    [HarmonyPatch(typeof(GameplayModifiersModelSO))]
    public class ModMultiplierPatch {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(GameplayModifiersModelSO.GetTotalMultiplier))]
        static bool SetMultiplier(ref float __result, float energy) {
            if (!Config.Instance.Enabled) return true;
            
            float mul = Modifiers.instance.Multiplier;
            if (Modifiers.instance.NoFail && energy <= 1E-05) mul -= 0.5f;
            __result = mul < 0f ? 0f : mul;
            return false;
        }
    }
}