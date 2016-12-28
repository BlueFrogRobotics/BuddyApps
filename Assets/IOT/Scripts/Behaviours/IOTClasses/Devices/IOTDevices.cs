using UnityEngine;
using System.Collections;

namespace BuddyApp.IOT
{
    public class IOTDevices : IOTObjects
    {
        public enum DeviceType : int { DEVICE, LIGHT, SWITCH, STORE}
        protected DeviceType mType = DeviceType.DEVICE;
        public DeviceType Type { get { return mType; } }

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
    }
}
