using UnityEngine;
using System.Collections;
using BuddyOS.Command;

namespace BuddyApp.Guardian
{
    public class DsactKidnappingDetectionCmd : ACommand
    {

        protected override void ExecuteImpl()
        {
            GuardianData.Instance.KidnappingDetectionIsActive = false;
        }
    }
}