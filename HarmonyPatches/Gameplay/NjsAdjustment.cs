using System;
using HarmonyLib;

namespace BeatSaber5.HarmonyPatches.Gameplay {
    [HarmonyPatch(typeof(BeatmapObjectSpawnController.InitData))]
    class NjsAdjustment {
        [HarmonyPostfix]
        [HarmonyPatch(MethodType.Constructor, typeof(float), typeof(int), typeof(float), typeof(BeatmapObjectSpawnMovementData.NoteJumpValueType), typeof(float))]
        static void SetNjs(ref float ___noteJumpMovementSpeed) {
            float baseNjs = ___noteJumpMovementSpeed;
            if (Config.Instance.ProMode) ___noteJumpMovementSpeed = ((float)Math.Pow(___noteJumpMovementSpeed, 2) + 5f*___noteJumpMovementSpeed + 15f) / (___noteJumpMovementSpeed + 18f) + 11f;
            if (GameplayModifiersData.SongSpeedMultiplier <= 1) return;
            ___noteJumpMovementSpeed *= Multiplier(GameplayModifiersData.SongSpeedMultiplier) / GameplayModifiersData.SongSpeedMultiplier;
        }

        private static float Multiplier(float speed) {
            switch (speed) {
                case 1.2f: return 1.1f;
                case 1.5f: return 1.2f;
                default: return 1f;
            }
        }
    }
}