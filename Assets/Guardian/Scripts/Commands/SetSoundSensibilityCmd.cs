using UnityEngine;
using System.Collections;
using BuddyOS.Command;

namespace BuddyApp.Guardian
{
    public class SetSoundSensibilityCmd : ACommand
    {

        protected override void ExecuteImpl()
        {
            GuardianData.Instance.SoundSensibility = Parameters.Integers[0];
        }
    }
}