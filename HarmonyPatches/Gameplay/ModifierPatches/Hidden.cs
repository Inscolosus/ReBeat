using HarmonyLib;
using ReBeat.HarmonyPatches.UI;
using UnityEngine;

namespace ReBeat.HarmonyPatches.Gameplay.ModifierPatches {
    [HarmonyPatch(typeof(DisappearingArrowControllerBase<GameNoteController>))]
    class Hidden {
        static readonly IPA.Utilities.FieldAccessor<DisappearingArrowController, GameNoteController>.Accessor ArrowControllerController =
            IPA.Utilities.FieldAccessor<DisappearingArrowController, GameNoteController>.GetAccessor("_gameNoteController");
        static readonly IPA.Utilities.FieldAccessor<CutoutAnimateEffect, CutoutEffect[]>.Accessor CutoutController =
            IPA.Utilities.FieldAccessor<CutoutAnimateEffect, CutoutEffect[]>.GetAccessor("_cuttoutEffects");

        private const float FadeEndDistance = 2;
        private const float FadeDistanceDuration = 7;

        [HarmonyPostfix]
        [HarmonyPatch("HandleNoteMovementNoteDidMoveInJumpPhase")]
        static void FadeMesh(DisappearingArrowControllerBase<GameNoteController> __instance) {
            if (!Config.Instance.Enabled || Modifiers.instance.GhostNotes || Modifiers.instance.DisappearingArrows) return;
            if (!(__instance is DisappearingArrowController dac)) return;
            
            float dist = ArrowControllerController(ref dac).noteMovement.distanceToPlayer;

            if (dist < FadeEndDistance) return;

            var cutoutAnimateEffect = __instance.gameObject.GetComponent<CutoutAnimateEffect>();
            if (cutoutAnimateEffect is null) return;

            var cutoutEffects = CutoutController(ref cutoutAnimateEffect);

            foreach (var cutoutEffect in cutoutEffects) {
                if (!cutoutEffect.name.Equals("NoteCube")) continue;

                float val = Mathf.Clamp01((dist - FadeEndDistance) / FadeDistanceDuration);
                val = val < 0.25 ? 0 : val; // the notes don't fully disappear without this
                cutoutEffect.SetCutout(1f - val);

                break;
            }
        }
    }
}