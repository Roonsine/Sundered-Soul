﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace SS{
    public static class StaticStrings
    {
        //inputs
        public static string Mouse_X = "Mouse X";
        public static string Mouse_Y = "Mouse Y";
        public static string RightAxis_y = "RightAxis Y";
        public static string RightAxis_X = "RightAxis X";
        public static string Vertical = "Vertical";
        public static string Horizontal = "Horizontal";
        public static string B = "B";
        public static string A = "A";
        public static string X = "X";
        public static string Y = "Y";
        public static string RT = "RT";
        public static string LT = "LT";
        public static string RB = "RB";
        public static string LB = "LB";
        public static string L = "L";
        public static string Pad_X = "Pad_x";
        public static string Pad_Y = "Pad_y";

        //Animator Parameters
        public static string vertical = "vertical";
        public static string horizontal = "horizontal";
        public static string mirror = "mirror";
        public static string parry_attack = "parry_attack";
        public static string animSpeed = "animSpeed";
        public static string onGround = "onGround";
        public static string run = "run";
        public static string two_handed = "two_handed";
        public static string interacting = "interacting";
        public static string blocking = "blocking";
        public static string isLeft = "isLeft";
        public static string canMove = "can_move";
        public static string lockon = "lockon";


        // Animator States
        public static string Rolls = "Rolls";
        public static string attack_interrupt = "attack_interrupt";
        public static string parry_recieved = "parry_recieved";
        public static string backstabbed = "backstabbed";
        public static string changeWeapon = "changeWeapon";
        public static string damage1 = "damage_1";
        public static string damage2 = "damage_2";
        public static string damage3 = "damage_3";
        public static string emptyBoth = "Empty Both";
        public static string emptyRight = "Empty Right";
        public static string emptyLeft = "Empty Left";
        public static string equipWeapon_oh = "equipWeapon_oh";

        //other
        public static string _l = "_l";
        public static string _r = "_r";

        //Data
        public static string itemFolder = "/Items/";
        
        public static string SaveLocation() {
            string r = Application.streamingAssetsPath;
            if(!Directory.Exists(r)){
                Directory.CreateDirectory(r);
            }
            return r;
        }
    }
}