using BuddyOS;

namespace BuddyApp.Remote
{
    public class JoystickOTOReceiver : OTONetReceiver
    {
        public override void ReceiveData(byte[] iData, int iNbData)
        {
            ACommand.Deserialize(iData).Execute();
        }
    }
}
