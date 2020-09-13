using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS {
    public class RuntimeWeapon : MonoBehaviour
    {
        public Weapon instance;
        public GameObject weaponModel;
        public WeaponHook w_hook;

        // public void init(Weapon w) {
        //     weaponModel = w.modelPrefab;
        // }
    }
}