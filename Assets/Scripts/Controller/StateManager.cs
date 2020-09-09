using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public class StateManager : MonoBehaviour
    {
        [Header("Init")]
        public GameObject activeModel;

        [Header("Player Stats")]
        public Attributes attributes;
        public CharacterStats characterStats;

        [Header("Inputs")]
        public float vertical;
        public float horizontal;
        public float moveAmount;
        public Vector3 moveDir;
        public bool rt, rb, lt, lb;
        public bool rollInput;
        public bool itemInput;

        [Header("stats")]
        public float moveSpeed = 2;
        public float runSpeed = 3.5f;
        public float rotateSpeed = 5;
        public float toGround = 0.5f;
        public float rollSpeed = 1;
        public float critAtkOffset = 1.4f;

        [Header("States")]
        public bool run;
        public bool onGround;
        public bool lockOn;
        public bool inAction;
        public bool canMove;
        public bool isTwoHanded;
        public bool usingItem;
        public bool canBeParried;
        public bool parryable;
        public bool isBlocking;
        public bool isLeftHand;
        

        [Header("Other")]
        public EnemyTarget lockOnTarget;
        public Transform lockOnTransform;
        public AnimationCurve roll_curve;

        [HideInInspector]
        public Animator anim;
        [HideInInspector]
        public Rigidbody rigid;
        [HideInInspector]
        public AnimatorHook a_hook;
        [HideInInspector]
        public ActionManager actionManager;
        [HideInInspector]
        public InventoryManager inventoryManager;

        [HideInInspector]
        public float delta;
        [HideInInspector]
        public LayerMask ignoreLayers;

        [HideInInspector]
        public Action currentAction ;

        float _actionDelay;

        public void Init()
        {
            SetupAnimator();
            rigid = GetComponent<Rigidbody>();
            rigid.angularDrag = 999;
            rigid.drag = 4;
            rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            inventoryManager = GetComponent<InventoryManager>();
            inventoryManager.Init(this);

            actionManager = GetComponent<ActionManager>();
            actionManager.Init(this);

            a_hook = activeModel.AddComponent<AnimatorHook>();
            if(a_hook == null)
                a_hook = activeModel.AddComponent<AnimatorHook>();
            a_hook.Init(this, null);

            gameObject.layer = 8;
            ignoreLayers = ~(1 << 9);

            anim.SetBool("onGround", true);
        }

        void SetupAnimator()
        {
            if (activeModel == null)
            {
                anim = GetComponentInChildren<Animator>();
                if (anim == null)
                {
                    Debug.Log("No Model Found!");
                }
                else
                {
                    activeModel = anim.gameObject;
                }
            }
            if (anim == null)
                anim = activeModel.GetComponent<Animator>();

            anim.applyRootMotion = false;
        }

        public void FixedTick(float d)
        {
            delta = d;

            isBlocking = false;
            usingItem = anim.GetBool(StaticStrings.interacting);
            DetectAction();
            DetectItemAction();
            inventoryManager.rightHandWeapon.instance.weaponModel.SetActive(!usingItem);

            anim.SetBool(StaticStrings.blocking, isBlocking);
            anim.SetBool(StaticStrings.isLeft, isLeftHand);

            if (inAction)
            {
                anim.applyRootMotion = true;
                canMove = false;
                _actionDelay += delta;
                if (_actionDelay > 1)
                { 
                     inAction = false;
                    _actionDelay = 0;
                }
                else
                {
                    return;
                } 
            }
            canMove = anim.GetBool("can_move");
            if (!canMove)
                return;

            //a_hook.rm_multi = 1;
            a_hook.CloseRoll();
            HandleRolls();


            anim.applyRootMotion = false;
            rigid.drag = (moveAmount > 0 || onGround == false) ? 0 : 4;         

            float targetSpeed = moveSpeed;

            if(usingItem) {
                run = false;
                moveAmount = Mathf.Clamp(moveAmount, 0, 0.4f);
            }

            if (run)
                targetSpeed = runSpeed;

            if(onGround)
                rigid.velocity = moveDir * (targetSpeed * moveAmount);

            if (run)
                lockOn = false;

            Vector3 targetDir = (lockOn == false) ? 
                moveDir 
                : 
                (lockOnTransform != null ) ? 
                    lockOnTransform.transform.position - transform.position 
                    : 
                    moveDir;
            targetDir.y = 0;
            if (targetDir == Vector3.zero)
                targetDir = transform.forward;
            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, delta * moveAmount * rotateSpeed);
            transform.rotation = targetRotation;


            anim.SetBool("lockon", lockOn);
            if (lockOn == false)
                HandleMovementAnimations();
            else
                HandleLockOnAnimations(moveDir);
        }

        public void DetectItemAction() {
            if(canMove == false || usingItem || isBlocking)
                return;
            
            if(itemInput == false)
                return;
            
            ItemAction slot = actionManager.consumableItem;
            string targetAnim = slot.targetAnim;
            if(string.IsNullOrEmpty(targetAnim))
                return;
            
            usingItem = true;
            anim.Play(targetAnim);
        }

        public void DetectAction()
        {
            if (canMove == false || usingItem)
                return;
            if (onGround == false)
                return;

            if (rb == false && rt == false && lt == false && lb == false)
                return;


            Action slot = actionManager.GetActionSlot(this);
            if(slot == null)
                return;
            switch (slot.type)
            {
                case ActionType.attack:
                    AttackAction(slot);
                    break;
                case ActionType.block:
                    BlockAction(slot);
                    break;
                case ActionType.spells:
                    break;
                case ActionType.parry:
                    ParryAction(slot);
                    break;
                default:
                    break;
            }
        }

        void AttackAction(Action slot) {

            if(CheckForParry(slot))
                return;
            if(CheckForBackStab(slot))
                return;

            string targetAnim = null;
            targetAnim = slot.targetAnim;

            if (string.IsNullOrEmpty(targetAnim))
                return;

            currentAction = slot;

            canMove = false;
            inAction = true;
            float  targetSpeed = 1;
            if(slot.changeSpeed) {
                targetSpeed = slot.animSpeed;
                if(targetSpeed == 0)
                    targetSpeed = 1;
            }
            anim.SetFloat(StaticStrings.animSpeed, targetSpeed);
            anim.SetBool(StaticStrings.mirror, slot.mirror);
            anim.Play(targetAnim);
        }

        bool CheckForParry(Action slot) {
            if(slot.canRiposte == false)
                return false;

            EnemyStates parryTarget = null;
            Vector3 origin = transform.position;
            origin.y += 1;
            Vector3 rayDir = transform.forward;
            RaycastHit hit;
            if(Physics.Raycast(origin, rayDir, out hit, 3, ignoreLayers)){
                parryTarget = hit.transform.GetComponentInParent<EnemyStates>();
            }

            if(parryTarget == null)
                return false;

            if(parryTarget.parriedBy == null)
                return false;
            
            // float dis = Vector3.Distance(parryTarget.transform.position, transform.position);

            // if( dis > 3) {
            //     return false;
            // }
            
            Vector3 dir = parryTarget.transform.position - transform.position;
            dir.Normalize();
            dir.y = 0;
            float angle = Vector3.Angle(transform.forward, dir);

            if(angle < 60) {
                Vector3 targetPosition = -dir * critAtkOffset;
                targetPosition += parryTarget.transform.position;
                transform.position = targetPosition;
                
                if(dir == Vector3.zero) 
                    dir = -parryTarget.transform.forward;
                Quaternion eRotation = Quaternion.LookRotation(-dir);
                Quaternion playerRot = Quaternion.LookRotation(dir);

                parryTarget.transform.rotation = eRotation;
                transform.rotation = playerRot;
                parryTarget.BeingRiposted(slot);
                canMove = false;
                inAction = true;
                anim.SetBool(StaticStrings.mirror, slot.mirror);
                anim.Play(StaticStrings.parry_attack);
                lockOnTarget = null;
                return true;
            }
            return false;
        }

        bool CheckForBackStab(Action slot) {
            if(slot.canBackstab == false)
                return false;
            EnemyStates backstabTarget = null;
            Vector3 origin = transform.position;
            origin.y += 1;
            Vector3 rayDir = transform.forward;
            RaycastHit hit;
            if(Physics.Raycast(origin, rayDir, out hit, 1, ignoreLayers)) {
                backstabTarget = hit.transform.GetComponentInParent<EnemyStates>();
            }

            if(backstabTarget == null)
                return false;
            
            Vector3 dir = transform.position - backstabTarget.transform.position;
            dir.Normalize();
            dir.y = 0;
            float angle =  Vector3.Angle(backstabTarget.transform.forward, dir);

            if(angle > 150) {
                Vector3 targetPosition = dir * critAtkOffset;
                targetPosition += backstabTarget.transform.position;
                transform.position = targetPosition;


                backstabTarget.transform.rotation = transform.rotation;
                backstabTarget.BeingBackstabbed(slot);
                canMove = false;
                inAction = true;
                anim.SetBool(StaticStrings.mirror, slot.mirror);
                anim.Play(StaticStrings.parry_attack);
                lockOnTarget = null;
                return true;
            }
            return false;
        }

        void BlockAction(Action slot) {
            isBlocking = true;
            isLeftHand = slot.mirror;
        }

        void ParryAction(Action slot) {
            string targetAnim = null;
            targetAnim = slot.targetAnim;

            if (string.IsNullOrEmpty(targetAnim))
                return;

            float  targetSpeed = 1;
            if(slot.changeSpeed) {
                targetSpeed = slot.animSpeed;
                if(targetSpeed == 0)
                    targetSpeed = 1;
            }

            anim.SetFloat(StaticStrings.animSpeed, targetSpeed);
            
            canBeParried = slot.canBeParried;
            canMove = false;
            inAction = true;
            anim.SetBool(StaticStrings.mirror, slot.mirror);
            anim.Play(targetAnim);
        }

        public void Tick(float d)
        {
            delta = d;
            onGround = OnGround();
            anim.SetBool(StaticStrings.onGround, onGround);
        }
        
        void HandleRolls()
        {
            if (!rollInput || usingItem || !onGround)
                return;

            float v = vertical;
            float h = horizontal;
            v = (moveAmount > 0.3f) ? 1 : 0;
            h = 0;

            /* if(lockOn == false)
             {
                 v = (moveAmount > 0.3f) ? 1 : 0;
                 h = 0;
             } else
             {
                 if (Mathf.Abs(v) < 0.3f)
                     v = 0;
                 if (Mathf.Abs(h) < 0.3f)
                     h = 0;
             }*/



            if (v != 0)
            {
                if (moveDir == Vector3.zero)
                    moveDir = transform.forward;
                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                transform.rotation = targetRot;
                a_hook.InitForRoll();
                a_hook.rm_multi = rollSpeed;
            } else {
                a_hook.rm_multi = 1.3f;
            }
            anim.SetFloat(StaticStrings.vertical, v);
            anim.SetFloat(StaticStrings.horizontal, h);

            canMove = false;
            inAction = true;
            anim.Play(StaticStrings.Rolls);

        }

        void HandleMovementAnimations()
        {
            anim.SetBool(StaticStrings.run, run);
            anim.SetFloat(StaticStrings.vertical, moveAmount, 0.4f, delta);
        }

        void HandleLockOnAnimations(Vector3 moveDir)
        {
            Vector3 relativeDir = transform.InverseTransformDirection(moveDir);
            float h = relativeDir.x;
            float v = relativeDir.z;

            anim.SetFloat(StaticStrings.vertical, v, 0.2f, delta);
            anim.SetFloat(StaticStrings.horizontal, h, 0.2f, delta);
        }

        public bool OnGround()
        {
            bool r = false;

            Vector3 origin = transform.position + (Vector3.up * toGround);
            Vector3 dir = -Vector3.up;
            float dis = toGround + 0.3f;
            RaycastHit hit;
            if(Physics.Raycast(origin, dir, out hit, dis, ignoreLayers))
            {
                r = true;
                Vector3 targetPosition = hit.point;
                transform.position = targetPosition;
            }

            return r;
        }

        public void HandleTwoHanded()
        {
            bool isRight = true;
            Weapon w = inventoryManager.rightHandWeapon.instance;
            if(w == null){
                w = inventoryManager.leftHandWeapon.instance;
                isRight = false;
            }
            if(w == null) {
                return;
            }

            if(isTwoHanded){
                anim.CrossFade(w.th_idle, .2f);
                actionManager.UpdateActionsTwoHanded();
                if(isRight) {
                    if(inventoryManager.leftHandWeapon)
                    inventoryManager.leftHandWeapon.instance.weaponModel.SetActive(false);
                } else{
                    if(inventoryManager.rightHandWeapon)
                        inventoryManager.rightHandWeapon.instance.weaponModel.SetActive(false);
                }
              }
            else {
            string targetIdle = w.oh_idle;
                targetIdle += (isRight) ? StaticStrings._r : StaticStrings._l;
                anim.Play(StaticStrings.equipWeapon_oh);
                actionManager.UpdateActionsOneHanded();
                if(isRight) {
                    if(inventoryManager.leftHandWeapon)
                        inventoryManager.leftHandWeapon.instance.weaponModel.SetActive(true);
                } else {
                    if(inventoryManager.rightHandWeapon)
                        inventoryManager.rightHandWeapon.instance.weaponModel.SetActive(true);
                }
            }
        }

        public void BeingRiposted() {

        }

    }
}