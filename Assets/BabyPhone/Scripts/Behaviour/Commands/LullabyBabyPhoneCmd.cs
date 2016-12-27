using UnityEngine;
using System.Collections;
using BuddyOS.Command;
using System;

namespace BuddyApp.BabyPhone
{
    public class LullabyBabyPhoneCmd : ACommand
    {

        protected override void ExecuteImpl()
        {
            BabyPhoneData.Instance.LullabyToPlay = (BabyPhoneData.Lullaby)Parameters.Objects[0];
        }
    }
}