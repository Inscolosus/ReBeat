using System;
using ReBeat.HarmonyPatches.BeamapData;

namespace ReBeat.HarmonyPatches.Energy {
    public class EnergyCounter {
        public const float ShieldCooldown = 2f; //0.2f;
        
        public int MaxShield { get; }
        public int MaxHealth { get; }
        
        public int Health { get; set; }
        public int Shield { get; set; }
        public int ShieldProgress { get; set; }
        public int ShieldRegen { get; } = (int)Math.Round(-20d / (1d + Math.Pow(Math.E, (NoteCount.Count / AudioLength.Length - 10d) / 2d)) + 30d);
        public float LastMiss { get; set; }
        public float TimeToNextWallDamage { get; set; } = 0.5f;
        public bool WasInWallLastFrame { get; set; }

        public int Misses { get; set; }
        public int TotalMisses { get; set; }

        public int Combo { get; set; }
        public int MaxCombo { get; set; }

        public EnergyCounter(int maxHealth, int maxShield) {
            MaxHealth = maxHealth;
            Health = maxHealth;
            MaxShield = maxShield;
            Shield = maxShield;
        }
    }
}