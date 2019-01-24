using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.ExperienceCenter
{

    public class ParameterWindow : MonoBehaviour
    {
        private bool mIsShowing;
        private TTextField mUser;
        private TPasswordField mPassword;

        // Use this for initialization
        void Start()
        {
            mIsShowing = false;
            Buddy.GUI.Header.OnClickParameters.Add(OnClickParameters);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnDisable()
        {
            Buddy.GUI.Header.OnClickParameters.Remove(OnClickParameters);
        }

        private void ShowParameters()
        {
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) => {
                TText lTextStatus = iBuilder.CreateWidget<TText>();
                lTextStatus.SetLabel("Tcp status: " + ExperienceCenterData.Instance.StatusTcp);
                TText lTextIP = iBuilder.CreateWidget<TText>();
                lTextIP.SetLabel("ip: " + ExperienceCenterData.Instance.IPAddress);
                TText lTextState = iBuilder.CreateWidget<TText>();
                lTextState.SetLabel("state: " + ExperienceCenterData.Instance.Scenario);

                mUser = iBuilder.CreateWidget<TTextField>();
                mUser.SetText(ExperienceCenterData.Instance.UserID);
                mUser.OnEndEdit.Add(OnUser);

                mPassword = iBuilder.CreateWidget<TPasswordField>();
                mPassword.SetText(ExperienceCenterData.Instance.Password);
                mPassword.OnEndEdit.Add(OnPassword);
            });
        }

        private void OnClickParameters()
        {
            if (!mIsShowing)
                ShowParameters();
            else {
                mUser.OnEndEdit.Remove(OnUser);
                mPassword.OnEndEdit.Remove(OnPassword);
                Buddy.GUI.Toaster.Hide();
            }

            mIsShowing = !mIsShowing;
        }

        private void OnUser(string iText)
        {
            ExperienceCenterData.Instance.UserID = iText;
        }

        private void OnPassword(string iText)
        {
            ExperienceCenterData.Instance.Password = iText;
        }
    }
}