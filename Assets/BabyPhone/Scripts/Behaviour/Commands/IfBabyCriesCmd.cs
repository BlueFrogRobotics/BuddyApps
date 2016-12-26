using UnityEngine;
using System.Collections;
using BuddyOS.Command;
using System;

namespace BuddyApp.BabyPhone
{
    public class IfBabyCriesCmd : ACommand
    {

        protected override void ExecuteImpl()
        {
            BabyPhoneData.Instance.ActionWhenBabyCries = (BabyPhoneData.Action)Parameters.Objects[0];
        }
    }
}