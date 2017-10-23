using Buddy.UI;

namespace BuddyApp.TakePhoto
{
    public class TakePhotoLayout : AWindowLayout
    {
		private OnOff mOverlay;
		
        public override void Build()
        {		
            /*
             * Create needed widgets
             * ==> Which widget do I need for my app settings ?
             */
            mOverlay = CreateWidget<OnOff>();

			mOverlay.IsActive = TakePhotoData.Instance.Overlay;



			mOverlay.OnSwitchEvent((bool iVal) => {
				TakePhotoData.Instance.Overlay = iVal;
			});
		}

        public override void LabelizeWidgets()
        {

			mOverlay.Label = "Overlay";
		}
    }
}