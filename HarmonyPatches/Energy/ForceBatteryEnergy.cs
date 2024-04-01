using HarmonyLib;

namespace BeatSaber5.HarmonyPatches.Energy {
    [HarmonyPatch(typeof(GameEnergyCounter.InitData))]
    class ForceBatteryEnergy {
        internal static bool OneHP { get; private set; }
        
        [HarmonyPostfix]
        [HarmonyPatch(MethodType.Constructor, typeof(GameplayModifiers.EnergyType), typeof(bool), typeof(bool),
            typeof(bool))]
        static void SetEnergyType(ref GameplayModifiers.EnergyType ___energyType) {
            if (!Config.Instance.Enabled) return;
            OneHP = ___energyType == GameplayModifiers.EnergyType.Battery;
            ___energyType = GameplayModifiers.EnergyType.Battery;
        }
    }
}