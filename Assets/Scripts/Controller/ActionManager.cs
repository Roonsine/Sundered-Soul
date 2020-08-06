using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS 
{
    public class ActionManager : MonoBehaviour
    {
        public List<Action> actionSlots = new List<Action>();
        
        public ItemAction consumableItem;
        
        StateManager states;

        public void Init(StateManager st) {
            states = st;
            UpdateActionsOneHanded();
            

        }
        
        public void UpdateActionsOneHanded(){
            EmptyAllSlots();

            if(states.inventoryManager.hasLeftHandWeapon){                
                UpdateActionsLeftHand();
                return;
            }
            Weapon w = states.inventoryManager.rightHandWeapon;
            for (int i = 0; i < w.actions.Count; i++)
            {
                Action a = GetAction(w.actions[i].input);
                a.targetAnim = w.actions[i].targetAnim;
            }
        }

        public void UpdateActionsLeftHand(){
            Weapon r_w = states.inventoryManager.rightHandWeapon;
            Weapon l_w = states.inventoryManager.leftHandWeapon;

            Action rb = GetAction(ActionInput.rb);
            Action rt = GetAction(ActionInput.rt);
            rb.targetAnim = r_w.GetAction(r_w.actions, ActionInput.rb).targetAnim;
            rt.targetAnim = r_w.GetAction(r_w.actions, ActionInput.rt).targetAnim;

            Action lb = GetAction(ActionInput.lb);
            Action lt = GetAction(ActionInput.lt);
            lb.targetAnim = l_w.GetAction(l_w.actions, ActionInput.rb).targetAnim;
            lt.targetAnim = l_w.GetAction(l_w.actions, ActionInput.rt).targetAnim;
            
            if(l_w.leftHandMirror){
                lb.mirror = true;
                lt.mirror = true;
            }
        }

        public void UpdateActionsTwoHanded(){
            EmptyAllSlots();
            Weapon w = states.inventoryManager.rightHandWeapon;
            for (int i = 0; i < w.two_handedActions.Count; i++)
            {
                Action a = GetAction(w.two_handedActions[i].input);
                a.targetAnim = w.two_handedActions[i].targetAnim;
            }
        }

        void EmptyAllSlots(){
            for (int i = 0; i < 4; i++)
            {
                Action a = GetAction((ActionInput)i);
                a.targetAnim = null;
                a.mirror = false;
            }
        }

        ActionManager() {
            for (int i = 0; i < 4; i++)
            {
                Action a = new Action();
                a.input = (ActionInput)i;
                actionSlots.Add(a);
            }
        }
        
        public Action GetActionSlot(StateManager st){
            ActionInput a_input = GetActionInput(st);
            return GetAction(a_input);
        }

        Action GetAction(ActionInput inp){
            for (int i = 0; i < actionSlots.Count; i++)
            {
                if(actionSlots[i].input == inp)
                    return actionSlots[i];
            }
            return null;
        }

        public ActionInput GetActionInput(StateManager st) {
            if(st.rb)
                return ActionInput.rb;
            if(st.lb)
                return ActionInput.lb;
            if(st.rt)
                return ActionInput.rt;
            if(st.lt)
                return ActionInput.lt;
            
            return ActionInput.rb;
        }
    }

    public enum ActionInput {
    rb,lb,rt,lt
    }

    [System.Serializable]
    public class Action {
        public ActionInput input;
        public string targetAnim;
        public bool mirror = false;
        
    }
    [System.Serializable]
    public class ItemAction {
        public string targetAnim;
        public string item_id;
    }
}