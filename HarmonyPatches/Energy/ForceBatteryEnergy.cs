using HarmonyLib;

namespace BeatSaber5.HarmonyPatches.Energy {
    [HarmonyPatch(typeof(GameEnergyCounter.InitData))]
    class ForceBatteryEnergy {
        [HarmonyPostfix]
        [HarmonyPatch(MethodType.Constructor, typeof(GameplayModifiers.EnergyType), typeof(bool), typeof(bool),
            typeof(bool))]
        static void SetEnergyType(ref GameplayModifiers.EnergyType ___energyType) {
            ___energyType = GameplayModifiers.EnergyType.Battery;
        }
    }
}