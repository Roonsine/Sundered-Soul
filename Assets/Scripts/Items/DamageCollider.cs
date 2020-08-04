﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SS {
    public class DamageCollider : MonoBehaviour
    {
        void OnTriggerEnter(Collider other) {
            EnemyStates eStates = other.transform.GetComponentInParent<EnemyStates>();

            if(eStates == null)
                return;
            
            eStates.DoDamage(50);
        }
    }
}