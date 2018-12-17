using BlueQuark;

using UnityEngine;

namespace BuddyApp.Guardian
{
    /// <summary>
    /// State where the user can set the genaral parameters
    /// </summary>
    public sealed class GeneralParametersState : AStateMachineBehaviour
    {
        private bool mHasSwitchState = false;

        //private TToggle mToggleMobileGuard;
        //private TToggle mToggleMobileHead;
        private TToggle mToggleAlarm;
        private TToggle mToggleMailNotif;
        private TButton mButtonRecipient;

        private RecipientsData mContacts;


        public override void Start()
        {
            mHasSwitchState = false;
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mContacts = Utils.UnserializeXML<RecipientsData>(Buddy.Resources.GetRawFullPath("contacts.xml"));

            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("generalsettings"));
            
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) =>
            {
                //mToggleMobileGuard = iBuilder.CreateWidget<TToggle>();
                //mToggleMobileGuard.SetLabel(Buddy.Resources.GetString("mobile"));
                //mToggleMobileGuard.ToggleValue = GuardianData.Instance.MobileDetection;

                //mToggleMobileHead = iBuilder.CreateWidget<TToggle>();
                //mToggleMobileHead.SetLabel(Buddy.Resources.GetString("scan"));
                //mToggleMobileHead.ToggleValue = GuardianData.Instance.ScanDetection;

                mToggleAlarm = iBuilder.CreateWidget<TToggle>();
                mToggleAlarm.SetLabel(Buddy.Resources.GetString("alarm"));
                mToggleAlarm.ToggleValue = GuardianData.Instance.AlarmActivated;

                mToggleMailNotif = iBuilder.CreateWidget<TToggle>();
                mToggleMailNotif.SetLabel(Buddy.Resources.GetString("notification"));
                mToggleMailNotif.ToggleValue = GuardianData.Instance.SendMail;
                mToggleMailNotif.OnToggle.Add(OnToggleNotif);

                mButtonRecipient = iBuilder.CreateWidget<TButton>();

                if(GuardianData.Instance.ContactId != -1 && GuardianData.Instance.ContactId < mContacts.Recipients.Count)
                    mButtonRecipient.SetLabel(mContacts.Recipients[GuardianData.Instance.ContactId].FirstName + " " + mContacts.Recipients[GuardianData.Instance.ContactId].LastName);
                else
                    mButtonRecipient.SetLabel(Buddy.Resources.GetString("whotocontact"));

                mButtonRecipient.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_sort_down"));
                mButtonRecipient.OnClick.Add(() => { Trigger("RecipientChoice"); Buddy.GUI.Toaster.Hide(); });
                OnToggleNotif(mToggleMailNotif.ToggleValue);
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
            GuardianData.Instance.MobileDetection = false;// mToggleMobileGuard.ToggleValue;
            GuardianData.Instance.ScanDetection = false;// mToggleMobileHead.ToggleValue;
            GuardianData.Instance.SendMail = mToggleMailNotif.ToggleValue;
            GuardianData.Instance.AlarmActivated = mToggleAlarm.ToggleValue;
        }

        private void OnToggleNotif(bool iNotif)
        {
            mButtonRecipient.SetActive(iNotif);
        }

    }
}