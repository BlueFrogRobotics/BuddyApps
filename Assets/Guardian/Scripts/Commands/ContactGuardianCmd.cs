using UnityEngine;
using System.Collections;
using BuddyOS.Command;
using System;

namespace BuddyApp.Guardian
{
    public class ContactGuardianCmd : ACommand
    {

        protected override void ExecuteImpl()
        {
            GuardianData.Instance.Recever = (GuardianData.Contact)Parameters.Objects[0];
        }
    }
}