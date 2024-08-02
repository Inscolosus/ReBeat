using System.ComponentModel;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using TMPro;
using UnityEngine;

// ReSharper disable UnusedMember.Local

namespace ReBeat.HarmonyPatches.UI {
    public class Modifiers : NotifiableSingleton<Modifiers> {
        [UIComponent("MultiplierValue")] private TextMeshProUGUI multiplierValue;
        [UIComponent("MaxRank")] private TextMeshProUGUI maxRank;

        public override void OnEnable() {
            PropertyChanged += OnPropertyChanged;
        }
                                                     // nf  1l 1hp  nb  nw   easy  hidden da  sameColor
        private static readonly float[] Multipliers = { 0f, 0f, 0f, 0f, 0f, -0.4f, 0.05f, 0f, 0.07f,
         // pro   gn     ss    fs    sfs    na  sn
            0.12f, 0f, -0.5f, 0.07f, 0.15f, 0f, 0f };

        public float Multiplier {
            get {
                var mods = GetModifiers();
                float multiplier = 1f;
                for (int i = 0; i < mods.Length; i++) {
                    if (mods[i]) multiplier += Multipliers[i];
                }

                return multiplier;
            }
        }

        private bool[] GetModifiers() {
            return new[] { _noFail, _oneLife, _oneHp, _noBombs, _noWalls, _easyMode, _hidden, _disappearingArrows, _sameColor, 
                _proMode, _ghostNotes, _slowerSong, _fasterSong, _superFastSong, _noArrows, _smallNotes };
        }
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
            Color color = Multiplier >= 1 ? new Color(0f, 0.75f, 1f) : new Color(1f, 0.37f, 0f);
            multiplierValue.color = color;
            multiplierValue.text = Multiplier.ToString("P0");
            maxRank.color = color;
            maxRank.text = MaxRank(Multiplier);
        }

        private string MaxRank(float mul) {
            if (mul >= 0.9f) return "SS";
            if (mul >= 0.8f) return "S";
            if (mul >= 0.65f) return "A";
            if (mul >= 0.5f) return "B";
            if (mul >= 0.35f) return "C";
            if (mul >= 0.2f) return "D";
            return "E";
        }

        
        
        private bool _noFail;
        [UIValue("NoFail")]
        public bool NoFail {
            get => _noFail;
            private set {
                _noFail = value;
                NotifyPropertyChanged();
            }
        }

        [UIAction("SetNoFail")]
        private void SetNoFail(bool value) {
            NoFail = value;
        }
        
        
        private bool _oneLife;
        [UIValue("OneLife")]
        public bool OneLife {
            get => _oneLife;
            private set {
                if (value) {
                    OneHp = false;
                }
                
                _oneLife = value;
                NotifyPropertyChanged();
            }
        }

        [UIAction("SetOneLife")]
        private void SetOneLife(bool value) {
            OneLife = value;
        }
        
        
        private bool _oneHp;
        [UIValue("OneHp")] 
        public bool OneHp {
            get => _oneHp;
            private set {
                if (value) {
                    OneLife = false;
                }
                _oneHp = value;
                NotifyPropertyChanged();
            }
        }

        [UIAction("SetOneHp")]
        private void SetOneHP(bool value) {
            OneHp = value;
        }
        
        
        private bool _noBombs;
        [UIValue("NoBombs")] 
        public bool NoBombs {
            get => _noBombs;
            private set {
                _noBombs = value;
                NotifyPropertyChanged();
            }
        }

        [UIAction("SetNoBombs")]
        private void SetNoBombs(bool value) {
            NoBombs = value;
        }
        
        
        private bool _noWalls;
        [UIValue("NoWalls")] 
        public bool NoWalls {
            get => _noWalls;
            private set {
                _noWalls = value;
                NotifyPropertyChanged();
            }
        }

        [UIAction("SetNoWalls")]
        private void SetNoWalls(bool value) {
            NoWalls = value;
        }
        
        
        private bool _easyMode;
        [UIValue("EasyMode")] 
        public bool EasyMode {
            get => _easyMode;
            private set {
                if (value) {
                    ProMode = false;
                }
                _easyMode = value;
                NotifyPropertyChanged();
            }
        }

        [UIAction("SetEasyMode")]
        private void SetEasyMode(bool value) {
            EasyMode = value;
        }
        
        
        private bool _hidden;
        [UIValue("Hidden")] 
        public bool Hidden {
            get => _hidden;
            private set {
                if (value) {
                    GhostNotes = false;
                    DisappearingArrows = false;
                }
                _hidden = value;
                NotifyPropertyChanged();
            }
        }

        [UIAction("SetHidden")]
        private void SetHidden(bool value) {
            Hidden = value;
        }
        
        
        private bool _disappearingArrows;
        [UIValue("DisappearingArrows")] 
        public bool DisappearingArrows {
            get => _disappearingArrows;
            private set {
                if (value) {
                    Hidden = false;
                    GhostNotes = false;
                }
                _disappearingArrows = value;
                NotifyPropertyChanged();
            }
        }

        [UIAction("SetDisappearingArrows")]
        private void SetDisappearingArrows(bool value) {
            DisappearingArrows = value;
        }
        
        
        private bool _sameColor;
        [UIValue("SameColor")] 
        public bool SameColor {
            get => _sameColor;
            private set {
                _sameColor = value;
                NotifyPropertyChanged();
            }
        }

        [UIAction("SetSameColor")]
        private void SetSameColor(bool value) {
            SameColor = value;
        }
        
        
        private bool _proMode;
        [UIValue("ProMode")] 
        public bool ProMode {
            get => _proMode;
            private set {
                if (value) {
                    EasyMode = false;
                }
                _proMode = value;
                NotifyPropertyChanged();
            }
        }

        [UIAction("SetProMode")]
        private void SetProMode(bool value) {
            ProMode = value;
        }
        
        
        private bool _ghostNotes;
        [UIValue("GhostNotes")] 
        public bool GhostNotes {
            get => _ghostNotes;
            private set {
                if (value) {
                    DisappearingArrows = false;
                    Hidden = false;
                }
                _ghostNotes = value;
                NotifyPropertyChanged();
            }
        }

        [UIAction("SetGhostNotes")]
        private void SetGhostNotes(bool value) {
            GhostNotes = value;
        }
        
        
        private bool _slowerSong;
        [UIValue("SlowerSong")] 
        public bool SlowerSong {
            get => _slowerSong;
            private set {
                if (value) {
                    FasterSong = false;
                    SuperFastSong = false;
                }
                _slowerSong = value;
                NotifyPropertyChanged();
            }
        }

        [UIAction("SetSlowerSong")]
        private void SetSlowerSong(bool value) {
            SlowerSong = value;
        }
        
        
        private bool _fasterSong;
        [UIValue("FasterSong")] 
        public bool FasterSong {
            get => _fasterSong;
            private set {
                if (value) {
                    SlowerSong = false;
                    SuperFastSong = false;
                }
                _fasterSong = value;
                NotifyPropertyChanged();
            }
        }

        [UIAction("SetFasterSong")]
        private void SetFasterSong(bool value) {
            FasterSong = value;
        }
        
        
        private bool _superFastSong;
        [UIValue("SuperFastSong")] 
        public bool SuperFastSong {
            get => _superFastSong;
            private set {
                if (value) {
                    SlowerSong = false;
                    FasterSong = false;
                }
                _superFastSong = value;
                NotifyPropertyChanged();
            }
        }

        [UIAction("SetSuperFastSong")]
        private void SetSuperFastSong(bool value) {
            SuperFastSong = value;
        }
        
        
        private bool _noArrows;
        [UIValue("NoArrows")] 
        public bool NoArrows {
            get => _noArrows;
            private set {
                if (value) {
                    DisappearingArrows = false;
                }
                _noArrows = value;
                NotifyPropertyChanged();
            }
        }

        [UIAction("SetNoArrows")]
        private void SetNoArrows(bool value) {
            NoArrows = value;
        }
        
        
        private bool _smallNotes;
        [UIValue("SmallNotes")] 
        public bool SmallNotes {
            get => _smallNotes;
            private set {
                _smallNotes = value;
                NotifyPropertyChanged();
            }
        }

        [UIAction("SetSmallNotes")]
        private void SetSmallNotes(bool value) {
            SmallNotes = value;
        }
    }
}