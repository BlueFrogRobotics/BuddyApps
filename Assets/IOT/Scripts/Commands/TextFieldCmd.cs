using UnityEngine;
using System.Collections;
using BuddyOS.Command;
using System;

namespace BuddyApp.IOT {
    public class TextFieldCmd : ACommand
    {
        protected override void ExecuteImpl()
        {
            string lVal = Parameters.Strings[0];
            int lIVal = Parameters.Integers[0];
            ((IOTObjects)Parameters.Objects[0]).Credentials[lIVal] = lVal;
        }
    }
}
