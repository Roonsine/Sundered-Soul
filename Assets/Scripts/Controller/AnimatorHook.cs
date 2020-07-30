using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public class AnimatorHook : MonoBehaviour
    {
        Animator anim;
        StateManager states;
        public void Init(StateManager st)
        {
            states = st;
            anim = st.anim;
        }

        void OnAnimatorMove()
        {
            if (states.canMove)
                return;
        }
    }
}