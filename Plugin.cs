using System.Reflection;
using BeatSaberMarkupLanguage.GameplaySetup;
using BeatSaberMarkupLanguage.Settings;
using HarmonyLib;
using IPA;
using IPA.Config.Stores;
using ReBeat.HarmonyPatches.UI;
using IPALogger = IPA.Logging.Logger;

namespace ReBeat {
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }
        internal static Harmony Harmony { get; private set; }


        [Init]
        public Plugin(IPALogger logger, IPA.Config.Config config) {
            Instance = this;
            Log = logger;
            Harmony = new Harmony("Inscolosus.BeatSaber.ReBeat");
            Config.Instance = config.Generated<Config>();
            
        }

        [OnStart]
        public void OnEnable() {
            Harmony.PatchAll(Assembly.GetExecutingAssembly());
            BSMLSettings.instance.AddSettingsMenu("ReBeat", "ReBeat.Views.Menu.bsml", Config.Instance);
            GameplaySetup.instance.AddTab("ReBeat", "ReBeat.Views.Menu.bsml", Config.Instance, MenuType.All);
            GameplaySetup.instance.AddTab("Modifiers", "ReBeat.Views.Modifiers.bsml", Modifiers.instance, MenuType.All);
        }

        [OnExit]
        public void OnDisable() {
            Harmony.UnpatchSelf();
        }
    }
}
