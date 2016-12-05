using UnityEngine;
using BuddyOS.Command;

namespace BuddyApp.Remote
{
    public class JoystickOTOReceiver : OTONetReceiver
    {
        public override void ReceiveData(byte[] iData, int iNbData)
        {
            ACommand lCmd = ACommand.Deserialize(iData);
            Debug.Log("Received command " + lCmd.Parameters.Strings);
            lCmd.Execute();
        }
    }
}
