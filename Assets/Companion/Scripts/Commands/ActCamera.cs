using BuddyOS;
using BuddyOS.Command;

namespace BuddyApp.Companion
{
    internal class ActCamera : ACommand
    {
        protected override void ExecuteImpl()
        {
            bool lUseCamera = Parameters.Integers[0] == 1;
            CompanionData.Instance.UseCamera = lUseCamera;

            if (lUseCamera && !BYOS.Instance.RGBCam.IsOpen)
                BYOS.Instance.RGBCam.Open();
            else if (!lUseCamera && !BYOS.Instance.RGBCam.IsOpen)
                BYOS.Instance.RGBCam.Close();
        }
    }
}