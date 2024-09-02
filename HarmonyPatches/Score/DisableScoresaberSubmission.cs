using System.Reflection;
using HarmonyLib;

namespace ReBeat.HarmonyPatches.Score {
    [HarmonyPatch]
    class DisableScoresaberSubmission {
        static MethodBase TargetMethod() {
            return AccessTools.TypeByName("ScoreSaber.Core.Daemons.UploadDaemon").GetMethod("Five");
        }

        static bool Prefix() {
            return !Config.Instance.Enabled;
        }
    }
}