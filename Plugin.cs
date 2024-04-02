using System.Reflection;
using BeatSaberMarkupLanguage.GameplaySetup;
using BeatSaberMarkupLanguage.Settings;
using HarmonyLib;
using IPA;
using IPA.Config.Stores;
using IPALogger = IPA.Logging.Logger;

namespace BeatSaber5 {
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }
        internal static Harmony Harmony { get; private set; }


        [Init]
        public Plugin(IPALogger logger, IPA.Config.Config config) {
            Instance = this;
            Log = logger;
            Harmony = new Harmony("Inscolosus.BeatSaber.BeatSaber5");
            Config.Instance = config.Generated<Config>();
            
        }

        [OnStart]
        public void OnEnable() {
            Harmony.PatchAll(Assembly.GetExecutingAssembly());
            BSMLSettings.instance.AddSettingsMenu("BeatSaber5", "BeatSaber5.Views.Menu.bsml", Config.Instance);
            GameplaySetup.instance.AddTab("BeatSaber5", "BeatSaber5.Views.Menu.bsml", Config.Instance, MenuType.All);
        }

        [OnExit]
        public void OnDisable() {
            Harmony.UnpatchSelf();
        }
    }
}
