using HarmonyLib;

namespace BeatSaber5.HarmonyPatches.Score {
    [HarmonyPatch(typeof(GameplayModifierParamsSO))]
    class SetModifiersMultiplier {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameplayModifierParamsSO.multiplier), MethodType.Getter)]
        static void Multiplier(ref float __result, GameplayModifierParamsSO __instance) {
            Plugin.Log.Info(__instance.modifierNameLocalizationKey);
            switch (__instance.modifierNameLocalizationKey) {
                case "MODIFIER_SLOWER_SONG":     __result = -0.5f; break;
                case "MODIFIER_FASTER_SONG":     __result = 0.07f; break;
                case "MODIFIER_SUPER_FAST_SONG": __result = 0.15f; break;
                case "MODIFIER_STRICT_ANGLES":   __result = 0f; break;
                case "MODIFIER_SMALL_CUBES":     __result = 0.07f; break;
                case "MODIFIER_GHOST_NOTES":     __result = 0.05f; break;
            }

            /* MODIFIER_NO_FAIL_ON_0_ENERGY
             * MODIFIER_NO_BOMBS
             * MODIFIER_GHOST_NOTES
             * MODIFIER_PRO_MODE
             * MODIFIER_SLOWER_SONG
             * MODIFIER_ONE_LIFE
             * MODIFIER_NO_OBSTACLES
             * MODIFIER_DISAPPEARING_ARROWS
             * MODIFIER_STRICT_ANGLES
             * MODIFIER_FASTER_SONG
             * MODIFIER_FOUR_LIVES
             * MODIFIER_NO_ARROWS
             * MODIFIER_SMALL_CUBES
             * MODIFIER_ZEN_MODE
             * MODIFIER_SUPER_FAST_SONG
             */
        }
    }
}