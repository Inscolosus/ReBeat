using System;
using HarmonyLib;
using IPA.Utilities;

namespace BeatSaber5.HarmonyPatches.Energy {
    [HarmonyPatch(typeof(GameEnergyCounter))]
    class EnergyController {
        internal static EnergyCounter EnergyCounter { get; private set; }

        private static PropertyAccessor<GameEnergyCounter, float>.Setter EnergySetter =
            PropertyAccessor<GameEnergyCounter, float>.GetSetter("energy");
        
        [HarmonyPrefix]
        [HarmonyPatch(nameof(GameEnergyCounter.Start))]
        static void EnergyCounterStart(ref int ____batteryLives, ref GameEnergyCounter.InitData ____initData) {
            EnergyCounter = ____initData.instaFail ? new EnergyCounter(1, 0) :
                ForceBatteryEnergy.OneHP ? new EnergyCounter(1, 4) :
                new EnergyCounter(5, 4);
            
            ____batteryLives = EnergyCounter.Health + EnergyCounter.MaxShield;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(GameEnergyCounter.ProcessEnergyChange))]
        static void ProcessEnergyChange(GameEnergyCounter __instance, ref float energyChange) {
            energyChange = 0f;
            EnergySetter.Invoke(ref __instance, 1f);

            if (EnergyCounter.Misses == 0) return;

            if (EnergyCounter.Shield > 0 && EnergyCounter.Shield >= EnergyCounter.Misses) {
                EnergyCounter.Shield -= EnergyCounter.Misses;
                EnergyCounter.Misses = 0;
            }
            else if (EnergyCounter.Shield > 0) {
                EnergyCounter.Misses -= EnergyCounter.Shield;
                EnergyCounter.Shield = 0;
            }

            EnergyCounter.Health -= EnergyCounter.Misses;
            EnergyCounter.Misses = 0;

            if (EnergyCounter.Health < 1) {
                energyChange = -1f;
                EnergySetter.Invoke(ref __instance, 0.1f);
            }
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(nameof(GameEnergyCounter.HandleNoteWasCut))]
        static void NoteWasCut(NoteController noteController, NoteCutInfo noteCutInfo) {
            switch (noteController.noteData.gameplayType) {
                case NoteData.GameplayType.Normal:
                case NoteData.GameplayType.BurstSliderHead:
                case NoteData.GameplayType.BurstSliderElement:
                    if (noteCutInfo.allIsOK) {
                        HandleCut();
                    }
                    else HandleMiss();
                    break;

                case NoteData.GameplayType.Bomb:
                    HandleMiss();
                    break;
            }
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(nameof(GameEnergyCounter.HandleNoteWasMissed))]
        static void NoteWasMissed(NoteController noteController) {
            if (noteController.noteData.gameplayType != NoteData.GameplayType.Bomb) 
                HandleMiss();
        }
        
        private static void HandleCut() {
            if ((DateTime.Now - EnergyCounter.LastMiss).TotalSeconds < EnergyCounter.ShieldCooldown) return;
            if (EnergyCounter.ShieldProgress < EnergyCounter.ShieldRegen) {
                EnergyCounter.ShieldProgress++;
            }

            if (EnergyCounter.ShieldProgress >= EnergyCounter.ShieldRegen && EnergyCounter.Shield < EnergyCounter.MaxShield) {
                EnergyCounter.Shield++;
                EnergyCounter.ShieldProgress = 0;
            }

            EnergyCounter.Combo++;
            if (EnergyCounter.Combo > EnergyCounter.MaxCombo) EnergyCounter.MaxCombo = EnergyCounter.Combo;
        }

        private static void HandleMiss() {
            EnergyCounter.LastMiss = DateTime.Now;
            EnergyCounter.ShieldProgress = 0;
            EnergyCounter.Misses++;

            EnergyCounter.TotalMisses++;
            EnergyCounter.Combo = 0;
        }
    }
}