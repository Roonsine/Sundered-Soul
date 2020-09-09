using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;


namespace SS.Utilities {
    [ExecuteInEditMode]
    public class XMLtoResources : MonoBehaviour
    {
        public bool load;

        public ResourcesManager resourcesManager;
        public string weaponFileName = "items_database.xml";

        void Update() {
            if(!load)
                return;
            
            load = false;
            
            LoadWeaponData(resourcesManager);
        }

        public void LoadWeaponData(ResourcesManager rm) {
            string filePath = StaticStrings.SaveLocation() + StaticStrings.itemFolder;
            filePath += weaponFileName;

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            foreach (XmlNode w in doc.DocumentElement.SelectNodes("//Weapon"))
            {
                Weapon _w = new Weapon();
                _w.actions = new List<Action>();
                _w.actions = new List<Action>();

                XmlNode weaponName = w.SelectSingleNode("WeaponName");
                _w.weaponName = weaponName.InnerText;
                XmlNode weaponId = w.SelectSingleNode("Weapon_ID");
                _w.weaponID = weaponId.InnerText;
                XmlNode oh_Idle = w.SelectSingleNode("oh_Idle");
                _w.oh_idle = oh_Idle.InnerText;
                XmlNode th_Idle = w.SelectSingleNode("th_Idle");
                _w.th_idle = th_Idle.InnerText;

                XmlToActions(doc, "actions", ref _w);
                XmlToActions(doc, "twoHanded", ref _w);

                XmlNode parryMultiplier = w.SelectSingleNode("parryMultiplier");
                float.TryParse(parryMultiplier.InnerText, out _w.parryMultiplier);
                XmlNode backstabMultiplier = w.SelectSingleNode("backstabMultiplier");
                float.TryParse(backstabMultiplier.InnerText, out _w.backstabMultiplier);
                XmlNode LeftHandMirror = w.SelectSingleNode("LeftHandMirror");
                _w.leftHandMirror = (LeftHandMirror.InnerText == "True");

                resourcesManager.weaponList.Add(_w);
            }
        }

        void XmlToActions(XmlDocument doc, string nodeName, ref Weapon _w) {
            foreach (XmlNode a in doc.DocumentElement.SelectNodes("//" + nodeName))
            {
                Action _a = new Action();
                XmlNode actionInput = a.SelectSingleNode("ActionInput");
                _a.input = (ActionInput) Enum.Parse(typeof(ActionInput), actionInput.InnerText);
                XmlNode actionType = a.SelectSingleNode("ActionType");
                _a.type = (ActionType) Enum.Parse(typeof(ActionType), actionType.InnerText);
                XmlNode targetAnim = a.SelectSingleNode("targetAnim");
                _a.targetAnim = targetAnim.InnerText;
                XmlNode mirror = a.SelectSingleNode("mirror");
                _a.mirror = (mirror.InnerText == "True");
                XmlNode canBeParried = a.SelectSingleNode("canBeParried");
                _a.canBeParried = (canBeParried.InnerText == "True");
                XmlNode changeSpeed = a.SelectSingleNode("changeSpeed");
                _a.changeSpeed = (changeSpeed.InnerText == "True");

                XmlNode animSpeed = a.SelectSingleNode("animSpeed");
                float.TryParse(animSpeed.InnerText, out _a.animSpeed);

                XmlNode canRiposte = a.SelectSingleNode("canRiposte");
                _a.canRiposte = (canRiposte.InnerText == "True");
                XmlNode canBackstab = a.SelectSingleNode("canBackstab");
                _a.canBackstab = (canBackstab.InnerText == "True");
                XmlNode overrideDamageAnim = a.SelectSingleNode("overrideDamageAnim");
                _a.overrideDamageAnim = (overrideDamageAnim.InnerText == "True");

                XmlNode damageAnim = a.SelectSingleNode("damageAnim");
                _a.damageAnim = damageAnim.InnerText;

                _a.weaponStats = new WeaponStats();

                XmlNode Physical = a.SelectSingleNode("Physical");
                int.TryParse(Physical.InnerText, out _a.weaponStats.physical);
                XmlNode Strike = a.SelectSingleNode("Strike");
                int.TryParse(Strike.InnerText, out _a.weaponStats.strike);
                XmlNode Slash = a.SelectSingleNode("Slash");
                int.TryParse(Slash.InnerText, out _a.weaponStats.slash);
                XmlNode Thrust = a.SelectSingleNode("Thrust");
                int.TryParse(Thrust.InnerText, out _a.weaponStats.thrust);
                XmlNode Magic = a.SelectSingleNode("Magic");
                int.TryParse(Magic.InnerText, out _a.weaponStats.magic);
                XmlNode Fire = a.SelectSingleNode("Fire");
                int.TryParse(Fire.InnerText, out _a.weaponStats.fire);
                XmlNode Lightning = a.SelectSingleNode("Lightning");
                int.TryParse(Lightning.InnerText, out _a.weaponStats.lightning);
                XmlNode Dark = a.SelectSingleNode("Dark");
                int.TryParse(Dark.InnerText, out _a.weaponStats.dark);

                if(nodeName == "actions"){
                    _w.actions.Add(_a);
                } else {
                    _w.two_handedActions.Add(_a);
                }
            }

        }
    }
}