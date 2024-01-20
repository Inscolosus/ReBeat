using HarmonyLib;

namespace BeatSaber5.HarmonyPatches.Gameplay {
    [HarmonyPatch(typeof(GameNoteController))]
    class DisableWrongColorBadCut {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(GameNoteController.HandleCut))]
        static bool IgnoreWrongSaberType(Saber saber, GameNoteController __instance) {
            if (saber.saberType.MatchesColorType(__instance.noteData.colorType)) return true;
            return false;
        }
    }
}