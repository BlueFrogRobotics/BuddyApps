using UnityEngine;
using BlueQuark;
using System.Collections;

namespace BuddyApp.Somfy
{
	public class SomfyParameterState : AStateMachineBehaviour {

        private TTextField mLoginField;
        private TTextField mPasswordField;

        private string mLogin;
        private string mPassword;

        public override void Start()
		{
		}
			
		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}
			
		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}

        private void ShowToaster()
        {
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) =>
            {
                //iBuilder.CreateWidget<TText>().SetLabel("test");
                mLoginField = iBuilder.CreateWidget<TTextField>();
                mLoginField.SetPlaceHolder(Buddy.Resources.GetString("login"));
                mLoginField.OnChangeValue.Add((iLogin) => { mLogin = iLogin; });
                mLoginField.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_user"));

                mPasswordField = iBuilder.CreateWidget<TTextField>();
                mPasswordField.SetPlaceHolder(Buddy.Resources.GetString("password"));
                mPasswordField.OnChangeValue.Add((iPassword) => { mPassword = iPassword; });
                mPasswordField.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_user"));

            },
            () => { Trigger("RecipientChoice"); Buddy.GUI.Toaster.Hide(); }, Buddy.Resources.GetString("cancel"),
            () => { AddAndQuit(); }, Buddy.Resources.GetString("add")
            );
        }

        private void AddAndQuit()
        {

        }
    }
}