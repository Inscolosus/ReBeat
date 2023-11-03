namespace BeatSaber5 {
	public class Config {
        public static Config Instance;

        private bool _enabled;
        public virtual bool Enabled {
            get => _enabled;
            set {
                _enabled = value;
                if (value) {
                    Plugin.Instance.OnEnable();
                }
                else {
                    Plugin.Instance.OnDisable();
                }
            }
        }

        public virtual bool ShowComboPercent { get; set; } = false;
        public virtual bool ScoreDebug { get; set; } = false;
        public virtual bool ProMode { get; set; } = false;
        public virtual bool DebugHitbox { get; set; } = false;
        public virtual float DebugHitboxSize { get; set; } = 1;

        public const float StartingHealth = 5.0f;

        /// <summary>
        /// This is called whenever BSIPA reads the config from disk (including when file changes are detected).
        /// </summary>
        public virtual void OnReload() {
            // Do stuff after config is read from disk.
        }

        /// <summary>
        /// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
        /// </summary>


        private bool startup = true;
        public virtual void Changed() {
            // Do stuff when the config is changed.

        }

        /// <summary>
        /// Call this to have BSIPA copy the values from <paramref name="other"/> into this config.
        /// </summary>
        public virtual void CopyFrom(Config other) {
            // This instance's members populated from other
        }
    }
}
