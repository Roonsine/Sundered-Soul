using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

// Roll speed of 5 is a fat roll, roll speed of 7 is normal roll and a roll speed of 10 is a fast roll.
namespace SS
{
    public class AnimatorHook : MonoBehaviour
    {
        Animator anim;
        StateManager states;
        EnemyStates enemyStates;
        Rigidbody rigid;


        public float rm_multi;
        bool rolling;
        float roll_t;
        float delta;
        AnimationCurve roll_curve;

        public void Init(StateManager st, EnemyStates eSt)
        {
            states = st;
            enemyStates = eSt;
            if(st != null){
                anim = st.anim;
                rigid = st.rigid;
                roll_curve = states.roll_curve;
                delta = st.delta;
            }
            if(eSt != null){
                anim = eSt.anim;
                rigid = eSt.rigid;
                delta = eSt.delta;

            }
        }

        public void InitForRoll() {
            rolling = true;
            roll_t = 0;
        }

        public void CloseRoll() {
            if(rolling == false)
            return;

            rm_multi = 1;
            roll_t = 0;
            rolling = false;
        }

        void OnAnimatorMove()
        {
            if(states == null && enemyStates == null)
                return;

            if(rigid == null)
                return;

            if (states != null) {
                if(states.canMove)
                    return;

                delta = states.delta;
            }
            
            if(enemyStates != null) {
                if(enemyStates.canMove)
                    return;
                delta = enemyStates.delta;
            }

            rigid.drag = 0;

            if (rm_multi == 0)
                rm_multi = 1;
            
            if(rolling == false){

            Vector3 delta2 = anim.deltaPosition;
            delta2.y = 0;
            Vector3 v = (delta2 * rm_multi) / delta;
            rigid.velocity = v;
            } else {
                roll_t += delta;
                if(roll_t > 1) {
                    roll_t = 1;
                }

                if(states == null)
                    return;
                float zValue = roll_curve.Evaluate(roll_t);
                Vector3 v1 = Vector3.forward * zValue;
                Vector3 relative = transform.TransformDirection(v1);
                Vector3 v2 = (relative * rm_multi);
                rigid.velocity = v2;
            }
        }

        public void OpenDamageColliders() {
            if(states == null)
                return;

            states.inventoryManager.OpenAllDamageColiders();
        }

        public void CloseDamageColliders(){
            if(states == null)
                return;

            states.inventoryManager.CloseAllDamageColliders();
        }
    }
}