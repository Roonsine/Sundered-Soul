using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS {
    public class ResourcesManager : MonoBehaviour
    {
        Dictionary<string, int> item_ids = new Dictionary<string,int>();
        public static ResourcesManager singleton;
        void Awake() {
            singleton = this;
            LoadIds();
        }



        void LoadIds() {
            WeaponScriptableObject obj = Resources.Load("SS.WeaponScriptableObject") as WeaponScriptableObject;
            for (int i = 0; i < obj.weapons_all.Count; i++)
            {
                if(item_ids.ContainsKey(obj.weapons_all[i].itemName)) {
                    Debug.Log("item is a duplicate.");
                } else {
                    item_ids.Add(obj.weapons_all[i].itemName, i);
                }
            }
        }
        
        int GetItemIdFromString(string id) {
            int index = -1;
            if(item_ids.TryGetValue(id, out index)){
                return index;
            }
            return -1;
        }
    
        public Weapon GetWeapon(string id) {
            WeaponScriptableObject obj = Resources.Load("SS.WeaponScriptableObject") as WeaponScriptableObject;
            int index = GetItemIdFromString(id);
            if(index == -1)
                return null;            
            return obj.weapons_all[index];

            /* int index = -1;
            if(weapon_dict.TryGetValue(id, out index)) {
                return weaponList[index];
            }
            return null;*/
        }
    }
}