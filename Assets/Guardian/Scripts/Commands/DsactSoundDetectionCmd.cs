using UnityEngine;
using System.Collections;
using BuddyOS.Command;

namespace BuddyApp.Guardian
{
    public class DsactSoundDetectionCmd : ACommand
    {
        protected override void ExecuteImpl()
        {
            GuardianData.Instance.SoundDetectionIsActive = false;
        }
    }
}