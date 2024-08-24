using HarmonyLib;
using ReBeat.HarmonyPatches.UI;
using UnityEngine;

namespace ReBeat.HarmonyPatches.Gameplay.ModifierPatches {
    [HarmonyPatch(typeof(ColorManager))]
    public class SameColor {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(ColorManager.ColorForType), typeof(ColorType))]
        static bool NoteColorPatch(ref Color __result, ref ColorScheme ____colorScheme, ColorType type) {
            if (!Config.Instance.Enabled) return true;
            if (!Modifiers.instance.SameColor) return true;
            
            if (type == ColorType.None) __result = Color.black;
            __result = Config.Instance.UseLeftColor ? __result = ____colorScheme.saberAColor : 
                __result = ____colorScheme.saberBColor;
            
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(ColorManager.ColorForSaberType))]
        static bool SaberColorPatch(ref ColorScheme ____colorScheme, ref Color __result) {
            if (!Config.Instance.Enabled) return true;
            if (!Modifiers.instance.SameColor) return true;

            __result = Config.Instance.UseLeftColor ? ____colorScheme.saberAColor : ____colorScheme.saberBColor;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(ColorManager.EffectsColorForSaberType))]
        static bool SaberEffectsColorPatch(ref ColorScheme ____colorScheme, ref Color __result) {
            if (!Config.Instance.Enabled) return true;
            if (!Modifiers.instance.SameColor) return true;

            float h, s;
            if (Config.Instance.UseLeftColor) Color.RGBToHSV(____colorScheme.saberAColor, out h, out s, out _);
            else Color.RGBToHSV(____colorScheme.saberBColor, out h, out s, out _);
            
            __result = Color.HSVToRGB(h, s, 1f);
            return false;
        }
    }
}