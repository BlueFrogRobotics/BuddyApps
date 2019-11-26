using BlueQuark;

using UnityEngine;

namespace BuddyApp.BabyPhone
{
    /// <summary>
    /// State where the user can set the genaral parameters
    /// </summary>
    public sealed class GeneralParametersState : AStateMachineBehaviour
    {
        private bool mHasSwitchState = false;

        private TButton mButtonRecipient;
        private string mBabyName;

        private RecipientsData mContacts;


        public override void Start()
        {
            mHasSwitchState = false;
            mBabyName = BabyPhoneData.Instance.BabyName;
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mContacts = Utils.UnserializeXML<RecipientsData>(Buddy.Resources.GetRawFullPath("contacts.xml"));

            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("contact"));
            
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) =>
            {
                TTextField lField = iBuilder.CreateWidget<TTextField>();
                if (string.IsNullOrEmpty(mBabyName))
                    lField.SetPlaceHolder(Buddy.Resources.GetString("babysname"));
                else
                    lField.SetText(mBabyName);
                lField.OnChangeValue.Add((iVal) => {
                    mBabyName = iVal;
                });

                mButtonRecipient = iBuilder.CreateWidget<TButton>();

                if(BabyPhoneData.Instance.ContactId != -1 && BabyPhoneData.Instance.ContactId < mContacts.Recipients.Count)
                    mButtonRecipient.SetLabel(mContacts.Recipients[BabyPhoneData.Instance.ContactId].FirstName + " " + mContacts.Recipients[BabyPhoneData.Instance.ContactId].LastName);
                else
                    mButtonRecipient.SetLabel(Buddy.Resources.GetString("whotocontact"));

                mButtonRecipient.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_sort_down"));
                mButtonRecipient.OnClick.Add(() => { Trigger("RecipientChoice"); Buddy.GUI.Toaster.Hide(); });
            },
            () => { Trigger("Parameter"); Buddy.GUI.Toaster.Hide(); }, Buddy.Resources.GetString("cancel"),
            () => { SaveValues(); Trigger("Parameter");  Buddy.GUI.Toaster.Hide(); }, Buddy.Resources.GetString("save")
            );


        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.GUI.Header.HideTitle();
        }

        private void SaveValues()
        {
            //BabyPhoneData.Instance.SendMail = mToggleMailNotif.ToggleValue;
            BabyPhoneData.Instance.BabyName = mBabyName;
        }

    }
}