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
    public class AddRecipientState : AStateMachineBehaviour
    {
        //private GuardianLayout mDetectionLayout;
        private bool mHasSwitchState = false;

        private Dictionary<string, string> mButtonContent = new Dictionary<string, string>();

        private RecipientsData mContacts;
        private TTextField mNameField;
        private TTextField mMailField;

        private string mName;
        private string mMail;


        public override void Start()
        {
            //mDetectionLayout = new GuardianLayout();
            mHasSwitchState = false;
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

            mContacts = Utils.UnserializeXML<RecipientsData>(Buddy.Resources.GetRawFullPath("contacts.xml"));
            //Buddy.GUI.Toaster.Display<ParameterToast>().With(mDetectionLayout,
            //	() => { Trigger("NextStep"); }, 
            //	null);
            

            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) =>
            {
                //iBuilder.CreateWidget<TText>().SetLabel("test");
                mNameField = iBuilder.CreateWidget<TTextField>();
                mNameField.SetPlaceHolder("enter recipient name");
                mNameField.OnChangeValue.Add((iName) => { mName = iName; });
                mMailField = iBuilder.CreateWidget<TTextField>();
                mMailField.SetPlaceHolder("enter recipient mail");
                mMailField.OnChangeValue.Add((iMail) => { mMail = iMail; });
                //iBuilder.CreateWidget<TText>().SetLabel("test2");
            },
            () => { Trigger("RecipientChoice"); Buddy.GUI.Toaster.Hide(); }, "Cancel",
            () => { AddAndQuit(); }, "Next"
            );


        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }

        private void AddAndQuit()
        {
            RecipientData lRecipient = new RecipientData();
            lRecipient.Name = mName;
            lRecipient.Mail = mMail;
            mContacts.Recipients.Add(lRecipient);
            Utils.SerializeXML<RecipientsData>(mContacts, Buddy.Resources.GetRawFullPath("contacts.xml"));
            mNameField.OnChangeValue.Clear();
            mMailField.OnChangeValue.Clear();
            Trigger("RecipientChoice");
            Buddy.GUI.Toaster.Hide();
        }
    }
}