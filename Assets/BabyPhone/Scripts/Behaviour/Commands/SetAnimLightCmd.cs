using UnityEngine;
using System.Collections;
using BuddyOS.Command;

namespace BuddyApp.BabyPhone
{
    internal class SetAnimLightCmd : ACommand
    {
        protected override void ExecuteImpl()
        {
            BabyPhoneData.Instance.AnimationLight = Parameters.Integers[0];
        }
    }
}