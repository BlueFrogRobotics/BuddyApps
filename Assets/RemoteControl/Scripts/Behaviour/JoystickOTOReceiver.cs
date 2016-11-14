using BuddyOS;


namespace BuddyApp.Remote
{
    public class JoystickOTOReceiver : OTONetReceiver
    {
        public override void ReceiveData(byte[] data, int ndata)
        {
            ACommand cmd = ACommand.Deserialize(data);
            cmd.Execute();
        }
    }
}
