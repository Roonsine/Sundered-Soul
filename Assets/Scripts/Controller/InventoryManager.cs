﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.UI;

namespace SS {
    public class InventoryManager : MonoBehaviour
    {
        public Weapon rightHandWeapon;
        public bool hasLeftHandWeapon = true;
        public Weapon leftHandWeapon;

        public GameObject parryCollider;

        StateManager states;

        public void Init(StateManager st) {
            states = st;

            EquipWeapon(rightHandWeapon, false);
            EquipWeapon(leftHandWeapon, true);
            CloseAllDamageColliders();
            ParryCollider pr = parryCollider.GetComponent<ParryCollider>();
            pr.InitPlayer(st);
            CloseParryCollider();

        }

        public void EquipWeapon(Weapon w, bool isLeft = false) {
            string targetIdle = w.oh_idle;
            targetIdle += (isLeft) ? "_l" : "_r";
            states.anim.SetBool(StaticStrings.mirror, isLeft);
            states.anim.Play(StaticStrings.changeWeapon);
            states.anim.Play(targetIdle);

            QuickSlot uiSlot = QuickSlot.singleton;
            uiSlot.UpdateSlot(
                (isLeft)?
                QSlotType.lh : QSlotType.rh, w.icon);
        }

        public void OpenAllDamageColiders(){
            if(rightHandWeapon.w_hook != null)
                rightHandWeapon.w_hook.OpenDamageColliders();
            
            if(leftHandWeapon.w_hook != null)
                leftHandWeapon.w_hook.OpenDamageColliders();
        }

        public void CloseAllDamageColliders(){
            if(rightHandWeapon.w_hook != null)
                rightHandWeapon.w_hook.CloseDamageColliders();
            if(leftHandWeapon.w_hook != null)
                leftHandWeapon.w_hook.CloseDamageColliders();
        }

        public void CloseParryCollider(){
            parryCollider.SetActive(false);
        }

        public void OpenParryCollider(){
            parryCollider.SetActive(true);
        }
    }

    [System.Serializable]
    public class Weapon {
        public string weaponID;
        public Sprite icon;
        public string oh_idle;
        public string th_idle;

        public List<Action> actions;
        public List<Action> two_handedActions;
        public bool leftHandMirror;

        public GameObject weaponModel;
        public WeaponHook w_hook;

        public Action GetAction(List<Action> l, ActionInput inp) {
            for (int i = 0; i < l.Count; i++)
            {
                if(l[i].input == inp) {
                    return l[i];
                }
            }
            return null;
        }
    }
}