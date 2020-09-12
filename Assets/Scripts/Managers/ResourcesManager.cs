using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS {
    public class ResourcesManager : MonoBehaviour
    {
        public List<Weapon> weaponList = new List<Weapon>();
        Dictionary<string, int> weapon_dict = new Dictionary<string, int>();

        public static ResourcesManager singleton;
        void Awake() {
            singleton = this;

            for (int i = 0; i < weaponList.Count; i++)
            {
                if(string.IsNullOrEmpty(weaponList[i].weaponID)) {
                    continue;
                }

                if(!weapon_dict.ContainsKey(weaponList[i].weaponID)) {
                    weapon_dict.Add(weaponList[i].weaponID, i);
                } else {
                    Debug.Log(weaponList[i].weaponID + " is a duplicate id");
                }
            }
        }

        public Weapon GetWeapon(string id) {
            int index = -1;
            if(weapon_dict.TryGetValue(id, out index)) {
                return weaponList[index];
            }
            return null;
        }
    }
}