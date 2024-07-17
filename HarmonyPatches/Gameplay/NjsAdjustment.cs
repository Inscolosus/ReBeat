using System;
using HarmonyLib;
using ReBeat.HarmonyPatches.UI;

namespace ReBeat.HarmonyPatches.Gameplay {
    [HarmonyPatch(typeof(BeatmapObjectSpawnController.InitData))]
    class NjsAdjustment {
        [HarmonyPostfix]
        [HarmonyPatch(MethodType.Constructor, typeof(float), typeof(int), typeof(float), typeof(BeatmapObjectSpawnMovementData.NoteJumpValueType), typeof(float))]
        static void SetNjs(ref float ___noteJumpMovementSpeed) {
            if (!Config.Instance.Enabled) return;
            float baseNjs = ___noteJumpMovementSpeed;
            if (Modifiers.instance.ProMode) ___noteJumpMovementSpeed = ((float)Math.Pow(___noteJumpMovementSpeed, 2) + 5f*___noteJumpMovementSpeed + 15f) / (___noteJumpMovementSpeed + 18f) + 11f;
            if (GameplayModifiersPatcher.SongSpeedMultiplier <= 1) return;
            ___noteJumpMovementSpeed *= Multiplier(GameplayModifiersPatcher.SongSpeedMultiplier) / GameplayModifiersPatcher.SongSpeedMultiplier;
        }

        private static float Multiplier(float speed) {
            switch (speed) {
                case 1.2f: return 1.1f;
                case 1.5f: return 1.3f;
                default: return 1f;
            }
        }
    }
}