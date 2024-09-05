using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;

namespace ReBeat.HarmonyPatches.UI {
    [HarmonyPatch]
    class HsvPatch {
        static bool Prepare() {
            bool hsvExists = AppDomain.CurrentDomain.GetAssemblies().Any(asm => asm.GetName().Name == "HitScoreVisualizer");
            if (!hsvExists) Plugin.Log.Info("HSV not found, ignoring");
            return hsvExists;
        }

        static MethodBase TargetMethod() {
            var judgmentService = AccessTools.TypeByName("HitScoreVisualizer.Services.JudgmentService");
            return AccessTools.Constructor(judgmentService,
                       new[] { AccessTools.TypeByName("HitScoreVisualizer.Services.ConfigProvider") }) ??
                   AccessTools.Constructor(judgmentService,
                       new[] {
                           AccessTools.TypeByName("HitScoreVisualizer.Services.ConfigProvider"),
                           AccessTools.TypeByName("SiraUtil.Logging.SiraLog")
                       });
        }

        [HarmonyPostfix]
        // have to use object for everything to not depend on hsv
        //                          JudgmentService, ConfigProvider
        static async void SetHsvConfig(object __instance, object configProvider) {
            if (!Config.Instance.Enabled) return;
            
            var path = (string)configProvider.GetType()
                .GetField("_hsvConfigsFolderPath", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(configProvider);
            string defaultName = "HitScoreVisualizerConfig_100max";
            string[] possibleNames = { Config.Instance.HsvConfig, defaultName, $"{defaultName}_1", $"{defaultName} (1)" };

            // Configuration
            object config = null;
            foreach (var name in possibleNames) {
                string filename = name.EndsWith(".json") ? name : $"{name}.json";
                if (!File.Exists($"{path}/{filename}")) continue;
                
                // call LoadConfig(filename)
                var loadConfig = configProvider.GetType()
                    .GetMethod("LoadConfig", BindingFlags.Instance | BindingFlags.NonPublic);
                var loadConfigTask = (Task)loadConfig.Invoke(configProvider, new object[] { filename });
                
                // get result
                await loadConfigTask.ConfigureAwait(false);
                config = loadConfigTask.GetType().GetProperty("Result").GetValue(loadConfigTask);
                break;
            }

            if (config is null) {
                Plugin.Log.Notice("No hsv config found");
                return;
            }
            __instance.GetType().GetField("_config", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(__instance, config);
        }
    }
}