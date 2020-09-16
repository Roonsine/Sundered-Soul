﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.UI;

namespace SS {
    public class InventoryManager : MonoBehaviour
    {
        public List<string> rh_weapons;
        public List<string> lh_weapons;
        public List<string> spell_items;

        public int r_index;
        public int l_index;
        public int s_index;
        List<RuntimeWeapon> r_r_weapons = new List<RuntimeWeapon>();
        List<RuntimeWeapon> r_l_weapons = new List<RuntimeWeapon>();
        List<RuntimeSpellItems> r_spells = new List<RuntimeSpellItems>();
        
        public RuntimeSpellItems currentSpell;
        public RuntimeWeapon rightHandWeapon;
        public bool hasLeftHandWeapon = true;
        public RuntimeWeapon leftHandWeapon;

        public GameObject parryCollider;

        StateManager states;

        public void Init(StateManager st) {
            states = st;
            LoadInventory();

            ParryCollider pr = parryCollider.GetComponent<ParryCollider>();
            pr.InitPlayer(st);
            CloseParryCollider();
        }

        public void LoadInventory(){

            for (int i = 0; i < rh_weapons.Count; i++)
            {
                WeaponToRuntimeWeapon(ResourcesManager.singleton.GetWeapon(rh_weapons[i]));
            }

            for (int i = 0; i < lh_weapons.Count; i++)
            {
                WeaponToRuntimeWeapon(ResourcesManager.singleton.GetWeapon(lh_weapons[i]), true);
            }

            if(r_r_weapons.Count > 0) {
                if(r_index > r_r_weapons.Count - 1)
                    r_index = 0;
                
                rightHandWeapon = r_r_weapons[r_index];
            }
            
            if(r_l_weapons.Count > 0) {
                if(l_index > r_l_weapons.Count - 1)
                    l_index = 0;
                
                leftHandWeapon = r_l_weapons[l_index];
            }

            if(rightHandWeapon != null)
                EquipWeapon(rightHandWeapon, false);           

            if(leftHandWeapon != null){
                EquipWeapon(leftHandWeapon, true);
                hasLeftHandWeapon = true;
            }

            for (int i = 0; i < spell_items.Count; i++)
            {
                SpellToRuntimeSpell(ResourcesManager.singleton.GetSpell(spell_items[i]));
            }

            if(r_spells.Count > 0)
                if(s_index > r_spells.Count -1)
                    s_index = 0;
                EquipSpell(r_spells[s_index]);

            InitAllDamageColliders(states);
            CloseAllDamageColliders();
        }

        public void EquipWeapon(RuntimeWeapon w, bool isLeft = false) {
            if(isLeft) {
                if(leftHandWeapon != null) {
                    leftHandWeapon.weaponModel.SetActive(false);
                }
                leftHandWeapon = w;
            } else {
                if(rightHandWeapon != null) {
                    rightHandWeapon.weaponModel.SetActive(false);
                }

                rightHandWeapon = w;
            }
            string targetIdle = w.instance.oh_idle;
            targetIdle += (isLeft) ? StaticStrings._l : StaticStrings._r;
            states.anim.SetBool(StaticStrings.mirror, isLeft);
            states.anim.Play(StaticStrings.changeWeapon);
            states.anim.Play(targetIdle);

            QuickSlot uiSlot = QuickSlot.singleton;
            uiSlot.UpdateSlot(
                (isLeft)?
                QSlotType.lh : QSlotType.rh, w.instance.icon);
            
            w.weaponModel.SetActive(true);
        }

        public void EquipSpell(RuntimeSpellItems spell) {
            currentSpell = spell;
            QuickSlot uiSlot = QuickSlot.singleton;
            uiSlot.UpdateSlot(QSlotType.spell, spell.instance.icon);
        }

        public Weapon GetCurrentWeapon(bool isLeft) {
            if(isLeft)
                return leftHandWeapon.instance;
            else
                return rightHandWeapon.instance;
        }

        public void OpenAllDamageColiders(){
            if(rightHandWeapon.w_hook != null)
                rightHandWeapon.w_hook.OpenDamageColliders();
            
            if(leftHandWeapon != null)
                if(leftHandWeapon.w_hook != null)
                    leftHandWeapon.w_hook.OpenDamageColliders();
        }

        public void CloseAllDamageColliders(){
            if(rightHandWeapon.w_hook != null)
                rightHandWeapon.w_hook.CloseDamageColliders();
            
            if(leftHandWeapon != null)
                if(leftHandWeapon.w_hook != null)
                    leftHandWeapon.w_hook.CloseDamageColliders();
        }

        public void InitAllDamageColliders(StateManager states) {
            if(rightHandWeapon.w_hook != null)
                rightHandWeapon.w_hook.InitDamageColliders(states);
                
            if(leftHandWeapon != null)
                if(leftHandWeapon.w_hook != null)
                    leftHandWeapon.w_hook.InitDamageColliders(states);
        }

        public void CloseParryCollider(){
            parryCollider.SetActive(false);
        }

        public void OpenParryCollider(){
            parryCollider.SetActive(true);
        }

        public RuntimeSpellItems SpellToRuntimeSpell (Spell s) {
            GameObject go = new GameObject();
            RuntimeSpellItems inst = go.AddComponent<RuntimeSpellItems>();
            inst.instance = new Spell();
            StaticFunctions.DeepCopySpell(s, inst.instance);
            go.name = s.itemName;
            r_spells.Add(inst);
            return inst;
        }

        public RuntimeWeapon WeaponToRuntimeWeapon(Weapon w, bool isLeft = false) {
            GameObject go = new GameObject();
            RuntimeWeapon inst = go.AddComponent<RuntimeWeapon>();
            go.name = w.itemName;           

            inst.instance = new Weapon();
            StaticFunctions.DeepCopyWeapon(w, inst.instance);

            inst.weaponModel = Instantiate(inst.instance.modelPrefab) as GameObject;
            Transform p = states.anim.GetBoneTransform((isLeft) ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand);
            inst.weaponModel.transform.parent = p;
            if(isLeft){
                inst.weaponModel.transform.localPosition = inst.instance.l_model_pos;
                inst.weaponModel.transform.localEulerAngles = inst.instance.l_model_eulers;
            } else {
                inst.weaponModel.transform.localPosition = inst.instance.r_model_pos;
                inst.weaponModel.transform.localEulerAngles = inst.instance.r_model_eulers;                
            }
            inst.weaponModel.transform.localScale = inst.instance.model_scale;
            
            inst.w_hook = inst.weaponModel.GetComponentInChildren<WeaponHook>();
            inst.w_hook.InitDamageColliders(states);

            if(isLeft) {
                r_l_weapons.Add(inst);
            } else {
                r_r_weapons.Add(inst);
            }

            inst.weaponModel.SetActive(false);
            return inst;
        }

        public void ChangeToNextWeapon(bool isLeft) {
            if(isLeft) {
                if(l_index < r_l_weapons.Count -1) 
                    l_index++;
                else 
                    l_index = 0;
                
                EquipWeapon(r_l_weapons[l_index], true);
            } else {
                if(r_index < r_r_weapons.Count - 1) 
                    r_index++;
                else 
                    r_index = 0;
                EquipWeapon(r_r_weapons[r_index]);
            }

            states.actionManager.UpdateActionsOneHanded();
        }

        public void ChangeNextSpell() {
            if(s_index < r_spells.Count -1)
                s_index++;
            else
                s_index = 0;
            EquipSpell(r_spells[s_index]);
        }
    }

    [System.Serializable]
    public class Item {
        public string itemName;
        public string itemDescription;
        public Sprite icon;
    }

    [System.Serializable]
    public class Weapon : Item {
        public string oh_idle;
        public string th_idle;

        public List<Action> actions;
        public List<Action> two_handedActions;
        public float parryMultiplier;
        public float backstabMultiplier;
        public bool leftHandMirror;

        public GameObject modelPrefab;

        public Action GetAction(List<Action> l, ActionInput inp) {
            for (int i = 0; i < l.Count; i++)
            {
                if(l[i].input == inp) {
                    return l[i];
                }
            }
            return null;
        }

        public Vector3 r_model_pos;
        public Vector3 l_model_pos;
        public Vector3 r_model_eulers;
        public Vector3 l_model_eulers;
        public Vector3 model_scale;
    }

    [System.Serializable]
    public class Spell : Item {
        public SpellType spellType;
        public SpellClass spellClass;
        public GameObject projectile;
        public GameObject particlePrefab;
    }


}