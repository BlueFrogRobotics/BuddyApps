using UnityEngine;
using System.Collections;
using BuddyOS;
using BuddyOS.Command;

namespace BuddyApp.Guardian
{
    public class ActFireDetectionCmd : ACommand
    {
        protected override void ExecuteImpl()
        {
            GuardianData.Instance.FireDetectionIsActive = Parameters.Integers[0] == 1;
        }
    }
}