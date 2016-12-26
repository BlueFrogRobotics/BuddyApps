using UnityEngine;
using System.Collections;
using BuddyOS.Command;

namespace BuddyApp.IOT
{
    public class IOTStoreCmd : ACommand
    {
        public IOTStoreCmd(object iObject, int iInt)
        {
            Parameters = new CommandParam();
            Parameters.Objects = new object[1] { iObject };
            Parameters.Integers = new int[1] { iInt };
        }
        protected override void ExecuteImpl()
        {
            ((IOTSomfyStore)Parameters.Objects[0]).Command(Parameters.Integers[0]);
        }
    }
}