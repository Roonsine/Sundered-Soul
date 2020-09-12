using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS {
    public static class StaticFunctions
    {
        public static void DeepCopyWeapon(Weapon from, Weapon to) {
            to.weaponID = from.weaponID;
            to.icon = from.icon;
            to.oh_idle = from.oh_idle;
            to.th_idle = from.th_idle;

            to.actions = new List<Action>();
            for (int i = 0; i < from.actions.Count; i++)
            {
                Action a = new Action();
                a.weaponStats = new WeaponStats();
                DeepCopyActionToAction(a, from.actions[i]);
                to.actions.Add(a);
            }

            to.two_handedActions = new List<Action>();
            for (int i = 0; i < from.two_handedActions.Count; i++)
            {
                Action a = new Action();
                a.weaponStats = new WeaponStats();
                DeepCopyActionToAction(a, from.two_handedActions[i]);
                to.two_handedActions.Add(a);
            }

            to.parryMultiplier = from.parryMultiplier;
            to.backstabMultiplier = from.backstabMultiplier;
            to.leftHandMirror = from.leftHandMirror;
            to.modelPrefab = from.modelPrefab;
            to.model_pos = from.model_pos;
            to.model_eulers = from.model_eulers;
            to.model_scale = from.model_scale;

            
        }

        public static void DeepCopyActionToAction(Action a, Action w_a) {
            a.targetAnim = w_a.targetAnim;
            a.type = w_a.type;
            a.canBeParried = w_a.canBeParried;
            a.changeSpeed = w_a.changeSpeed;
            a.animSpeed = w_a.animSpeed;
            a.canBackstab = w_a.canBackstab;
            a.overrideDamageAnim = w_a.overrideDamageAnim;
            a.damageAnim = w_a.damageAnim;

            DeepCopyWeaponStats(w_a.weaponStats, a.weaponStats);

        }

        public static void DeepCopyAction(Weapon w, ActionInput input, ActionInput assign, List<Action> actionList, bool isLeftHand = false) {
        Action a = GetAction(assign, actionList);
        Action w_a = w.GetAction(w.actions, input);
        if(w_a == null)
            return;

        a.targetAnim = w_a.targetAnim;
        a.type = w_a.type;
        a.canBeParried = w_a.canBeParried;
        a.changeSpeed = w_a.changeSpeed;
        a.animSpeed = w_a.animSpeed;
        a.canBackstab = w_a.canBackstab;
        a.overrideDamageAnim = w_a.overrideDamageAnim;
        a.damageAnim = w_a.damageAnim;
        a.parryMultiplier = w.parryMultiplier;
        a.backstabMultiplier = w.backstabMultiplier;

        if(isLeftHand){
            a.mirror = true;
        }

        DeepCopyWeaponStats(w_a.weaponStats, a.weaponStats);
    }

        public static void DeepCopyWeaponStats(WeaponStats from, WeaponStats to) {
            to.physical = from.physical;
            to.slash = from.slash;
            to.strike = from.strike;
            to.thrust = from.thrust;
            to.magic = from.magic;
            to.fire = from.fire;
            to.lightning = from.lightning;
            to.dark = from.dark;
        }
        public static Action GetAction(ActionInput inp, List<Action> actionSlots){
            for (int i = 0; i < actionSlots.Count; i++)
            {
                if(actionSlots[i].input == inp)
                    return actionSlots[i];
            }
            return null;
        }
    }
}