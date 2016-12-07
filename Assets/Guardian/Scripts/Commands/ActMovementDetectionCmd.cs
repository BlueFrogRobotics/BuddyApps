using UnityEngine;
using System.Collections;
using BuddyOS.Command;

namespace BuddyApp.Guardian
{
    public class ActMovementDetectionCmd : ACommand
    {

        protected override void ExecuteImpl()
        {
            GuardianData.Instance.MovementDetectionIsActive = true;
        }
    }
}