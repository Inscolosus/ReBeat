using System;
using System.Collections.Generic;
using HarmonyLib;
using IPA.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaber5.HarmonyPatches {
    [HarmonyPatch(typeof(GameEnergyCounter), nameof(GameEnergyCounter.Start))]
    static class BatteryLivesPatch {
        public const int BatteryLives = 9;
        static void Postfix(ref int ____batteryLives) {
            if (!Config.Instance.Enabled) return;
            ____batteryLives = BatteryLives;
            EnergyPatch.ShieldProgress = 0;
            EnergyPatch.Shield = EnergyPatch.MaxShield;
        }
    }



    [HarmonyPatch(typeof(GameEnergyCounter.InitData), MethodType.Constructor, new[] { typeof(GameplayModifiers.EnergyType), typeof(bool), typeof(bool), typeof(bool) })]
    static class ForceBatteryEnergyPatch {
        static void Postfix(ref GameplayModifiers.EnergyType ___energyType) {
            if (!Config.Instance.Enabled) return;
            ___energyType = GameplayModifiers.EnergyType.Battery;
        }
    }



    [HarmonyPatch(typeof(GameEnergyCounter), nameof(GameEnergyCounter.ProcessEnergyChange))]
    static class EnergyPatch {
        internal const int MaxShield = 4;
        internal static int Shield;
        internal static int ShieldProgress;
        internal static DateTime LastMiss;
        internal static readonly float OneEnergySegment = 1f / BatteryLivesPatch.BatteryLives;

        internal static PropertyAccessor<GameEnergyCounter, float>.Setter EnergySetter =
            PropertyAccessor<GameEnergyCounter, float>.GetSetter("energy");

        static void Prefix(float energyChange, GameEnergyCounter __instance) {
            if (!Config.Instance.Enabled) return;
            if (energyChange <= 0) {
                LastMiss = DateTime.Now;
                ShieldProgress = 0;

                if (Shield > 0) Shield--;


                if (Config.Instance.StrictEnergy) {
                    int energyToSubtract = (int)(-energyChange / 0.15f) - 1; // game will already subtract 1 segment
                    for (int i = 0; i < energyToSubtract; i++) {
                        if (Shield < 1) break;
                        Shield--;
                    }

                    if (energyToSubtract < 1) return;
                    EnergySetter(ref __instance, __instance.energy - energyToSubtract * OneEnergySegment);
                }
            }
            else {
                // there's certainly a better way to do this
                if (Config.Instance.EnableShieldCooldown && (DateTime.Now - LastMiss).TotalSeconds < Config.Instance.ShieldCooldown) return;

                ShieldProgress += ShieldProgress < (int)Config.Instance.ShieldRegen ? 1 : 0;
                if (ShieldProgress >= (int)Config.Instance.ShieldRegen && Shield < MaxShield) {
                    Shield++;
                    ShieldProgress = 0;

                    if (__instance.energy + OneEnergySegment > 1f) EnergySetter(ref __instance, 1f);
                    else EnergySetter(ref __instance, __instance.energy + OneEnergySegment);
                }
            }
        }
    }



    [HarmonyPatch(typeof(GameEnergyCounter), nameof(GameEnergyCounter.HandleNoteWasCut))]
    static class BadCutEnergyPatch {
        static bool Prefix(NoteController noteController, NoteCutInfo noteCutInfo, ref float ____nextFrameEnergyChange) {
            if (!Config.Instance.Enabled || !Config.Instance.StrictEnergy) return true;

            if (noteController.noteData.gameplayType == NoteData.GameplayType.Normal || noteController.noteData.gameplayType == NoteData.GameplayType.BurstSliderHead) {
                if (noteCutInfo.allIsOK) return true;
                ____nextFrameEnergyChange -= 0.15f;
                return false;
            }

            return true;
        }
    }



    [HarmonyPatch(typeof(GameEnergyUIPanel), nameof(GameEnergyUIPanel.RefreshEnergyUI))]
    static class EnergyUIPatch {
        static void Postfix(ref List<Image> ____batteryLifeSegments, ref IGameEnergyCounter ____gameEnergyCounter) {
            if (!Config.Instance.Enabled) return;

            int health = ____gameEnergyCounter.batteryEnergy - EnergyPatch.Shield;
            if (health > 5) health = 5; // bruh
            Color healthColor = health > 3 ? Color.green :
                health > 1 ? Color.yellow :
                Color.red;

            Color shieldColor = EnergyPatch.Shield < EnergyPatch.MaxShield ? Color.blue : Color.cyan;

            for (int i = 0; i < health; i++) {
                ____batteryLifeSegments[i].color = healthColor;
            }
            for (int i = health; i < ____gameEnergyCounter.batteryEnergy; i++) {
                ____batteryLifeSegments[i].color = shieldColor;
            }
        }
    }
}
