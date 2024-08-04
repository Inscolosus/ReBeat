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
            if (EnergyController.EnergyCounter.MaxHealth == 1) healthColor = Color.green;
            else healthColor = EnergyController.EnergyCounter.Health > 3 ? Color.green :
                EnergyController.EnergyCounter.Health > 1 ? Color.yellow :
                Color.red;

            // 0 145 255
            Color bruhColor = new Color(Config.Instance.ColorRed/255f, Config.Instance.ColorGreen/255f, Config.Instance.ColorBlue/255f);
            Color shieldColor = EnergyController.EnergyCounter.Shield < EnergyController.EnergyCounter.MaxShield ? bruhColor : Color.cyan;

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
        
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        static void MoveNormalEnergyBar(ref Image ____energyBar) {
            if (!Config.Instance.Enabled) return;
            ____energyBar.gameObject.transform.position = new Vector3(-0.9539997f, -0.86f, 7.75f);
        }
    }
}