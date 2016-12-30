using UnityEngine;
using System.Collections;
using BuddyOS.Command;
using System;

namespace BuddyApp.IOT
{
    public class IOTTempGaugeEcoCmd : ACommand
    {
        public IOTTempGaugeEcoCmd(IOTObjects iObject)
        {
            Parameters = new CommandParam();
            Parameters.Objects = new object[1] { iObject };
        }
        protected override void ExecuteImpl()
        {
            ((IOTSomfyThermostat)Parameters.Objects[0]).SetEcoTemperature(Parameters.Integers[0]/10F);
        }
    }
}