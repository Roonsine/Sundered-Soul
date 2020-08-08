using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS {
    public class ParryCollider : MonoBehaviour
    {
        StateManager states;
        EnemyStates enemyStates;

        public float maxTimer = 0.6f;
        float timer = 0;

        public void InitPlayer(StateManager st) {
            states = st;
        }

        public void InitEnemy(EnemyStates st) {

        }

        public void Update(){
            if(states) {
                timer += states.delta;
                if(timer > maxTimer){
                    timer = 0;
                    gameObject.SetActive(false);
                }
            }

            if(enemyStates) {
                timer += enemyStates.delta;
                if(timer > maxTimer) {
                    timer = 0;
                    gameObject.SetActive(false);
                }
            }
        }

        void OnTriggerEnter(Collider other) {

            if(states){
                EnemyStates e_st = other.transform.GetComponentInParent<EnemyStates>();

                if(e_st != null){
                    e_st.CheckForParry(transform.root, states);
                }
            }

            if(enemyStates) {
                //Check for player
            }
        }
    }
}