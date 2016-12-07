using UnityEngine;
using System.Collections;
using BuddyOS.Command;

namespace BuddyApp.Guardian
{
    public class ActSoundDetectionCmd : ACommand
    {

        protected override void ExecuteImpl()
        {
            GuardianData.Instance.SoundDetectionIsActive = true;
        }
    }
}