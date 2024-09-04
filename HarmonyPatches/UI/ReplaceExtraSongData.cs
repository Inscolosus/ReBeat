using System.Linq;
using HarmonyLib;
using SongCore.Data;

namespace ReBeat.HarmonyPatches.UI {
    [HarmonyPatch(typeof(SongCore.Collections))]
    class ReplaceExtraSongData {
        [HarmonyPatch(nameof(SongCore.Collections.RetrieveExtraSongData))]
        static void Postfix(ref ExtraSongData __result) {
            if (__result is null) return;
            if (Config.Instance.Enabled) {
                foreach (var diffData in __result._difficulties) {
                    if (SongCore.Collections.customCharacteristics.All(x => x.serializedName != $"ReBeat_{diffData._beatmapCharacteristicName}")) continue;
                    diffData._beatmapCharacteristicName = $"ReBeat_{diffData._beatmapCharacteristicName}";
                }
            }
            else {
                foreach (var diffData in __result._difficulties) {
                    if (!diffData._beatmapCharacteristicName.StartsWith("ReBeat_")) continue;
                    diffData._beatmapCharacteristicName = diffData._beatmapCharacteristicName.Substring(7);
                }
            }
        }
    }
}