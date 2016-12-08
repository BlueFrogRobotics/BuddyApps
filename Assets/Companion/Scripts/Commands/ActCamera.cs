using BuddyOS;
using BuddyOS.Command;

namespace BuddyApp.Companion
{
    internal class ActCamera : ACommand
    {
        protected override void ExecuteImpl()
        {
            CompanionData.Instance.UseCamera = Parameters.Integers[0] == 1;

            if (!BYOS.Instance.RGBCam.IsOpen)
                BYOS.Instance.RGBCam.Open();
        }
    }
}