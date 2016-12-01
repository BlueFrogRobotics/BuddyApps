using UnityEngine;
using System.Collections;
using BuddyOS.Command;

namespace BuddyApp.Guardian
{
    public class DsactMovementDetectionCmd : ACommand
    {

        protected override void ExecuteImpl()
        {
            GuardianData.Instance.MovementDetectionIsActive = false;
        }
    }
}