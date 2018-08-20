using BlueQuark;

namespace BuddyApp.Somfy
{
    public class SomfyLayout : AWindowLayout
    {
        private TextField mLogin;
        private TextField mPassword;

        public override void Build()
        {
            /*
             * Create needed widgets
             * ==> Which widget do I need for my app settings ?
             */
            mLogin = CreateWidget<TextField>();
            mPassword = CreateWidget<TextField>();

            /*
             * Set widgets parameters
             */
            //mLogin.OnEndEditEvent

            /*
             * Retrieve app data and display them inside the view
             * ==> What info must be displayed ?
             */
            mLogin.FieldText = SomfyData.Instance.Login;
            mPassword.FieldText = SomfyData.Instance.Password;

            /*
            * Set command to widgets
            * At each interaction with a widget, a callback will be called
            * ==> What must happen when I interacted with a widget ?
            */
            mLogin.OnUpdateEvent((iVal) => {
				SomfyData.Instance.Login = iVal;
			});

            mPassword.OnUpdateEvent((iVal) => {
                SomfyData.Instance.Password = iVal;
            });
        }

        public override void LabelizeWidgets()
        {
            mLogin.Label = "Login";
            mPassword.Label = "Password";
        }
    }
}