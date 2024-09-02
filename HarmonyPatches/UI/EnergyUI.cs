using System;
using System.Collections.Generic;
using HarmonyLib;
using ReBeat.HarmonyPatches.Energy;
using UnityEngine;
using UnityEngine.UI;

namespace ReBeat.HarmonyPatches.UI {
    [HarmonyPatch(typeof(GameEnergyUIPanel))]
    public class EnergyUI {
        [HarmonyPostfix]
        [HarmonyPatch("RefreshEnergyUI")]
        static void RefreshEnergyUI(ref List<Image> ____batteryLifeSegments, ref IGameEnergyCounter ____gameEnergyCounter, ref Image ____energyBar, ref RectTransform ____energyBarRectTransform) {
            if (!Config.Instance.Enabled) return;
            // health bar
            if (EnergyController.EnergyCounter.Health < 1) {
                foreach (var image in ____batteryLifeSegments) {
                    image.enabled = false;
                    ____energyBar.gameObject.SetActive(false);
                }
            }

            Color healthColor;
            if (EnergyController.EnergyCounter.MaxHealth == 1) healthColor = Config.Instance.HealthColor;
            else healthColor = EnergyController.EnergyCounter.Health > 3 ? Config.Instance.HealthColor :
                EnergyController.EnergyCounter.Health > 1 ? Config.Instance.LowHealthColor :
                Config.Instance.MinHealthColor;

            Color shieldColor = EnergyController.EnergyCounter.Shield < EnergyController.EnergyCounter.MaxShield
                ? Config.Instance.LowShieldColor
                : Config.Instance.ShieldColor;

            for (int i = 0; i < ____batteryLifeSegments.Count; i++) {
                if (i < EnergyController.EnergyCounter.Health) {
                    ____batteryLifeSegments[i].enabled = true;
                    ____batteryLifeSegments[i].color = healthColor;
                }
                else if (i < EnergyController.EnergyCounter.Health + EnergyController.EnergyCounter.Shield) {
                    ____batteryLifeSegments[i].enabled = true;
                    ____batteryLifeSegments[i].color = shieldColor;
                }
                else {
                    ____batteryLifeSegments[i].enabled = false;
                }
            }


             //recharge bar
            ____energyBar.gameObject.SetActive(EnergyController.EnergyCounter.Shield < EnergyController.EnergyCounter.MaxShield);
            ____energyBarRectTransform.anchorMax = new Vector2((float)EnergyController.EnergyCounter.ShieldProgress / (EnergyController.EnergyCounter.ShieldRegen-1), 1f);
        }

        private static float _prevEnergyBarY = 0;
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        static void MoveNormalEnergyBar(ref Image ____energyBar) {
            if (!Config.Instance.Enabled) return;
            if (Math.Abs(Math.Abs(____energyBar.transform.position.y - _prevEnergyBarY) - 0.22) < 0.01) return;
            _prevEnergyBarY = ____energyBar.transform.position.y;
            var vector3 = ____energyBar.transform.position;
            vector3.y -= 0.22f;
            ____energyBar.transform.position = vector3;
        }
    }
}