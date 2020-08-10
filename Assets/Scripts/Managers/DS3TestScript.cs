using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS {
    public static class DS3TestScript 
    {
        public static float CalculateBasePhysical(WeaponStats weaponStats, CharacterStats stats){

            float physicalDmg = 0;
                
            if(stats.physical > 8 * weaponStats.physical){
                return physicalDmg = 0.10f * weaponStats.physical;
            }
            else if(stats.physical > weaponStats.physical) {
                return physicalDmg = (19.2f / 49 * Mathf.Pow((weaponStats.physical / stats.physical - 0.125f), 2) + 0.1f) * weaponStats.physical;
            }
            else if(stats.physical > 0.4f * weaponStats.physical) {
                return physicalDmg = (-0.43f * Mathf.Pow((weaponStats.physical / stats.physical - 2.5f), 2) + 0.7f) * weaponStats.physical;
            }
            else if(stats.physical > 0.125f * weaponStats.physical) {
                return physicalDmg = (-0.8f / 121 * Mathf.Pow((weaponStats.physical / stats.physical - 8), 2) + 0.9f) * weaponStats.physical;
            }
            else if(stats.physical < 0.125f * weaponStats.physical) 
                return physicalDmg = 0.90f * weaponStats.physical;
            
            if(physicalDmg <= 0)
                return physicalDmg = 1;
            
            return physicalDmg;
        }

        public static float GatherPhysicals(WeaponStats weaponStats, CharacterStats stats){
            float physicalDmg = 0;
            
            return physicalDmg;
        }
    }
}
