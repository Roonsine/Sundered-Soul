using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS {
    public static class StatsCalculations 
    {
        public static int CalculateBaseDamage(WeaponStats weaponStats, CharacterStats stats){
            int physical = weaponStats.physical - stats.physical;
            int slash = weaponStats.slash - stats.vs_slash;
            int strike = weaponStats.strike - stats.vs_strike;
            int thrust = weaponStats.thrust - stats.vs_thrust;

            int sum = physical + slash + strike + thrust;

            int magic = weaponStats.magic - stats.magic_def;
            int fire = weaponStats.fire - stats.fire_def;
            int lightning = weaponStats.lightning - stats.lightning_def;
            int dark = weaponStats.dark - stats.dark_def;

            sum += magic + fire + lightning + dark;

            if(sum < 0)
                sum = 1;

            return sum;
        }
    }
}