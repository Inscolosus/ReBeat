using System.Reflection;
using BeatSaberMarkupLanguage.GameplaySetup;
using BeatSaberMarkupLanguage.Settings;
using HarmonyLib;
using IPA;
using IPA.Config.Stores;
using IPALogger = IPA.Logging.Logger;

namespace BeatSaber5 {
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }
        internal static Harmony harmony { get; private set; }

        internal static bool Submission { get; set; }

        [Init]
        public Plugin(IPALogger logger, IPA.Config.Config config) {
            Instance = this;
            Log = logger;
            Config.Instance = config.Generated<Config>();
            harmony = new Harmony("Inscolosus.BeatSaber.BeatSaber5");
        }

        [OnEnable]
        public void OnEnable() {
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            //BSMLSettings.instance.AddSettingsMenu("BeatSaber5", "BeatSaber5.Views.Menu.bsml", Config.Instance);
            GameplaySetup.instance.AddTab("BeatSaber5", "BeatSaber5.Views.Menu.bsml", Config.Instance, MenuType.All);
        }

        [OnDisable]
        public void OnDisable() {
            harmony.UnpatchSelf();
        }

    }
}
