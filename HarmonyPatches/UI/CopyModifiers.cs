using HarmonyLib;

namespace ReBeat.HarmonyPatches.UI {
    [HarmonyPatch(typeof(GameplayModifiersPanelController))]
    class CopyModifiers {
        [HarmonyPatch(nameof(GameplayModifiersPanelController.Awake))]
        static void Postfix(GameplayModifiersPanelController __instance) {
            __instance.didChangeGameplayModifiersEvent += () => {
                if (!Config.Instance.Enabled) Config.modifiers = __instance.gameplayModifiers.CopyWith();
            };
        }
    }
}