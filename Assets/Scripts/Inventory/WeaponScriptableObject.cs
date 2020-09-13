using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS {
    public class WeaponScriptableObject : ScriptableObject
    {
        public List<Weapon> weapons_all = new List<Weapon>();
    }
}