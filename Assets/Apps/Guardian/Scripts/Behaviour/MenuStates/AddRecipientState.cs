using UnityEngine.UI;
using UnityEngine;
using BlueQuark;
using System;
using System.Collections.Generic;

namespace BuddyApp.Guardian
{
    /// <summary>
    /// State where the user can set the detection sensibility, test them and set the head orientation
    /// </summary>
    public sealed class AddRecipientState : AStateMachineBehaviour
    {
        //private GuardianLayout mDetectionLayout;
        private bool mHasSwitchState = false;

        private Dictionary<string, string> mButtonContent = new Dictionary<string, string>();

        private RecipientsData mContacts;
        private TTextField mFirstNameField;
        private TTextField mLastNameField;
        private TTextField mMailField;

        private string mFirstName;
        private string mLastName;
        private string mMail;


        public override void Start()
        {
            //mDetectionLayout = new GuardianLayout();
            mHasSwitchState = false;
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("addcontact"));

            mContacts = Utils.UnserializeXML<RecipientsData>(Buddy.Resources.GetRawFullPath("contacts.xml"));
            //Buddy.GUI.Toaster.Display<ParameterToast>().With(mDetectionLayout,
            //	() => { Trigger("NextStep"); }, 
            //	null);


            ShowToaster(false);


        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }

        private void AddAndQuit()
        {
            if (string.IsNullOrEmpty(mMail) || !mMail.Contains("@"))
            {
                Buddy.GUI.Toaster.Hide();
                ShowToaster(true);
            }
            else
            {
                RecipientData lRecipient = new RecipientData();
                lRecipient.FirstName = mFirstName;
                lRecipient.LastName = mLastName;
                lRecipient.Mail = mMail;
                mContacts.Recipients.Add(lRecipient);
                Utils.SerializeXML<RecipientsData>(mContacts, Buddy.Resources.GetRawFullPath("contacts.xml"));
                mFirstNameField.OnChangeValue.Clear();
                mMailField.OnChangeValue.Clear();
                Trigger("RecipientChoice");
                Buddy.GUI.Toaster.Hide();
            }
        }

        private void ShowToaster(bool iError)
        {
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) =>
            {
                //iBuilder.CreateWidget<TText>().SetLabel("test");
                mFirstNameField = iBuilder.CreateWidget<TTextField>();
                mFirstNameField.SetPlaceHolder(Buddy.Resources.GetString("firstname"));
                mFirstNameField.OnChangeValue.Add((iName) => { mFirstName = iName; });
                mFirstNameField.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_user"));

                mLastNameField = iBuilder.CreateWidget<TTextField>();
                mLastNameField.SetPlaceHolder(Buddy.Resources.GetString("lastname"));
                mLastNameField.OnChangeValue.Add((iName) => { mLastName = iName; });
                mLastNameField.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_user"));

                if(iError)
                    iBuilder.CreateWidget<TText>().SetLabel(Buddy.Resources.GetString("invalidemail"));

                mMailField = iBuilder.CreateWidget<TTextField>();
                mMailField.SetPlaceHolder(Buddy.Resources.GetString("email"));
                mMailField.OnChangeValue.Add((iMail) => { mMail = iMail; });
                mMailField.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_mail"));
                //iBuilder.CreateWidget<TText>().SetLabel("test2");
            },
            () => { Trigger("RecipientChoice"); Buddy.GUI.Toaster.Hide(); }, Buddy.Resources.GetString("cancel"),
            () => { AddAndQuit(); }, Buddy.Resources.GetString("add")
            );
        }

        //bool IsValidEmail(string iEmail)
        //{
        //    try
        //    {
        //        System.Net.Mail.MailAddress lAddr = new System.Net.Mail.MailAddress(iEmail);
        //        return lAddr.Address == iEmail;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
    }
}