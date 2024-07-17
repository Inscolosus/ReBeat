using HarmonyLib;

namespace ReBeat.HarmonyPatches.BeamapData {
    [HarmonyPatch(typeof(BeatmapDataLoader))]
    class NoteCount {
        internal static int Count { get; private set; }
        
        [HarmonyPostfix]
        [HarmonyPatch(nameof(BeatmapDataLoader.GetBeatmapDataFromSaveData))]
        static void SetNoteCount(BeatmapData __result) {
            Count = __result.cuttableNotesCount;
        }
    }
}