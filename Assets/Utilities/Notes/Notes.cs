using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SS.Utilities{
    public class Notes : MonoBehaviour
    {
        [TextArea(15, 1000)]
        public string notes;
    }
}