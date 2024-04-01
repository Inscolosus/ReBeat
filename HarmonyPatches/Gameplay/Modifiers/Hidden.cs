using HarmonyLib;
using UnityEngine;

namespace BeatSaber5.HarmonyPatches.Gameplay.Modifiers {
    [HarmonyPatch(typeof(DisappearingArrowControllerBase<GameNoteController>))]
    class Hidden {
        static readonly IPA.Utilities.FieldAccessor<DisappearingArrowController, GameNoteController>.Accessor ArrowControllerController =
            IPA.Utilities.FieldAccessor<DisappearingArrowController, GameNoteController>.GetAccessor("_gameNoteController");
        static readonly IPA.Utilities.FieldAccessor<CutoutAnimateEffect, CutoutEffect[]>.Accessor CutoutController =
            IPA.Utilities.FieldAccessor<CutoutAnimateEffect, CutoutEffect[]>.GetAccessor("_cuttoutEffects");

        [HarmonyPostfix]
        [HarmonyPatch("HandleNoteMovementNoteDidMoveInJumpPhase")]
        static void FadeMesh(DisappearingArrowControllerBase<GameNoteController> __instance) {
            if (!Config.Instance.Enabled) return;
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