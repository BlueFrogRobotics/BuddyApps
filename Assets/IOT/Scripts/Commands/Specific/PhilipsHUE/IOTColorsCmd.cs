using UnityEngine;
using System.Collections;
using BuddyOS.Command;
using System;

namespace BuddyApp.IOT
{
    public class IOTColorsCmd : ACommand
    {
        public IOTColorsCmd(IOTObjects iObject)
        {
            Parameters = new CommandParam();
            Parameters.Objects = new object[1] { iObject };
        }
        protected override void ExecuteImpl()
        {
            ((IOTPhilipsLightHUE)Parameters.Objects[0]).SetColor(Parameters.Integers[0]);
        }
    }
}
