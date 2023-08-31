using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaber5.HarmonyPatches {
    [HarmonyPatch(typeof(ScoreMultiplierCounter), nameof(ScoreMultiplierCounter.ProcessMultiplierEvent))]
    static class ComboPatch {
        // TODO: fix multiplier wrapping
        static bool Prefix(ref bool __result, ScoreMultiplierCounter.MultiplierEventType multiplierEventType, ref int ____multiplier, ref int ____multiplierIncreaseProgress, ref int ____multiplierIncreaseMaxProgress) {
            if (!Config.Instance.Enabled) return true;
            __result = false;

            int multiplierMax = 1073741824; //2^30
            int threshold = ____multiplierIncreaseMaxProgress == 2 ? 1 : (____multiplierIncreaseMaxProgress) / 4 * 3;

            if (multiplierEventType == ScoreMultiplierCounter.MultiplierEventType.Positive) {
                if (____multiplier < multiplierMax) {
                    if (____multiplierIncreaseProgress < ____multiplierIncreaseMaxProgress) {
                        ____multiplierIncreaseProgress++;
                        __result = true;
                    }
                    if (____multiplierIncreaseProgress >= ____multiplierIncreaseMaxProgress) {
                        ____multiplier *= 2;
                        ____multiplierIncreaseProgress = 0;
                        ____multiplierIncreaseMaxProgress = ____multiplier * 2;
                        __result = true;
                    }
                }
            }
            else if (multiplierEventType == ScoreMultiplierCounter.MultiplierEventType.Negative) {
                if (____multiplierIncreaseProgress > threshold) {
                    ____multiplierIncreaseProgress = 0;
                    __result = true;
                }
                else if (____multiplier > 1) {
                    ____multiplier /= 2;
                    ____multiplierIncreaseMaxProgress = ____multiplier * 2;
                    ____multiplierIncreaseProgress = ____multiplier; // maxProgress / 2
                    __result = true;
                }
            }

            return false;
        }
    }



    [HarmonyPatch(typeof(ScoreMultiplierUIController), nameof(ScoreMultiplierUIController.Update))]
    static class ThresholdColorPatch {
        static void Postfix(ref Image ____multiplierProgressImage) {
            ____multiplierProgressImage.color = ____multiplierProgressImage.fillAmount >= 0.75f ? Color.cyan : Color.white;
        }
    }

    [HarmonyPatch(typeof(ScoreMultiplierUIController), nameof(ScoreMultiplierUIController.HandleMultiplierDidChange))]
    static class DebugMultiplierTexts {
        static void Postfix(ref TextMeshProUGUI[] ____multiplierTexts) {
            foreach (var thang in ____multiplierTexts) {
                Plugin.Log.Debug(thang.text);
            }
            Plugin.Log.Debug("(end)");
        }
    }
}
