using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public class Helper : MonoBehaviour
    {
        [Range(0, 1)]
        public float vertical;
        [Range(-1, 1)]
        public float horizontal;

        public bool playAnim = false;
        public string[] oh_attacks;
        public string[] th_attacks;

        public bool twoHanded;
        public bool enableRM;
        public bool useItem;
        public bool interacting;
        public bool lockon;

        Animator anim;
        void Start()
        {
            anim = GetComponent<Animator>();
            anim.applyRootMotion = false;
        }

        void Update()
        {
            enableRM = !anim.GetBool("can_move");
            anim.applyRootMotion = enableRM;

            interacting = anim.GetBool("interacting");

            if (lockon == false)
            {
                horizontal = 0;
                vertical = Mathf.Clamp01(vertical);
            }

            anim.SetBool("lockon", lockon);

            if(enableRM)
                return;

            if (useItem)
            {
                anim.Play("use_item");
                useItem = false;
            }

            if(interacting)
            {
                playAnim = false;
                vertical = Mathf.Clamp(vertical, 0, 0.5f);
            }

                anim.SetBool("two_handed", twoHanded);
            if (playAnim)
            {
                string targetAnim;
                if(twoHanded)
                {
                    int r = Random.Range(0, oh_attacks.Length);
                    targetAnim = th_attacks[r];
                }
                else
                {
                    int r = Random.Range(0, th_attacks.Length);
                    targetAnim = oh_attacks[r];
                }
                vertical = 0;
                anim.CrossFade(targetAnim, 0.2f);
                playAnim = false;
            }
            anim.SetFloat("vertical", vertical);
            anim.SetFloat("horizontal", horizontal);

        }
    }
}