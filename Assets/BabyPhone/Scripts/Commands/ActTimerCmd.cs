using UnityEngine;
using System.Collections;
using BuddyOS.Command;

namespace BuddyApp.BabyPhone
{
    internal class ActTimerCmd : ACommand
    {
        protected override void ExecuteImpl()
        {
            BabyPhoneData.Instance.TimerIsActive = true;
        }
    }
}