using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS {
    public class SpellItemsScriptableObject : ScriptableObject
    {
        public List<Spell> spellItems = new List<Spell>();
    }
}