using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS {
    [System.Serializable]
    public class CharacterStats
    {
        [Header("Base Power")]
        public int hp = 100;
        public int fp = 50;
        public int stamina = 100;
        public float equipLoad = 20;
        public float poise = 20;
        public int itemDiscovery = 111;

        [Header("Attack Power")]
        public int R_weapon_1 = 51;
        public int R_weapon_2 = 0;
        public int R_weapon_3 = 0;
        public int L_weapon_1 = 20;
        public int L_weapon_2 = 0;
        public int L_weapon_3 = 0;

        [Header("Defense")]
        public int physical = 87;
        public int vs_strike = 87;
        public int vs_slash = 87;
        public int vs_thrust = 87;
        public int magic_def = 20;
        public int fire_def = 20;
        public int lightning_def = 20;
        public int dark_def = 20;
        
        [Header("Resistances")]
        public int bleed = 10;
        public int poison = 10;
        public int frostbite = 10;
        public int toxic = 10;
        public int curse = 5;

        public int attunementSlots = 0;

    }

    [System.Serializable]
    public class Attributes {
        public int level = 1;
        public int souls = 0;
        public int vigor = 11;
        public int attunement = 11;
        public int endurance = 11;
        public int vitality = 11;
        public int strength = 11;
        public int dexterity = 11;
        public int intelligence = 11;
        public int faith = 11;
        public int luck = 11;
    }

    [System.Serializable]
    public class WeaponStats{
        public int physical = 10;
        public int strike = 10;
        public int slash = 0;
        public int thrust = 0;
        public int magic = 0;
        public int fire = 0;
        public int lightning = 0;
        public int dark = 0;
    }
}