using HarmonyLib;

namespace ReBeat.HarmonyPatches.Energy {
    [HarmonyPatch(typeof(GameEnergyCounter.InitData))]
    class ForceBatteryEnergy {
        [HarmonyPostfix]
        [HarmonyPatch(MethodType.Constructor, typeof(GameplayModifiers.EnergyType), typeof(bool), typeof(bool),
            typeof(bool))]
        static void SetEnergyType(ref GameplayModifiers.EnergyType ___energyType) {
            if (!Config.Instance.Enabled) return;
            ___energyType = GameplayModifiers.EnergyType.Battery;
        }
    }
}