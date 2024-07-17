using HarmonyLib;
using UnityEngine;

namespace ReBeat.HarmonyPatches.BeamapData {
    [HarmonyPatch(typeof(BeatmapLevelData))]
    class AudioLength {
        internal static float Length { get; private set; }
        
        [HarmonyPostfix]
        [HarmonyPatch(nameof(BeatmapLevelData.audioClip), MethodType.Getter)]
        static void SetLength(AudioClip __result) {
            Length = __result.length;
        }
    }
}