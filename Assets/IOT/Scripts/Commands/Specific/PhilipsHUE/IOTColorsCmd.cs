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
            Color lColor = new Color();
            switch (Parameters.Integers[0])
            {
                case 0:
                    lColor = Color.white;
                    break;
                case 1:
                    lColor = new Color(0.8F,0.2F,0.2F);
                    break;
                case 2:
                    lColor = new Color(0.8F, 0.2F, 0.5F);
                    break;
                case 3:
                    lColor = Color.blue;
                    break;
                case 4:
                    lColor = Color.cyan;
                    break;
                case 5:
                    lColor = new Color(0.0F, 0.8F, 0.1F);
                    break;
                case 6:
                    lColor = new Color(0.0F, 0.6F, 0.6F);
                    break;
                case 7:
                    lColor = Color.yellow;
                    break;
                case 8:
                    lColor = Color.red;
                    break;
            }
            ((IOTPhilipsLightHUE)Parameters.Objects[0]).SetColor(lColor);
        }
    }
}
