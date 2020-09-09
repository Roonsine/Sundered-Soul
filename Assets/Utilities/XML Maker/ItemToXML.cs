using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SS.Utilities {

    [ExecuteInEditMode]
    public class ItemToXML : MonoBehaviour
    {
        public bool make;
        public List<ItemInstance> candidates = new List<ItemInstance>();
        public string xml_version;
        public string targetName;

        void Update() {
            if(!make)
                return;
            make = false;

            string xml = xml_version;
            xml += "\n";
            xml += "<root>";
            foreach (ItemInstance i in candidates)
            {
                Weapon w = i.instance;

                xml += "<Weapon>" + "\n";
                xml += "<WeaponName>" + w.weaponName + "</WeaponName>" + "\n";
                xml += "<Weapon_ID>" + w.weaponID + "</Weapon_ID>" + "\n";
                xml += "<oh_Idle>" + w.oh_idle + "</oh_Idle>" + "\n";
                xml += "<th_Idle>" + w.th_idle + "</th_Idle>" + "\n";

                xml += ActionListToString(w.actions, "actions");
                xml += ActionListToString(w.two_handedActions, "two_handed");

                xml += "<parryMultiplier>" + w.parryMultiplier + "</parryMultiplier>" + "\n";
                xml += "<backstabMultiplier>" + w.backstabMultiplier + "</backstabMultiplier>" + "\n";
                xml += "<LeftHandMirror>" + w.leftHandMirror + "</LeftHandMirror>" + "\n";
                xml += "<ModelPosition>" + w.model_pos + "</ModelPosition>" + "\n";
                xml += "<ModelEulers>" + w.model_eulers + "</ModelEulers>" + "\n";
                xml += "<ModelScale>" + w.model_scale + "</ModelScale>" + "\n";
                
                xml += "</Weapon>" + "\n";

            }
            xml += "</root>";

            string path = StaticStrings.SaveLocation() + StaticStrings.itemFolder;
            if(string.IsNullOrEmpty(targetName)){
                targetName = "items_database.xml";
            }
            path += targetName;

            File.WriteAllText(path, xml);
        }

        string ActionListToString(List<Action> l, string nodeName) {
            string xml = null;
            
                foreach (Action a in l)
                {
                    xml += "<" + nodeName + ">" + "\n";
                    xml += "<ActionInput>" + a.input.ToString() + "</ActionInput>" + "\n";
                    xml += "<ActionType>" + a.type.ToString() + "</ActionType>" + "\n";
                    xml += "<targetAnim>" + a.targetAnim + "</targetAnim>" + "\n";
                    xml += "<mirror>" + a.mirror + "</mirror>" + "\n";
                    xml += "<canBeParried>" + a.canBeParried + "</canBeParried>" + "\n";
                    xml += "<changeSpeed>" + a.changeSpeed + "</changeSpeed>" + "\n";
                    xml += "<animSpeed>" + a.animSpeed.ToString() + "</animSpeed>" + "\n";
                    xml += "<canRiposte>" + a.canRiposte + "</canRiposte>" + "\n";
                    xml += "<canBackstab>" + a.canBackstab + "</canBackstab>" + "\n";
                    xml += "<overrideDamageAnim>" + a.overrideDamageAnim + "</overrideDamageAnim>" + "\n";
                    xml += "<damageAnim>" + a.damageAnim + "</damageAnim>" + "\n";

                    WeaponStats s = a.weaponStats;
                    xml += "<Physical>" + s.physical + "</Physical>" + "\n";
                    xml += "<Strike>" + s.strike + "</Strike>" + "\n";
                    xml += "<Slash>" + s.slash + "</Slash>" + "\n";
                    xml += "<Thrust>" + s.thrust + "</Thrust>" + "\n";
                    xml += "<Magic>" + s.magic + "</Magic>" + "\n";
                    xml += "<Fire>" + s.fire + "</Fire>" + "\n";
                    xml += "<Lightning>" + s.lightning + "</Lightning>" + "\n";
                    xml += "<Dark>" + s.dark + "</Dark>" + "\n";

                    xml += "</" + nodeName + ">" + "\n";
                }
                return xml;
        }
        
    }
}