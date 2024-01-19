using System;
using BeatSaberMarkupLanguage;
using HarmonyLib;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace BeatSaber5.HarmonyPatches {
    // force pro mode
    /*[HarmonyPatch(typeof(BeatmapObjectsInstaller), nameof(BeatmapObjectsInstaller.InstallBindings))]
    public class ProModePatch {
        static void Prefix(ref GameNoteController ____normalBasicNotePrefab, ref GameNoteController ____proModeNotePrefab) {
            if (!Config.Instance.Enabled) return;
            ____normalBasicNotePrefab = ____proModeNotePrefab;
        }
    }*/

    // COMMENTED OUT CODE IN MASTER :20:
    /*[HarmonyPatch(typeof(SphereCuttableBySaber), nameof(SphereCuttableBySaber.Awake))]
    static class BigBombPatch {
        static void Prefix(ref SphereCollider ____collider) {
            if (ModifierTogglePatch.BigBombs) ____collider.radius = 10.25f;
        }
    }

    [HarmonyPatch(typeof(GameplayModifierToggle), "get_toggle")]
    static class ModifierTogglePatch {
        public static bool BigBombs;
        static void Postfix(ref Toggle __result, ref GameplayModifierParamsSO ____gameplayModifier) {
            switch (____gameplayModifier.modifierNameLocalizationKey) {
                case "MODIFIER_DISAPPEARING_ARROWS": BigBombs = __result.isOn; 
                    __result.isOn = false; break;
            }
        }
    }*/
    
    // tbh I didn't really realize you could do this :skull:
    // will have to go back over the code and redo annotations like this for methods in the same class LMAOO
    [HarmonyPatch(typeof(ColorManager))]
    static class SameColorPatch {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(ColorManager.ColorForType), typeof(ColorType))]
        static bool NoteColorPatch(ref Color __result, ref ColorScheme ____colorScheme, ColorType type) {
            if (!Config.Instance.SameColor) return true;
            
            if (type == ColorType.None) __result = Color.black;
            __result = Config.Instance.UseLeftColor ? __result = ____colorScheme.saberAColor : 
                __result = ____colorScheme.saberBColor;
            
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(ColorManager.ColorForSaberType))]
        static bool SaberColorPatch(ref ColorScheme ____colorScheme, ref Color __result) {
            if (!Config.Instance.SameColor) return true;

            __result = Config.Instance.UseLeftColor ? ____colorScheme.saberAColor : ____colorScheme.saberBColor;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(ColorManager.EffectsColorForSaberType))]
        static bool SaberEffectsColorPatch(ref ColorScheme ____colorScheme, ref Color __result) {
            if (!Config.Instance.SameColor) return true;

            float h, s, v;
            if (Config.Instance.UseLeftColor) Color.RGBToHSV(____colorScheme.saberAColor, out h, out s, out v);
            else Color.RGBToHSV(____colorScheme.saberBColor, out h, out s, out v);
            
            __result = Color.HSVToRGB(h, s, 1f);
            return false;
        }
    }

    
    
    [HarmonyPatch(typeof(BoxCuttableBySaber), nameof(BoxCuttableBySaber.Awake))]
    static class ColliderBruhPatch {
        public static void Prefix(ref BoxCollider ____collider) {
            float j = Config.Instance.DebugHitboxSize;
            ____collider.size = Config.Instance.ProMode ? new Vector3(0.45f, 0.45f, 0.45f) :
                Config.Instance.DebugHitbox ? new Vector3(j,j,j) : 
            new Vector3(0.5f, 0.5f, 0.5f);
        }
    }



    [HarmonyPatch(typeof(BeatmapObjectSpawnController.InitData), MethodType.Constructor, typeof(float), typeof(int), typeof(float), typeof(BeatmapObjectSpawnMovementData.NoteJumpValueType), typeof(float))]
    public class NjsPatch {
        static void Postfix(ref float ___noteJumpMovementSpeed) {
            if (Config.Instance.DebugTwo) Plugin.Log.Debug($"pre {___noteJumpMovementSpeed}");
            float baseNjs = ___noteJumpMovementSpeed;
            if (Config.Instance.ProMode) ___noteJumpMovementSpeed = ((float)Math.Pow(___noteJumpMovementSpeed, 2) + 5f*___noteJumpMovementSpeed + 15f) / (___noteJumpMovementSpeed + 18f) + 11f;
            if (Config.Instance.DebugTwo) Plugin.Log.Debug($"promode {___noteJumpMovementSpeed}");
            if (SongSpeedPatch.SongSpeed <= 1) return;
            ___noteJumpMovementSpeed *= Multiplier(SongSpeedPatch.SongSpeed) / SongSpeedPatch.SongSpeed;
            if (Config.Instance.DebugTwo) Plugin.Log.Debug($"all {___noteJumpMovementSpeed}");
        }

        private static float Multiplier(float speed) {
            switch (speed) {
                case 1.2f: return 1.1f;
                case 1.5f: return 1.2f;
                default: return 1f;
            }
        }
    }

    [HarmonyPatch(typeof(GameplayModifiers), "get_songSpeedMul")]
    public class SongSpeedPatch {
        public static float SongSpeed;

        static void Postfix(ref float __result) {
            if (__result < 0.9f) __result = 0.75f;
            SongSpeed = __result;
        }
    }

    // looks like something with this is bugged if u start the game with the mod off
    [HarmonyPatch(typeof(GameplayModifierParamsSO), "get_multiplier")]
    public class MultiplierPatch {
        static void Postfix(ref float __result, GameplayModifierParamsSO __instance) {
            switch (__instance.modifierNameLocalizationKey) {
                case "MODIFIER_SLOWER_SONG":     __result = -0.5f; break;
                case "MODIFIER_FASTER_SONG":     __result = 0.07f; break;
                case "MODIFIER_SUPER_FAST_SONG": __result = 0.15f; break;
                case "MODIFIER_STRICT_ANGLES":   __result = 0.11f; break;
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


    
    /*[HarmonyPatch(typeof(GameplayModifierToggle), nameof(GameplayModifierToggle.Start))]
    public class ModifierNamesPatch {
        static void Postfix(ref TextMeshProUGUI ____nameText, ref GameplayModifierParamsSO ____gameplayModifier) {
            switch (____gameplayModifier.modifierNameLocalizationKey) {
                case "MODIFIER_FOUR_LIVES":          ____nameText.text = "1 Health"; break;
                case "MODIFIER_DISAPPEARING_ARROWS": ____nameText.text = "Full Size Bombs"; break;
            }
        }
    }*/
}