using HarmonyLib;
using ReBeat.HarmonyPatches.UI;
using UnityEngine;

namespace ReBeat.HarmonyPatches.Gameplay.ModifierPatches {
    // ty kinsi
    [HarmonyPatch(typeof(DisappearingArrowControllerBase<GameNoteController>))]
    class EnableMeshOnGn {
        [HarmonyPostfix]
        [HarmonyPatch("HandleCubeNoteControllerDidInit")]
        static void EnableMesh(MeshRenderer ____cubeMeshRenderer) {
            if (!Config.Instance.Enabled || Modifiers.instance.GhostNotes) return;
            ____cubeMeshRenderer.enabled = true;
        }
    }
}