using System;
using HarmonyLib;
using IPA.Utilities;
using ReBeat.HarmonyPatches.UI;
using UnityEngine;

namespace ReBeat.HarmonyPatches.Energy {
    [HarmonyPatch(typeof(GameEnergyCounter))]
    class EnergyController {
        internal static EnergyCounter EnergyCounter { get; private set; }

        private static PropertyAccessor<GameEnergyCounter, float>.Setter EnergySetter =
            PropertyAccessor<GameEnergyCounter, float>.GetSetter("energy");
        
        [HarmonyPrefix]
        [HarmonyPatch("Start")]
        static void EnergyCounterStart(ref int ____batteryLives) {
            if (!Config.Instance.Enabled) return;
            EnergyCounter = Modifiers.instance.OneLife ? new EnergyCounter(1, 0) :
                Modifiers.instance.OneHp ? new EnergyCounter(1, 4) :
                new EnergyCounter(5, 4);
            
            ____batteryLives = EnergyCounter.Health + EnergyCounter.MaxShield;
        }

        [HarmonyPrefix]
        [HarmonyPatch("LateUpdate")]
        static void HandleWall(ref PlayerHeadAndObstacleInteraction ____playerHeadAndObstacleInteraction) {
            if (!Config.Instance.Enabled) return;
            if (____playerHeadAndObstacleInteraction.playerHeadIsInObstacle) {
                if (EnergyCounter.WasInWallLastFrame) {
                    EnergyCounter.TimeToNextWallDamage -= Time.deltaTime;
                    if (EnergyCounter.TimeToNextWallDamage > 0) return;
                }

                EnergyCounter.WasInWallLastFrame = true;
                EnergyCounter.TimeToNextWallDamage = 0.5f;
                EnergyCounter.Misses++;
            }
            else {
                EnergyCounter.WasInWallLastFrame = false;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch("ProcessEnergyChange")]
        static void ProcessEnergyChange(GameEnergyCounter __instance, ref float energyChange) {
            if (!Config.Instance.Enabled) return;
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
        [HarmonyPatch("HandleNoteWasCut")]
        static void NoteWasCut(NoteController noteController, NoteCutInfo noteCutInfo) {
            if (!Config.Instance.Enabled) return;
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
        [HarmonyPatch("HandleNoteWasMissed")]
        static void NoteWasMissed(NoteController noteController) {
            if (!Config.Instance.Enabled) return;
            if (noteController.noteData.gameplayType != NoteData.GameplayType.Bomb) 
                HandleMiss();
        }
        
        private static void HandleCut() {
            EnergyCounter.Combo++;
            if (EnergyCounter.Combo > EnergyCounter.MaxCombo) EnergyCounter.MaxCombo = EnergyCounter.Combo;
            
            if (EnergyCounter.ShieldProgress >= EnergyCounter.ShieldRegen) return;
            if (Time.time - EnergyCounter.LastMiss < EnergyCounter.ShieldCooldown) return;
            
            EnergyCounter.ShieldProgress++;

            if (EnergyCounter.ShieldProgress < EnergyCounter.ShieldRegen ||
                EnergyCounter.Shield >= EnergyCounter.MaxShield) return;
            EnergyCounter.Shield++;
            EnergyCounter.ShieldProgress = 0;
        }

        private static void HandleMiss() {
            EnergyCounter.LastMiss = Time.time;
            EnergyCounter.ShieldProgress = 0;
            EnergyCounter.Misses++;

            EnergyCounter.TotalMisses++;
            EnergyCounter.Combo = 0;
        }
    }
}