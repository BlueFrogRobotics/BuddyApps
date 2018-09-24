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
    public sealed class RecipientChoiceState : AStateMachineBehaviour
    {
        //private GuardianLayout mDetectionLayout;
        private bool mHasSwitchState = false;

        //private Dictionary<string, string> mButtonContent = new Dictionary<string, string>();

        private GameObject mVerticalBox;

        private RecipientsData mContacts;

        private FButton mLeftButton;
        private FButton mValidateButton;

        private RecipientData mSelectedContact;


        public override void Start()
        {
            //mDetectionLayout = new GuardianLayout();
            mHasSwitchState = false;
            mVerticalBox = GetGameObject(5);
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("chooserecipient"));

            mContacts = Utils.UnserializeXML<RecipientsData>(Buddy.Resources.GetRawFullPath("contacts.xml"));

            if(GuardianData.Instance.ContactId!=-1 && GuardianData.Instance.ContactId<mContacts.Recipients.Count)
            {
                mSelectedContact = mContacts.Recipients[GuardianData.Instance.ContactId];
            }

            //Buddy.GUI.Toaster.Display<ParameterToast>().With(mDetectionLayout,
            //	() => { Trigger("NextStep"); }, 
            //	null);
            //mButtonContent.Clear();
            //mButtonContent.Add(Buddy.Resources.GetString("addrecipient"), "AddRecipient");
            //mButtonContent.Add("rodolphe", "SoundDetection");
            //mButtonContent.Add("maud", "SoundDetection");

            Buddy.GUI.Toaster.Display<VerticalListToast>().With((iBuilder) =>
            {
                TVerticalListBox lBox = iBuilder.CreateBox();
                //We create en event OnClick so we can trigger en event when we click on the box
                lBox.OnClick.Add(() => { Debug.Log("Click add"); iBuilder.Select(lBox); Trigger("AddRecipient"); Buddy.GUI.Toaster.Hide(); CloseFooter(); });
                //We label our button with our informations in the dictionary
                lBox.SetLabel(Buddy.Resources.GetString("addcontact"));
                lBox.LeftButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_user_circle"));
                lBox.LeftButton.SetIconColor(Color.black);
                lBox.LeftButton.SetBackgroundColor(Color.white);
                lBox.SetCenteredLabel(false);

                foreach(RecipientData contact in mContacts.Recipients)
                {
                    TVerticalListBox lBoxContact = iBuilder.CreateBox();
                    //We create en event OnClick so we can trigger en event when we click on the box
                    lBoxContact.OnClick.Add(() => { Debug.Log("Click add"); iBuilder.Select(lBoxContact); mSelectedContact = contact;/*Trigger(lButtonContent.Value); Buddy.GUI.Toaster.Hide();*/ });
                    //We label our button with our informations in the dictionary
                    lBoxContact.SetLabel(contact.Name, contact.Mail);
                    lBoxContact.LeftButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_user_circle"));
                    lBoxContact.LeftButton.SetIconColor(Color.black);
                    lBoxContact.LeftButton.SetBackgroundColor(Color.white);
                    lBoxContact.SetCenteredLabel(false);
                    if(mSelectedContact==contact)
                        iBuilder.Select(lBoxContact);
                }

                //foreach (KeyValuePair<string, string> lButtonContent in mButtonContent)
                //{
                //    //We create the container
                //    TVerticalListBox lBox = iBuilder.CreateBox();
                //    //We create en event OnClick so we can trigger en event when we click on the box
                //    lBox.OnClick.Add(() => { Debug.Log("Click " + lButtonContent.Key); iBuilder.Select(lBox); /*Trigger(lButtonContent.Value); Buddy.GUI.Toaster.Hide();*/ });
                //    //We label our button with our informations in the dictionary
                //    lBox.SetLabel(lButtonContent.Key, "rh@bluefrogrobotics.com");
                //    lBox.LeftButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_user_circle"));
                //    lBox.LeftButton.SetIconColor(Color.black);
                //    lBox.LeftButton.SetBackgroundColor(Color.white);
                //    //You can set a left button if you need to add en event or an icon at the left
                //    //lBox.LeftButton.Hide();
                //    //lBox.LeftButton.SetIcon(Buddy.Resources.Get<Sprite>("Fire_Alert"));
                //    //We place the text of the button in the center of the box
                //    lBox.SetCenteredLabel(false);
                //    //lBox.LeftButton.SetBackgroundColor(new Color(0.5f, 0.5f, 0.5f, 1F));
                //    //GameObject lVerticalBox = Instantiate(mVerticalBox);
                //    //iBuilder.AddCustomBox(lVerticalBox);
                //    //lVerticalBox.transform.GetChild(1).gameObject.SetActive(true);
                //    //lVerticalBox.GetComponent<TVerticalListBoxULink>().OnClick()(() => { Debug.Log("Click " + lButtonContent.Key); iBuilder.Select(lVerticalBox); });
                //}
            });

            mLeftButton = Buddy.GUI.Footer.CreateOnLeft<FButton>();

            mLeftButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_arrow_left"));

            mLeftButton.SetBackgroundColor(Color.white);
            mLeftButton.SetIconColor(Color.black);

            //lTrash.SetStroke(true);

            //lTrash.SetStrokeColor(Color.red);

            mLeftButton.OnClick.Add(() => { Trigger("GeneralParameters"); });



            mValidateButton = Buddy.GUI.Footer.CreateOnRight<FButton>();

            mValidateButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_check"));

            mValidateButton.SetBackgroundColor(Utils.BUDDY_COLOR);
            mValidateButton.SetIconColor(Color.white);

            //lButton.SetStroke(true);

            //lButton.SetStrokeColor(Utils.BUDDY_COLOR);

            mValidateButton.OnClick.Add(() => { SaveChoice(); Trigger("GeneralParameters"); });

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
            GuardianData.Instance.ContactId = mContacts.Recipients.IndexOf(mSelectedContact);

        }

        private void CloseFooter()
        {
            Buddy.GUI.Footer.Remove(mLeftButton);
            Buddy.GUI.Footer.Remove(mValidateButton);
        }

    }
}