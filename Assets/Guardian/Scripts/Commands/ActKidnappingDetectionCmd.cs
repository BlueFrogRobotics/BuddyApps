using UnityEngine;
using System.Collections;
using BuddyOS.Command;

namespace BuddyApp.Guardian
{
    public class ActKidnappingDetectionCmd : ACommand
    {

        protected override void ExecuteImpl()
        {
            GuardianData.Instance.KidnappingDetectionIsActive = true;
        }
    }
}