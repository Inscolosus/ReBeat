using System;
using System.Collections.Generic;
using HarmonyLib;
using IPA.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaber5.HarmonyPatches {
    [HarmonyPatch(typeof(GameEnergyCounter), nameof(GameEnergyCounter.Start))]
    static class BatteryLivesPatch {
        public static int BatteryLives;

        static void Prefix(ref int ____batteryLives) {
            ____batteryLives = (int)Config.StartingHealth + EnergyPatch.MaxShield; // was too lazy to actually change this for now lohl
            BatteryLives = ____batteryLives;

            //reset energy values
            EnergyPatch.Health = (int)Config.StartingHealth; // same here
            EnergyPatch.ShieldProgress = 0;
            EnergyPatch.Shield = EnergyPatch.MaxShield;
            EnergyPatch.Misses = 0;
            EnergyPatch.TotalMisses = 0;
            EnergyPatch.Combo = 0;
            EnergyPatch.HighestCombo = 0;
        }
    }


    [HarmonyPatch(typeof(GameEnergyCounter.InitData), MethodType.Constructor,
        new[] { typeof(GameplayModifiers.EnergyType), typeof(bool), typeof(bool), typeof(bool) })]
    static class ForceBatteryEnergyPatch {
        static void Postfix(ref GameplayModifiers.EnergyType ___energyType) {
            ___energyType = GameplayModifiers.EnergyType.Battery;
        }
    }



    [HarmonyPatch(typeof(GameEnergyCounter), nameof(GameEnergyCounter.ProcessEnergyChange))]
    static class EnergyPatch {
        internal const int MaxShield = 4;
        internal static int Health; 
        internal static int Shield;
        internal static int ShieldProgress;
        internal static DateTime LastMiss;

        internal static int Misses;
        internal static int TotalMisses;

        internal static int Combo;
        internal static int HighestCombo;

        internal static PropertyAccessor<GameEnergyCounter, float>.Setter EnergySetter =
            PropertyAccessor<GameEnergyCounter, float>.GetSetter("energy");

        static void Prefix(GameEnergyCounter __instance, float energyChange) {
            energyChange = 0f;
            EnergySetter.Invoke(ref __instance, 1f);

            if (Misses == 0) return;

            if (Shield > 0 && Shield >= Misses) {
                Shield -= Misses;
                Misses = 0;
            }
            else if (Shield > 0) {
                Misses -= Shield;
                Shield = 0;
            }

            Health -= Misses;
            Misses = 0;


            if (Health < 1) {
                energyChange = -1f;
                EnergySetter.Invoke(ref __instance, 0.1f);
            }
        }

        internal static void NoteWasHit() {
            if (!((DateTime.Now - LastMiss).TotalSeconds > Plugin.ShieldCooldown)) return;
            if (ShieldProgress < Plugin.ShieldRegen) {
                ShieldProgress++;
            }

            if (ShieldProgress >= Plugin.ShieldRegen && Shield < MaxShield) {
                Shield++;
                ShieldProgress = 0;
            }

            Combo++;
            if (Combo > HighestCombo) HighestCombo = Combo;
        }

        internal static void NoteWasMissed() {
            LastMiss = DateTime.Now;
            ShieldProgress = 0;
            Misses++;

            // other shit
            TotalMisses++;
            Combo = 0;
        }
    }



    [HarmonyPatch(typeof(GameEnergyCounter), nameof(GameEnergyCounter.HandleNoteWasCut))]
    static class NoteWasCutPatch {
        static void Prefix(NoteController noteController, NoteCutInfo noteCutInfo) {
            switch (noteController.noteData.gameplayType) {
                case NoteData.GameplayType.Normal:
                case NoteData.GameplayType.BurstSliderHead:
                case NoteData.GameplayType.BurstSliderElement:
                    if (noteCutInfo.allIsOK) {
                        EnergyPatch.NoteWasHit();
                    }
                    else EnergyPatch.NoteWasMissed();
                    break;

                case NoteData.GameplayType.Bomb:
                    EnergyPatch.NoteWasMissed();
                    break;
            }



        }
    }

    [HarmonyPatch(typeof(GameEnergyCounter), nameof(GameEnergyCounter.HandleNoteWasMissed))]
    static class NoteWasMissedPatch {
        static void Prefix(NoteController noteController) {
            if (noteController.noteData.gameplayType != NoteData.GameplayType.Bomb) 
                EnergyPatch.NoteWasMissed();
        }
    }

    [HarmonyPatch(typeof(GameEnergyUIPanel), nameof(GameEnergyUIPanel.RefreshEnergyUI))]
    static class EnergyUIPatch {

        static void Postfix(ref List<Image> ____batteryLifeSegments, ref IGameEnergyCounter ____gameEnergyCounter, ref Image ____energyBar, ref RectTransform ____energyBarRectTransform) {
            // health bar
            if (EnergyPatch.Health < 1) {
                foreach (var image in ____batteryLifeSegments) {
                    image.enabled = false;
                    ____energyBar.gameObject.SetActive(false);
                }
            }

            Color healthColor = EnergyPatch.Health > 3 ? Color.green :
                EnergyPatch.Health > 1 ? Color.yellow :
                Color.red;

            // 0 145 255
            Color bruhColor = new Color(Config.Instance.ColorRed/255f, Config.Instance.ColorGreen/255f, Config.Instance.ColorBlue/255f);
            Color shieldColor = EnergyPatch.Shield < EnergyPatch.MaxShield ? bruhColor : Color.cyan;

            for (int i = 0; i < ____batteryLifeSegments.Count; i++) {
                if (i < EnergyPatch.Health) {
                    ____batteryLifeSegments[i].enabled = true;
                    ____batteryLifeSegments[i].color = healthColor;
                }
                else if (i < EnergyPatch.Health + EnergyPatch.Shield) {
                    ____batteryLifeSegments[i].enabled = true;
                    ____batteryLifeSegments[i].color = shieldColor;
                }
                else {
                    ____batteryLifeSegments[i].enabled = false;
                }
            }


             //recharge bar
            ____energyBar.gameObject.SetActive(EnergyPatch.Shield < EnergyPatch.MaxShield);
            ____energyBarRectTransform.anchorMax = new Vector2((float)EnergyPatch.ShieldProgress / (Plugin.ShieldRegen-1), 1f);
        }
    }

    [HarmonyPatch(typeof(GameEnergyUIPanel), nameof(GameEnergyUIPanel.Start))]
    static class EnergyBarPatch {
        static void Postfix(ref Image ____energyBar) {
            ____energyBar.gameObject.transform.position = new Vector3(-0.9539997f, -0.86f, 7.75f);
        }
    }
}
