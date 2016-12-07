using UnityEngine;
using BuddyOS.Command;

namespace BuddyApp.Remote
{
    public class JoystickOTOReceiver : OTONetReceiver
    {
        public override void ReceiveData(byte[] iData, int iNbData)
        {
            ACommand lCmd = ACommand.Deserialize(iData);
            Debug.Log("Movement Command received : " + lCmd.ToString());
            lCmd.Execute();
            //new SetWheelsSpeedCmd(200F, -200F, 200).Execute();
        }
    }
}
