using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS {
    public class EnemyStates : MonoBehaviour
    {
        public float health;

        public CharacterStats characterStats;

        public bool canBeParried = true;
        public bool parryable = true;
        public bool isInvincible;
        public bool dontDoAnything;
        public bool canMove;
        public bool isDead;

        public StateManager parriedBy;

        public Animator anim;
        EnemyTarget enemyTarget;
        AnimatorHook a_hook;
        public Rigidbody rigid;
        public float delta;
        public float poiseDegrade = 2;

        List<Rigidbody> ragdollRigids = new List<Rigidbody>();
        List<Collider> ragdollColliders = new List<Collider>();

        float timer;

        void Start(){
            health = characterStats.hp;
            anim = GetComponentInChildren<Animator>();
            enemyTarget = GetComponent<EnemyTarget>();
            enemyTarget.Init(this);

            rigid = GetComponent<Rigidbody>();

            a_hook = anim.GetComponent<AnimatorHook>();
            if(a_hook == null)
                a_hook = anim.gameObject.AddComponent<AnimatorHook>();
            a_hook.Init(null,this);

            InitRagdoll();
            parryable = false;
        }
        
        void InitRagdoll() {
            Rigidbody[] rigs = GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < rigs.Length; i++)
            {
                if(rigs[i] == rigid)
                    continue;
                
                ragdollRigids.Add(rigs[i]);
                rigs[i].isKinematic = true;

                Collider col = rigs[i].gameObject.GetComponent<Collider>();
                col.isTrigger = true;
                ragdollColliders.Add(col);
            }
        }

        public void EnableRagdoll() {
            
            for (int i = 0; i < ragdollRigids.Count; i++)
            {
                ragdollRigids[i].isKinematic = false;
                ragdollColliders[i].isTrigger = false;
            }

            Collider controllerCollider = rigid.gameObject.GetComponent<Collider>();
            controllerCollider.enabled = false;
            rigid.isKinematic = true;

            StartCoroutine("CloseAnimator");
        }

        IEnumerator CloseAnimator(){
            yield return new WaitForEndOfFrame();
            anim.enabled = false;
            this.enabled = false;
        }

        void Update(){
            delta = Time.deltaTime;
            canMove = anim.GetBool(StaticStrings.canMove);
            if(dontDoAnything){
                dontDoAnything = !canMove;
                return;
            }
            if(health <=0){
                if(!isDead) {
                    isDead = true;
                    EnableRagdoll();
                }
            }
            if(isInvincible){
                isInvincible = !canMove;
            }
            
            if(parriedBy != null && parryable == false) {
                parriedBy = null;
            }

            if(canMove){
                parryable = false;
                anim.applyRootMotion = false;

                timer += Time.deltaTime;
                if(timer > 3){
                    DoAction();
                    timer = 0;
                }
            }

            characterStats.poise -= delta * poiseDegrade;
            if(characterStats.poise < 0)
                characterStats.poise = 0;
        }

        void DoAction(){
            anim.Play("oh_attack_1");
            anim.applyRootMotion = true;
            anim.SetBool(StaticStrings.canMove, false);
        }

        public void DoDamage(Action a){
            if(isInvincible)
                return;

            int damage = StatsCalculations.CalculateBaseDamage(a.weaponStats, characterStats);

            characterStats.poise += damage;          
            health -= damage;
            
            if(canMove || characterStats.poise > 100) {
                if(a.overrideDamageAnim)
                    anim.Play(a.damageAnim);
                else {
                    int ran = Random.Range(0, 100);
                    string tA = (ran > 50 ) ? StaticStrings.damage1 : StaticStrings.damage2;
                    anim.Play(tA);
               }
            }

            isInvincible = true;
            anim.applyRootMotion = true;
            anim.SetBool(StaticStrings.canMove, false);
        }

        public void CheckForParry(Transform target, StateManager states){
            if(canBeParried == false || parryable == false || isInvincible)
                return;
            
            Vector3 dir = transform.position - target.position;
            dir.Normalize();
            float dot = Vector3.Dot(target.forward, dir);
            if(dot < 0)
                return;


            isInvincible = true;
            anim.Play(StaticStrings.attack_interrupt);
            anim.applyRootMotion = true;
            anim.SetBool(StaticStrings.canMove, false);
            parriedBy = states;
        }

        public void BeingRiposted(Action a) {
            int damage = StatsCalculations.CalculateBaseDamage(a.weaponStats, characterStats, a.parryMultiplier);
            health -= damage;

            dontDoAnything = true;
            anim.SetBool(StaticStrings.canMove, false);
            anim.Play(StaticStrings.parry_recieved);
        }

        public void BeingBackstabbed(Action a) {
            int damage = StatsCalculations.CalculateBaseDamage(a.weaponStats, characterStats, a.backstabMultiplier);
            health -= damage;
            dontDoAnything = true;
            anim.SetBool(StaticStrings.canMove, false);
            anim.Play(StaticStrings.backstabbed);
        }
    }
}