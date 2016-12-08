using UnityEngine;
using System.Collections;
using BuddyOS.Command;
using System;

namespace BuddyApp.IOT
{
    public class IOTOnOff : ACommand
    {
        public IOTOnOff(IOTObjects iObject)
        {
            Parameters = new CommandParam();
            Parameters.Objects = new object[1] { iObject };
        }
        protected override void ExecuteImpl()
        {
            ((IOTPhilipsLightHUE)Parameters.Objects[0]).OnOff(Parameters.Singles[0] > 0 ? true : false);
        }
    }
}
