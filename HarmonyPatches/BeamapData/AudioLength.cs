using HarmonyLib;
using UnityEngine;

namespace ReBeat.HarmonyPatches.BeamapData {
    [HarmonyPatch(typeof(BeatmapLevelDataSO))]
    class AudioLength {
        internal static float Length { get; set; }
        
        /*[HarmonyPostfix]
        [HarmonyPatch(nameof(BeatmapLevelDataSO.songAudioClip), MethodType.Getter)]
        static void SetLength(AudioClip __result) {
            Length = __result.length;
        }*/
    }
}