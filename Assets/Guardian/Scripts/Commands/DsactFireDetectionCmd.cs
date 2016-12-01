using UnityEngine;
using System.Collections;
using BuddyOS.Command;

namespace BuddyApp.Guardian
{
    public class DsactFireDetectionCmd : ACommand
    {
        protected override void ExecuteImpl()
        {
            GuardianData.Instance.FireDetectionIsActive = false;
        }
    }
}