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
        internal static Harmony Harmony { get; private set; }

        public const float ShieldCooldown = 0.2f;

        [Init]
        public Plugin(IPALogger logger, IPA.Config.Config config) {
            Instance = this;
            Log = logger;
            Harmony = new Harmony("Inscolosus.BeatSaber.BeatSaber5");
            Config.Instance = config.Generated<Config>();

            BSMLSettings.instance.AddSettingsMenu("BeatSaber5", "BeatSaber5.Views.Menu.bsml", Config.Instance);
            GameplaySetup.instance.AddTab("BeatSaber5", "BeatSaber5.Views.Menu.bsml", Config.Instance, MenuType.All);
        }

        [OnEnable]
        public void OnEnable() {
            if (!Config.Instance.Enabled) return;
            Harmony.PatchAll(Assembly.GetExecutingAssembly());
            BS_Utils.Gameplay.ScoreSubmission.ProlongedDisableSubmission("BeatSaber5");
        }

        [OnDisable]
        public void OnDisable() {
            Harmony.UnpatchSelf();
            BS_Utils.Gameplay.ScoreSubmission.RemoveProlongedDisable("BeatSaber5");
        }

    }
}
