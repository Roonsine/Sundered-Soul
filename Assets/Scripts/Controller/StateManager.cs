using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public class StateManager : MonoBehaviour
    {
        [Header("Init")]
        public GameObject activeModel;

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

        [Header("States")]
        public bool run;
        public bool onGround;
        public bool lockOn;
        public bool inAction;
        public bool canMove;
        public bool isTwoHanded;
        public bool usingItem;
        

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

        float _actionDelay;

        public void Init()
        {
            SetupAnimator();
            rigid = GetComponent<Rigidbody>();
            rigid.angularDrag = 999;
            rigid.drag = 4;
            rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            inventoryManager = GetComponent<InventoryManager>();
            inventoryManager.Init();

            actionManager = GetComponent<ActionManager>();
            actionManager.Init(this);

            a_hook = activeModel.AddComponent<AnimatorHook>();
            a_hook.Init(this);

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

            usingItem = anim.GetBool("interacting");



            DetectItemAction();
            DetectAction();
            inventoryManager.curWeapon.weaponModel.SetActive(!usingItem);

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
                moveAmount = Mathf.Clamp(moveAmount, 0, 0.3f);
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
            if(canMove == false || usingItem)
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
            string targetAnim = null;

            Action slot = actionManager.GetActionSlot(this);
            if(slot == null)
                return;
            targetAnim = slot.targetAnim;

            if (string.IsNullOrEmpty(targetAnim))
                return;

            inAction = true;
            anim.Play(targetAnim);
            //rigid.velocity = Vector3.zero;
        }

        public void Tick(float d)
        {
            delta = d;
            onGround = OnGround();
            anim.SetBool("onGround", onGround);
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
            anim.SetFloat("vertical", v);
            anim.SetFloat("horizontal", h);

            canMove = false;
            inAction = true;
            anim.Play("Rolls");

        }

        void HandleMovementAnimations()
        {
            anim.SetBool("run", run);
            anim.SetFloat("vertical", moveAmount, 0.4f, delta);
        }

        void HandleLockOnAnimations(Vector3 moveDir)
        {
            Vector3 relativeDir = transform.InverseTransformDirection(moveDir);
            float h = relativeDir.x;
            float v = relativeDir.z;

            anim.SetFloat("vertical", v, 0.2f, delta);
            anim.SetFloat("horizontal", h, 0.2f, delta);
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
            anim.SetBool("two_handed", isTwoHanded);

            if(isTwoHanded)
                actionManager.UpdateActionsTwoHanded();
            else
                actionManager.UpdateActionsOneHanded();
        }
    }
}