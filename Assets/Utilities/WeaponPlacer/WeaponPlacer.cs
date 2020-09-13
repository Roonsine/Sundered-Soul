using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS {
    [ExecuteInEditMode]
    public class WeaponPlacer : MonoBehaviour
    {
        public string weaponID;
        public GameObject weaponModel;
        public bool leftHand;
        public bool saveWeapon;

        void Update(){
            if(!saveWeapon)
                return;
            
            saveWeapon = false;

            if(weaponModel == null)
                return;
            if(string.IsNullOrEmpty(weaponID))
                return;
            
            WeaponScriptableObject obj = Resources.Load("SS.WeaponScriptableObject") as WeaponScriptableObject;
            if(obj == null)
                return;
            for (int i = 0; i < obj.weapons_all.Count; i++)
            {
                if(obj.weapons_all[i].itemName == weaponID) {
                    Weapon w = obj.weapons_all[i];
                    if(leftHand) {
                        w.l_model_eulers = weaponModel.transform.localEulerAngles;
                        w.l_model_pos = weaponModel.transform.localPosition;
                    } else {
                        w.r_model_eulers = weaponModel.transform.localEulerAngles;
                        w.r_model_pos = weaponModel.transform.localPosition;
                    }
                    w.model_scale = weaponModel.transform.localScale;
                    return;
                }
            }
            Debug.Log(weaponID + " wasn't found in inventory");
        }
    }
}