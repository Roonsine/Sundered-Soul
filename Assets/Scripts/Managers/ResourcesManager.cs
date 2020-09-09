using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS {
    public class ResourcesManager : MonoBehaviour
    {
        public List<Weapon> weaponList = new List<Weapon>();
        
        public static ResourcesManager singleton;
        void Awake() {
            singleton = this;
        }
    }
}