using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS {
    public static class StatsCalculations 
    {
        public static int CalculateBaseDamage(WeaponStats weaponStats, CharacterStats stats, float multiplier = 1){
            float physical = (weaponStats.physical * multiplier) - stats.physical;
            float slash = (weaponStats.slash * multiplier) - stats.vs_slash;
            float strike = (weaponStats.strike * multiplier) - stats.vs_strike;
            float thrust = (weaponStats.thrust  * multiplier) - stats.vs_thrust;

            float sum = physical + slash + strike + thrust;

            float magic = (weaponStats.magic * multiplier) - stats.magic_def;
            float fire = (weaponStats.fire * multiplier) - stats.fire_def;
            float lightning = (weaponStats.lightning * multiplier) - stats.lightning_def;
            float dark = (weaponStats.dark * multiplier)  - stats.dark_def;

            sum += magic + fire + lightning + dark;

            if(sum < 0)
                sum = 1;

            return Mathf.RoundToInt(sum);
        }
    }
}