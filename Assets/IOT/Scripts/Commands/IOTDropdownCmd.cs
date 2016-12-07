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
            Parameters.Objects = new object[1];
            Parameters.Objects[0] = iObject;
        }

        protected override void ExecuteImpl()
        {
            string lVal = Parameters.Strings[0];
            ((IOTNewDevice)Parameters.Objects[0]).IOTObject = (IOTObjects)Activator.CreateInstance(Type.GetType(lVal));
            ((IOTNewDevice)Parameters.Objects[0]).FillParamClasses();
            ((IOTNewDevice)Parameters.Objects[0]).InitiliazeParameters();
        }
    }
}
