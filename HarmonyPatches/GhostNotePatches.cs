using HarmonyLib;
using UnityEngine;

namespace BeatSaber5.HarmonyPatches {
    [HarmonyPatch(typeof(DisappearingArrowControllerBase<GameNoteController>), "HandleCubeNoteControllerDidInit")]
    static class GhostNoteMeshEnabler {
        static void Postfix(MeshRenderer ____cubeMeshRenderer) {
            ____cubeMeshRenderer.enabled = true;
        }
    }

    [HarmonyPatch(typeof(DisappearingArrowControllerBase<GameNoteController>), "HandleNoteMovementNoteDidMoveInJumpPhase")]
    static class GhostNoteMeshFader {
        static readonly IPA.Utilities.FieldAccessor<DisappearingArrowController, GameNoteController>.Accessor ArrowControllerController =
            IPA.Utilities.FieldAccessor<DisappearingArrowController, GameNoteController>.GetAccessor("_gameNoteController");
        static readonly IPA.Utilities.FieldAccessor<CutoutAnimateEffect, CutoutEffect[]>.Accessor CutoutController =
            IPA.Utilities.FieldAccessor<CutoutAnimateEffect, CutoutEffect[]>.GetAccessor("_cuttoutEffects");

        static void Postfix(DisappearingArrowControllerBase<GameNoteController> __instance) {
            if (!(__instance is DisappearingArrowController dac)) return;
            
            float dist = ArrowControllerController(ref dac).noteMovement.distanceToPlayer;

            if (dist < Config.Instance.FadeEndDistance) return;

            var cutoutAnimateEffect = __instance.gameObject.GetComponent<CutoutAnimateEffect>();
            if (cutoutAnimateEffect is null) return;

            var cutoutEffects = CutoutController(ref cutoutAnimateEffect);

            foreach (var cutoutEffect in cutoutEffects) {
                if (!cutoutEffect.name.Equals("NoteCube")) continue;

                float val = Mathf.Clamp01((dist - Config.Instance.FadeEndDistance) / Config.Instance.FadeDistanceDuration);
                val = val < Config.Instance.DebugSlider ? 0 : val;
                cutoutEffect.SetCutout(1f - val);

                break;
            }
        }
    }
}