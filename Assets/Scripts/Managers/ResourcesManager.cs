using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS {
    public class ResourcesManager : MonoBehaviour
    {
        Dictionary<string,int> spellIds = new Dictionary<string, int>();
        Dictionary<string, int> weaponIds = new Dictionary<string,int>();
        public static ResourcesManager singleton;

        void Awake() {
            singleton = this;
            LoadWeaponIds();
            LoadSpellIds();
        }

        void LoadSpellIds() {
            SpellItemsScriptableObject obj = Resources.Load("SS.SpellItemsScriptableObject") as SpellItemsScriptableObject;
            if(obj == null){
                Debug.Log("SpellItemsScriptableObject could not be loaded.");
                return;
            }

            for (int i = 0; i < obj.spellItems.Count; i++)
            {
                if(spellIds.ContainsKey(obj.spellItems[i].itemName)) {
                    Debug.Log(obj.spellItems[i].itemName + " item is a duplicate");
                } else {
                    spellIds.Add(obj.spellItems[i].itemName, i);
                }
            }
        }

        void LoadWeaponIds() {
            WeaponScriptableObject obj = Resources.Load("SS.WeaponScriptableObject") as WeaponScriptableObject;
            if(obj == null){
                Debug.Log("WeaponScriptableObject could not be loaded.");
                return;
            }
            for (int i = 0; i < obj.weaponsAll.Count; i++)
            {
                if(weaponIds.ContainsKey(obj.weaponsAll[i].itemName)) {
                    Debug.Log(obj.weaponsAll[i].itemName + " item is a duplicate.");
                } else {
                    weaponIds.Add(obj.weaponsAll[i].itemName, i);
                }
            }
        }
        
        int GetWeaponIdFromString(string id) {
            int index = -1;
            if(weaponIds.TryGetValue(id, out index)){
                return index;
            }
            return index;
        }
    
        public Weapon GetWeapon(string id) {
            WeaponScriptableObject obj = Resources.Load("SS.WeaponScriptableObject") as WeaponScriptableObject;
            if(obj == null){
                Debug.Log("WeaponScriptableObject could not be loaded.");
                return null;
            }
            int index = GetWeaponIdFromString(id);
            if(index == -1)
                return null;            
            return obj.weaponsAll[index];
        }

        int GetSpellIdFromString(string id) {
            int index = -1;
            if(spellIds.TryGetValue(id, out index)) {
                return index;
            }
            return index;
        }

        public Spell GetSpell(string id) {
            SpellItemsScriptableObject obj = Resources.Load("SS.SpellItemsScriptableObject") as SpellItemsScriptableObject;
            if(obj == null){
                Debug.Log("SpellItemsScriptableObject could not be loaded.");
                return null;
            }
            int index = GetSpellIdFromString(id);
            if(index == -1)
                return null;
            return obj.spellItems[index];
        }
    }
}