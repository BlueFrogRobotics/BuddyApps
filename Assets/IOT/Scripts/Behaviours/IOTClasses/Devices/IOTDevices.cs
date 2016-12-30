using UnityEngine;
using System.Collections;

namespace BuddyApp.IOT
{
    public class IOTDevices : IOTObjects
    {
        public enum DeviceType : int { DEVICE, LIGHT, SWITCH, STORE, THERMOSTAT}
        protected DeviceType mType = DeviceType.DEVICE;
        public DeviceType Type { get { return mType; } }

        public IOTDevices() : base()
        {
            mSpriteName = "IOT_Device_Big";
        }

        public virtual void ChangeName(string iName)
        {
            mName = iName;
        }

        public override void OnOff(bool iOnOff)
        {
        }

        public virtual void Command(int iCommand)
        {
            switch (iCommand)
            {
                case 0:
                    OnOff(false);
                    break;
                case 1:
                    OnOff(true);
                    break;
            }
        }

        public virtual void UpdateSlow()
        {

        }
    }
}
