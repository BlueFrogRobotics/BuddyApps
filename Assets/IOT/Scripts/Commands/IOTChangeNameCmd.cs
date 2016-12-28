using UnityEngine;
using System.Collections;
using BuddyOS.Command;
using System;

namespace BuddyApp.IOT
{
    public class IOTChangeNameCmd : ACommand
    {
        public IOTChangeNameCmd(IOTDevices iObject)
        {
            Parameters = new CommandParam();
            Parameters.Objects = new object[1] { iObject };
        }
        protected override void ExecuteImpl()
        {
            ((IOTDevices)Parameters.Objects[0]).ChangeName(Parameters.Strings[0]);
        }
    }
}
