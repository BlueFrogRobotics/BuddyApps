using BlueQuark;

using UnityEngine;

namespace BuddyApp.BabyPhone
{
    /// <summary>
    /// State that give the choice between the users to send the mail to
    /// </summary>
    public sealed class RecipientChoiceState : AStateMachineBehaviour
    {
        private bool mHasSwitchState = false;

        private RecipientsData mContacts;

        private FButton mLeftButton;
        private FButton mValidateButton;

        private RecipientData mSelectedContact;


        public override void Start()
        {
            mHasSwitchState = false;
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("choosecontact"));

            mContacts = Utils.UnserializeXML<RecipientsData>(Buddy.Resources.GetRawFullPath("contacts.xml"));

            if(BabyPhoneData.Instance.ContactId!=-1 && BabyPhoneData.Instance.ContactId<mContacts.Recipients.Count)
            {
                mSelectedContact = mContacts.Recipients[BabyPhoneData.Instance.ContactId];
            }

            Buddy.GUI.Toaster.Display<VerticalListToast>().With((iBuilder) =>
            {
                TVerticalListBox lBox = iBuilder.CreateBox();
                lBox.OnClick.Add(() => { Debug.Log("Click add"); iBuilder.Select(lBox); Trigger("AddRecipient"); Buddy.GUI.Toaster.Hide(); CloseFooter(); });
                lBox.SetLabel(Buddy.Resources.GetString("addcontact"));
                lBox.LeftButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_user_circle"));
                lBox.LeftButton.SetIconColor(Color.black);
                lBox.LeftButton.SetBackgroundColor(Color.white);
                lBox.SetCenteredLabel(false);


                foreach(RecipientData contact in mContacts.Recipients)
                {
                    TVerticalListBox lBoxContact = iBuilder.CreateBox();
                    lBoxContact.OnClick.Add(() => { Debug.Log("Click add"); iBuilder.Select(lBoxContact); mSelectedContact = contact; });
                    lBoxContact.SetLabel(contact.FirstName + " " + contact.LastName, contact.Mail);
                    lBoxContact.LeftButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_user_circle"));
                    lBoxContact.LeftButton.SetIconColor(Color.black);
                    lBoxContact.LeftButton.SetBackgroundColor(Color.white);
                    lBoxContact.SetCenteredLabel(false);
                    if(mSelectedContact==contact)
                        iBuilder.Select(lBoxContact);
                }
            });

            mLeftButton = Buddy.GUI.Footer.CreateOnLeft<FButton>();
            mLeftButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_arrow_left"));
            mLeftButton.SetBackgroundColor(Color.white);
            mLeftButton.SetIconColor(Color.black);
            mLeftButton.OnClick.Add(() => { Trigger("Contact"); });

            mValidateButton = Buddy.GUI.Footer.CreateOnRight<FButton>();
            mValidateButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_check"));
            mValidateButton.SetBackgroundColor(Utils.BUDDY_COLOR);
            mValidateButton.SetIconColor(Color.white);

            mValidateButton.OnClick.Add(() => { SaveChoice(); Trigger("Contact"); });

        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            CloseFooter();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Header.HideTitle();
        }

        private void SaveChoice()
        {
            BabyPhoneData.Instance.ContactId = mContacts.Recipients.IndexOf(mSelectedContact);

        }

        private void CloseFooter()
        {
            Buddy.GUI.Footer.Remove(mLeftButton);
            Buddy.GUI.Footer.Remove(mValidateButton);
        }

    }
}