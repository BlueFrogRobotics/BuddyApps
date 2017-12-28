using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy.UI;

namespace BuddyApp.Somfy
{
    public class IOTCommandsLayout : AWindowLayout
    {
        private List<IOTDevices> mDevices;

        public IOTCommandsLayout(List<IOTDevices> iDevices)
        {
            mDevices = iDevices;
        }

        public override void Build()
        {
           if(mDevices!=null && mDevices.Count>0)
            {
                foreach(IOTDevices device in mDevices)
                {
                    CreateWidget<OnOff>();
                }
            }
        }

        public override void LabelizeWidgets()
        {
        }

    }
}