using UnityEngine;
using System.Collections;
using BuddyOS.Command;
using System;

namespace BuddyApp.BabyPhone
{
    public class AnimBabyPhoneCmd : ACommand
    {

        protected override void ExecuteImpl()
        {
            BabyPhoneData.Instance.AnimationToPlay = (BabyPhoneData.Animation)Parameters.Objects[0];
        }
    }
}