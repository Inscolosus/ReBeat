using HarmonyLib;
using UnityEngine;

namespace BeatSaber5.HarmonyPatches.Gameplay.Modifiers {
    [HarmonyPatch(typeof(DisappearingArrowControllerBase<GameNoteController>))]
    class EnableMeshOnGn {
        [HarmonyPostfix]
        [HarmonyPatch("HandleCubeNoteControllerDidInit")]
        static void EnableMesh(MeshRenderer ____cubeMeshRenderer) {
            ____cubeMeshRenderer.enabled = true;
        }
    }
}