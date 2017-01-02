using UnityEngine;
using System.Collections;
using BuddyOS.Command;

namespace BuddyApp.IOT
{
    public class IOTDeviceCmdCmd : ACommand
    {
        public IOTDeviceCmdCmd(object iObject, int iInt)
        {
            Parameters = new CommandParam();
            Parameters.Objects = new object[1] { iObject };
            Parameters.Integers = new int[1] { iInt };
        }

        public IOTDeviceCmdCmd(object iObject, int iInt, float iParam)
        {
            Parameters = new CommandParam();
            Parameters.Objects = new object[1] { iObject };
            Parameters.Integers = new int[1] { iInt };
            Parameters.Singles = new float[1] { iParam };
        }
        protected override void ExecuteImpl()
        {
            ((IOTDevices)Parameters.Objects[0]).Command(Parameters.Integers[0], Parameters.Singles[0]);
        }
    }
}