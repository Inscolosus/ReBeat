using HarmonyLib;
using ReBeat.HarmonyPatches.UI;
using UnityEngine;

namespace ReBeat.HarmonyPatches.Gameplay.ModifierPatches {
    // ty kinsi
    [HarmonyPatch(typeof(DisappearingArrowControllerBase<GameNoteController>))]
    class Hidden {
        static readonly IPA.Utilities.FieldAccessor<DisappearingArrowController, GameNoteController>.Accessor ArrowControllerController =
            IPA.Utilities.FieldAccessor<DisappearingArrowController, GameNoteController>.GetAccessor("_gameNoteController");
        static readonly IPA.Utilities.FieldAccessor<CutoutAnimateEffect, CutoutEffect[]>.Accessor CutoutController =
            IPA.Utilities.FieldAccessor<CutoutAnimateEffect, CutoutEffect[]>.GetAccessor("_cuttoutEffects");

        [HarmonyPostfix]
        [HarmonyPatch("HandleNoteMovementNoteDidMoveInJumpPhase")]
        static void FadeMesh(DisappearingArrowControllerBase<GameNoteController> __instance) {
            if (!Config.Instance.Enabled || Modifiers.instance.GhostNotes || Modifiers.instance.DisappearingArrows) return;
            if (!(__instance is DisappearingArrowController dac)) return;
            
            float dist = ArrowControllerController(ref dac).noteMovement.distanceToPlayer;
            if (dist < Config.FadeEndDistance) return;

            var cutoutAnimateEffect = __instance.gameObject.GetComponent<CutoutAnimateEffect>();
            if (cutoutAnimateEffect is null) return;

            var cutoutEffects = CutoutController(ref cutoutAnimateEffect);

            foreach (var cutoutEffect in cutoutEffects) {
                if (!cutoutEffect.name.Equals("NoteCube")) continue;

                float val = Mathf.Clamp01((dist - Config.FadeEndDistance) / Config.FadeDurationDistance);
                val = val < 0.25 ? 0 : val; // the notes don't fully disappear without this
                cutoutEffect.SetCutout(1f - val);

                break;
            }
        }

        // TODO: Fix arrow disappearing when changing hidden values
        /*
        [HarmonyPrefix]
        [HarmonyPatch("SetArrowTransparency")]
        static void FadeArrow(ref float arrowTransparency, DisappearingArrowControllerBase<GameNoteController> __instance) {
            if (!Config.Instance.Enabled || Modifiers.instance.GhostNotes || Modifiers.instance.DisappearingArrows) return;
            if (!(__instance is DisappearingArrowController dac)) return;
            
            float dist = ArrowControllerController(ref dac).noteMovement.distanceToPlayer;
            if (dist < Config.Instance.FadeEndDistance) return;
            float val = Mathf.Clamp01((dist - Config.Instance.FadeEndDistance) / Config.Instance.FadeDurationDistance);
            arrowTransparency = 1 - val;
        }*/
    }
}