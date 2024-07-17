using HarmonyLib;

namespace ReBeat.HarmonyPatches.Gameplay {
    [HarmonyPatch(typeof(GameNoteController))]
    class DisableWrongColorBadCut {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(GameNoteController.HandleCut))]
        static bool IgnoreWrongSaberType(Saber saber, GameNoteController __instance) {
            if (!Config.Instance.Enabled) return true;
            if (saber.saberType.MatchesColorType(__instance.noteData.colorType)) return true;
            return false;
        }
    }
}