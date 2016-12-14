using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BuddyOS.Command;
using System;
namespace BuddyApp.IOT
{
    public class IOTDropdownCmd : ACommand
    {
        public IOTDropdownCmd(object iObject)
        {
            Parameters = new CommandParam();
            Parameters.Objects = new object[1] { iObject };
        }

        protected override void ExecuteImpl()
        {
            object[] lVal = (object[])Parameters.Objects[0];
            IOTNewDevice lVal0 = (IOTNewDevice)lVal[0];
            string lVal1 = (string)lVal[1];
            lVal0.IOTObject = (IOTObjects)Activator.CreateInstance(Type.GetType(lVal1));
            lVal0.FillParamClasses();
            lVal0.InitiliazeParameters();
        }
    }
}
