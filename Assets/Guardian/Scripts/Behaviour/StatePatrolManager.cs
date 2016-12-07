using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS;
using BuddyOS.Command;


namespace BuddyApp.Guardian
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AudioSource))]
    public class StatePatrolManager : MonoBehaviour
    {


        [SerializeField]
        private DetectionManager detectorManager;

        [SerializeField]
        private BuddyFeature.Navigation.RoombaNavigation roomba;

        [SerializeField]
        private GameObject menu;

        [SerializeField]
        private ParametersGuardian parameters;

        [SerializeField]
        private GameObject loading;

        [SerializeField]
        private Text textCounter;

        [SerializeField]
        private GameObject backgroundPrefab;

        [SerializeField]
        private GameObject questionPrefab;

        [SerializeField]
        private GameObject haloPrefab;

        [SerializeField]
        private Animator backgroundAnimator;

        [SerializeField]
        private Animator questionAnimator;

        [SerializeField]
        private Animator haloAnimator;

        [SerializeField]
        private Animator loadingAnimator;

        [SerializeField]
        private Button cancelButton;

        [SerializeField]
        private Button validateButton;

        [SerializeField]
        private Image[] haloImages;

        [SerializeField]
        private Image icoMessage;

        [SerializeField]
        private Text messageText;

        [SerializeField]
        private Sprite[] listSpriteNotif;

        [SerializeField]
        private GameObject counterTime;

        [SerializeField]
        private GameObject objectButtonAskPassword;

        [SerializeField]
        private GameObject objectPasswordWriter;

        [SerializeField]
        private Button buttonValidatePassword;

        [SerializeField]
        private Button buttonCancelPassword;

        [SerializeField]
        private Dropdown dropListContact;

        [SerializeField]
        private NotifMail notifMail;

        [SerializeField]
        private DebugSoundWindow debugSoundWindow;

        [SerializeField]
        private DebugMovementWindow debugMovementWindow;

        [SerializeField]
        private ShowTemperature showTemperature;

        [SerializeField]
        private GameObject windowAppOverBuddyBlack;

        [SerializeField]
        private GameObject windowAppOverBuddyWhite;

        //private Face mFace;
        //private TextToSpeech mTTS;
        private Animator mAnimator;

        public DetectionManager DetectorManager { get { return detectorManager; } }
        public BuddyFeature.Navigation.RoombaNavigation Roomba { get { return roomba; } }
        public GameObject Menu { get { return menu; } }
        public ParametersGuardian Parameters { get { return parameters; } }
        public GameObject Loading { get { return loading; } }
        public Text TextCounter { get { return textCounter; } }
        public GameObject BackgroundPrefab { get { return backgroundPrefab; } }
        public GameObject QuestionPrefab { get { return questionPrefab; } }
        public GameObject HaloPrefab { get { return haloPrefab; } }
        public Animator BackgroundAnimator { get { return backgroundAnimator; } }
        public Animator QuestionAnimator { get { return questionAnimator; } }
        public Animator HaloAnimator { get { return haloAnimator; } }
        public Animator LoadingAnimator { get { return loadingAnimator; } }
        public Button CancelButton { get { return cancelButton; } }
        public Button ValidateButton { get { return validateButton; } }
        public Image[] HaloImages { get { return haloImages; } }
        public Image IcoMessage { get { return icoMessage; } }
        public Text MessageText { get { return messageText; } }
        public Sprite[] ListSpriteNotif { get { return listSpriteNotif; } }
        public GameObject CounterTime { get { return counterTime; } }
        public GameObject ObjectButtonAskPassword { get { return objectButtonAskPassword; } }
        public GameObject ObjectPasswordWriter { get { return objectPasswordWriter; } }
        public Button ButtonValidatePassword { get { return buttonValidatePassword; } }
        public Button ButtonCancelPassword { get { return buttonCancelPassword; } }
        public Dropdown DropListContact { get { return dropListContact; } }
        public NotifMail NotifMail { get { return notifMail; } }
        public DebugSoundWindow DebugSoundWindow { get { return debugSoundWindow; } }
        public DebugMovementWindow DebugMovementWindow { get { return debugMovementWindow; } }
        public ShowTemperature ShowTemperature { get { return showTemperature; } }
        public GameObject WindowAppOverBuddyBlack { get { return windowAppOverBuddyBlack; } }
        public GameObject WindowAppOverBuddyWhite { get { return windowAppOverBuddyWhite; } }

        //private GuardianData mGuardianData;
        void Awake()
        {
            // Find a reference to the Animator component in Awake since it exists in the scene.
            mAnimator = GetComponent<Animator>();
        }

        // Use this for initialization
        void Start()
        {
           // mGuardianData = GuardianData.Instance;
            
            //mFace = BYOS.Instance.Face;
            //mTTS = BYOS.Instance.TextToSpeech;
            AStateGuardian[] lStatesGuardian = mAnimator.GetBehaviours<AStateGuardian>();
            for (int i = 0; i < lStatesGuardian.Length; i++)
            {
                lStatesGuardian[i].StateManager = this;
            }

        }

        // Update is called once per frame
        void Update()
        {
            GuardianData mGuardianData;
            mGuardianData = GuardianData.Instance;
            //Debug.Log("is active: "+mGuardianData.FireDetectionIsActive);
        }

        void OnEnable()
        {
            BuddyFeature.Web.MailSender.OnMailSent += ShowNotifMailSent;
        }

        void OnDisable()
        {
            BuddyFeature.Web.MailSender.OnMailSent -= ShowNotifMailSent;
        }

        public void FixMode()
        {
            mAnimator.GetBehaviour<MenuPatrolState>().Mode = 1;
            Debug.Log("fix mode");
        }

        public void MobileMode()
        {
            mAnimator.GetBehaviour<MenuPatrolState>().Mode = 2;
        }


        public void QuitApplication()
        {
            //UnLoadAppCmd.Create().Execute();
            Debug.Log("exit magique");
            new HomeCmd().Execute();
        }


        public void ShowNotifMailSent()
        {
            detectorManager.SoundDetector.CanSave = true;
            notifMail.IncrementNumber();
            
            Debug.Log("mail a ete envoye");
        }

        public void DebugTest()
        {
            Debug.Log("machin");
        }

        public void PlayBeep()
        {
            AudioSource audio = GetComponent<AudioSource>();
            audio.Play();
        }
    }
}
