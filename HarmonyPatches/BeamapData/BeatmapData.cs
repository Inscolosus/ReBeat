using System.Threading.Tasks;
using HarmonyLib;

namespace ReBeat.HarmonyPatches.BeamapData {
    [HarmonyPatch(typeof(BeatmapDataLoader))]
    class BeatmapData {
        internal static float SongLength { get; set; }
        internal static int NoteCount { get; private set; }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(BeatmapDataLoader.LoadBeatmapDataAsync))]
        static async void GetNoteCount(Task<IReadonlyBeatmapData> __result) {
            var data = await __result;
            NoteCount = data.cuttableNotesCount;
        }
    }
}