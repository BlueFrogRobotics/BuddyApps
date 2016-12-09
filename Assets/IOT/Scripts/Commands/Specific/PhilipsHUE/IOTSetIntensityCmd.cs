using UnityEngine;
using System.Collections;
using BuddyOS.Command;
using System;

namespace BuddyApp.IOT
{
    public class IOTSetIntensityCmd : ACommand
    {
        public IOTSetIntensityCmd(IOTObjects iObject)
        {
            Parameters = new CommandParam();
            Parameters.Objects = new object[1] { iObject };
        }
        protected override void ExecuteImpl()
        {
            ((IOTPhilipsLightHUE)Parameters.Objects[0]).SetIntensity(Parameters.Singles[0]);
        }
    }
}