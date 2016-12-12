using UnityEngine;
using System.Collections;
using BuddyOS.Command;
using System;

namespace BuddyApp.IOT {
    public class IOTCredentialTextFieldCmd : ACommand
    {
        public IOTCredentialTextFieldCmd(IOTObjects iObject, int lIndex, string lString)
        {
            Parameters = new CommandParam();
            Parameters.Strings = new string[1] { lString };
            Parameters.Integers = new int[1] { lIndex };
            Parameters.Objects = new object[1] { iObject };
        }
        protected override void ExecuteImpl()
        {
            string lVal = Parameters.Strings[0];
            int lIVal = Parameters.Integers[0];
            ((IOTObjects)Parameters.Objects[0]).Credentials[lIVal] = lVal;
        }
    }
}
