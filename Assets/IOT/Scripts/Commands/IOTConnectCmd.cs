using UnityEngine;
using System.Collections;
using BuddyOS.Command;
using System;

namespace BuddyApp.IOT
{
    public class IOTConnectCmd : ACommand
    {
        public IOTConnectCmd(IOTObjects iObject)
        {
            Parameters = new CommandParam();
            Parameters.Objects = new object[1];
            Parameters.Objects[0] = iObject;
        }
        protected override void ExecuteImpl()
        {
            ((IOTObjects)Parameters.Objects[0]).Connect();
        }
    }
}
