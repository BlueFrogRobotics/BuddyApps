using UnityEngine;
using System.Collections;
using BuddyOS.Command;
using System;

namespace BuddyApp.IOT
{
    public class IOTTempGaugeCmd : ACommand
    {
        public IOTTempGaugeCmd(IOTObjects iObject)
        {
            Parameters = new CommandParam();
            Parameters.Objects = new object[1] { iObject };
        }
        protected override void ExecuteImpl()
        {
            ((IOTSomfyThermostat)Parameters.Objects[0]).SetTemperature(Parameters.Integers[0]/10F);
        }
    }
}