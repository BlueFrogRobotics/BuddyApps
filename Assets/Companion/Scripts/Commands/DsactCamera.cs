using BuddyOS;
using BuddyOS.Command;

namespace BuddyApp.Companion
{
    internal class DsactCamera : ACommand
    {
        protected override void ExecuteImpl()
        {
            CompanionData.Instance.UseCamera = false;

            if (BYOS.Instance.RGBCam.IsOpen)
                BYOS.Instance.RGBCam.Close();
        }
    }
}