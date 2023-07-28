namespace BeatSaber5 {
	public class Config {
        public static Config Instance;
        public virtual bool Enabled { get; set; } = true;
        public virtual bool LenientAcc { get; set; } = false;
        public virtual bool StrictEnergy { get; set; } = false;
        public virtual float Example { get; set; } = 45f;
        public virtual float ShieldRegen { get; set; } = 20f;
        public virtual bool EnableShieldCooldown { get; set; } = false;
        public virtual float ShieldCooldown { get; set; } = 0.5f;
        public virtual bool ShowComboPercent { get; set; } = true;


        /// <summary>
        /// This is called whenever BSIPA reads the config from disk (including when file changes are detected).
        /// </summary>
        public virtual void OnReload() {
            // Do stuff after config is read from disk.
            if (Enabled) {
                Plugin.Submission = false;
                BS_Utils.Gameplay.ScoreSubmission.ProlongedDisableSubmission("BeatSaber5");
            }
        }

        /// <summary>
        /// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
        /// </summary>
        public virtual void Changed() {
            // Do stuff when the config is changed.
            if (Plugin.Submission && Enabled) {
                Plugin.Submission = false;
                BS_Utils.Gameplay.ScoreSubmission.ProlongedDisableSubmission("BeatSaber5");
            }
            else if (!Plugin.Submission && !Enabled) {
                Plugin.Submission = true;
                BS_Utils.Gameplay.ScoreSubmission.RemoveProlongedDisable("BeatSaber5");
            }
        }

        /// <summary>
        /// Call this to have BSIPA copy the values from <paramref name="other"/> into this config.
        /// </summary>
        public virtual void CopyFrom(Config other) {
            // This instance's members populated from other
        }
    }
}
