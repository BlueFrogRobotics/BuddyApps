using UnityEngine;
using System.Collections;
using BuddyOS.Command;
using System;

namespace BuddyApp.IOT
{
    public class IOTOnOffCmd : ACommand
    {
        public IOTOnOffCmd(IOTObjects iObject)
        {
            Parameters = new CommandParam();
            Parameters.Objects = new object[1] { iObject };
        }
        protected override void ExecuteImpl()
        {
            ((IOTDevices)Parameters.Objects[0]).OnOff(Parameters.Integers[0] > 0 ? true : false);
        }
    }
}
