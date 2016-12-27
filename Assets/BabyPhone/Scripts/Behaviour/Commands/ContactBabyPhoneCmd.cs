using UnityEngine;
using System.Collections;
using BuddyOS.Command;
using System;

namespace BuddyApp.BabyPhone
{
    public class ContactBabyPhoneCmd : ACommand
    {

        protected override void ExecuteImpl()
        {
            BabyPhoneData.Instance.Recever = (BabyPhoneData.Contact)Parameters.Objects[0];
        }
    }
}