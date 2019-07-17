using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;
using OpenCVUnity;
using System.Timers;

namespace BuddyApp.Diagnostic
{
    public sealed class VocalWindow : MonoBehaviour
    {
        public enum TAB
        {
            TRIGGER,
            LOCALIZATION,
            BEAMFORMING,
            ECHO_CANCELLATION,
            COUNT,
        }

        [SerializeField]
        private Slider VolumeSlider;

        [SerializeField]
        private Text VolumeSliderText;

        [SerializeField]
        private InputField TextToSpeechInputField;

        [SerializeField]
        private Button TextToSpeechButton;

        [SerializeField]
        private Dropdown GimmickDropdown;

        [SerializeField]
        private Button GimmickPlayButton;

        [SerializeField]
        private Button PlayMusic;

        [SerializeField]
        private Text PlayMusicText;

        [SerializeField]
        private AudioClip AudioClipMusic;

        [SerializeField]
        private Toggle SpeechToTextFreeSpeechButton;

        [SerializeField]
        private Toggle SpeechToTextGrammarButton;

        [SerializeField]
        private Toggle SpeechToHybrid;

        [SerializeField]
        private Animator TriggerText;

        [SerializeField]
        private Text TriggerScore;

        [SerializeField]
        private Text LocalizationText;

        [SerializeField]
        private GameObject LocalizationRad;

        [SerializeField]
        private Text AmbiantSoundLevelText;

        [SerializeField]
        private Slider AmbiantSoundLevelSlider;

        [SerializeField]
        private Dropdown RecordDropdown;

        [SerializeField]
        private Button ResetList;

        [SerializeField]
        private Button RecordButton;

        [SerializeField]
        private Button PlayRecordButton;

        [SerializeField]
        private AudioSource ReplayAudioSource;

        [Header("TAB MANAGE")]
        [SerializeField]
        private Button[] TabButton = new Button[(int)TAB.COUNT];

        [SerializeField]
        private GameObject[] TabContent = new GameObject[(int)TAB.COUNT];

        [Header("TRIGGER TAB")]
        [SerializeField]
        private Toggle TriggerToggle;

        [SerializeField]
        private Slider TriggerTreshSlider;

        [SerializeField]
        private Text TriggerTreshText;

        [SerializeField]
        private Dropdown TriggerDropdown;

        [Header("LOCALIZATION TAB")]
        [SerializeField]
        private Toggle LocalizationToggle;

        [SerializeField]
        private Slider LocalizationTreshSlider;

        [SerializeField]
        private Text LocalizationTreshText;

        [Header("BEAMFORMING TAB")]
        [SerializeField]
        private Toggle BeamFormingToggle;
        [SerializeField]
        private Dropdown BeamFormingDropDown;
        [SerializeField]
        private GameObject BeamFormingText;

        [SerializeField]
        private List<GameObject> Circles;


        [Header("ECHO CANCELLATION TAB")]
        [SerializeField]
        private Toggle EchoCancellationToggle;
        [SerializeField]
        private GameObject EchoCancellationText;

        private readonly Color TAB_IDLE_COLOR = new Color(221F / 255F, 221F / 255F, 221F / 255F);
        private readonly Color STATUS_OFF_COLOR = new Color(56F / 255F, 56F / 255F, 56F / 255F);
        private readonly Color STATUS_ON_COLOR = new Color(0F, 212F / 255F, 209F / 255F);
        private readonly Color STATUS_OFF_RED_COLOR = new Color(1F, 0F, 0F);

        private const string CREDENTIAL_DEFAULT_URL = "http://bfr-dev.azurewebsites.net/dev/BuddyDev-cmfc3b05c071.txt";


        private Sprite mRecord;
        private Sprite mStop;
        private Sprite mPlay;
        private Sprite mStopBig;
        private Sprite mPlayBig;
        private Color mBuddyBlue = new Color(0.0f, 0.831f, 0.819f);
        private Color mWhite = new Color(1f, 1f, 1f);

        private Timer mTimeTrigger;
        private NoiseDetector mNoiseDetector;

        private bool mBIsPlaying = false;

        // List of all recorded audio
        private List<Queue<AudioClip>> mListRecorded = new List<Queue<AudioClip>>();

        // Last audio recorded
        private Queue<AudioClip> mListAudio = new Queue<AudioClip>();

        private AudioClip mAudioClip;

        private int mIPreviousMicroIndex = 0;

        private Image mSoundLocField;
        private int mSoundLocAngle;
        private float mSoundLocPreviousTreatedAngle;
        private int mDirectionValue;

        public void OnClickTabs(TAB iClickedTab)
        {
            if (iClickedTab < 0 && iClickedTab >= TAB.COUNT)
                return;
            if (TabContent[(int)iClickedTab].activeSelf)
                return;
            for (TAB iTab = 0; iTab < TAB.COUNT; iTab++)
            {
                TabContent[(int)iTab].SetActive(false);
                TabButton[(int)iTab].GetComponent<Image>().color = TAB_IDLE_COLOR;
            }
            TabContent[(int)iClickedTab].SetActive(true);
            TabButton[(int)iClickedTab].GetComponent<Image>().color = Color.white;
        }

        private void Start()
        {
            mDirectionValue = 1;
            mSoundLocPreviousTreatedAngle = 0F;
            mSoundLocField = LocalizationRad.GetComponent<Image>();
            mRecord = Buddy.Resources.Get<Sprite>("os_icon_micro_on");
            mStop = Buddy.Resources.Get<Sprite>("os_icon_stop");
            mPlay = Buddy.Resources.Get<Sprite>("os_icon_play");
            mStopBig = Buddy.Resources.Get<Sprite>("os_icon_stop_big");
            mPlayBig = Buddy.Resources.Get<Sprite>("os_icon_play_big");
            SpeechToTextFreeSpeechButton.interactable = true;

            TriggerTreshSlider.wholeNumbers = true;
            TriggerTreshSlider.minValue = 0F;
            TriggerTreshSlider.maxValue = 200;
            TriggerTreshSlider.value = 80;
            TriggerTreshText.text = (TriggerTreshSlider.value * 10).ToString();
            TriggerTreshSlider.onValueChanged.AddListener((iInput) => OnSliderThresholdChange(iInput));

            LocalizationTreshSlider.wholeNumbers = true;
            LocalizationTreshSlider.minValue = 10F;
            LocalizationTreshSlider.maxValue = 128F;
            LocalizationTreshSlider.value = 40F;
            LocalizationTreshText.text = LocalizationTreshSlider.value.ToString();
            LocalizationTreshSlider.onValueChanged.AddListener((iInput) => OnChangeThresholdLocalization(iInput));

            TriggerDropdown.onValueChanged.AddListener((iInput) => OnSearchTriggerOption(iInput));

            BeamFormingDropDown.onValueChanged.AddListener((iInput) => OnDirectionBeam(iInput));

            StartCoroutine(GetFreespeechCredentials());
        }

        // Manage warning text
        private void OnToggleActivated()
        {

            Debug.Log("OnToggleActivated");

            if(EchoCancellationToggle.isOn && TabContent[2].activeSelf)
            {
                BeamFormingText.SetActive(true);
                Debug.Log("Active BeamForming Warning");
            }
            else if (BeamFormingToggle.isOn && TabContent[3].activeSelf)
            {
                EchoCancellationText.SetActive(true);
                Debug.Log("Active Echo Cancel Warning");
            }
            else 
            {
                Debug.Log("DisActive both Cancel Warning");
                EchoCancellationText.SetActive(false);
                BeamFormingText.SetActive(false);
            }
        }
       
        private void OnChangeThresholdLocalization(float iInput)
        {
            
            byte lReso = Buddy.Sensors.Microphones.SoundLocalizationParameters.Resolution;
            LocalizationTreshText.text = FloatToInt(iInput).ToString();
            Buddy.Sensors.Microphones.SoundLocalizationParameters = new SoundLocalizationParameters(lReso, FloatToInt(iInput));
        }

        #region TabTrigger
        private void OnSliderThresholdChange(float iInput)
        {
            TriggerTreshText.text = FloatToShort(iInput*10).ToString();
            Buddy.Sensors.Microphones.VocalTriggerParameters = new VocalTriggerParameters(FloatToShort(iInput*10));

        }

        private void OnSearchTriggerOption(int iInput)
        {
            int lSearchValue = 0;
            switch (iInput)
            {
                case 0:
                    lSearchValue = 1;
                    break;
                case 1:
                    lSearchValue = 4;
                    break;
                case 2:
                    lSearchValue = 7;
                    break;
                case 3:
                    lSearchValue = 10;
                    break;
                case 4:
                    lSearchValue = 13;
                    break;
                case 5:
                    lSearchValue = 16;
                    break;
                case 6:
                    lSearchValue = 18;
                    break;
                default:
                    lSearchValue = 1;
                    break;
            }
            Buddy.Sensors.Microphones.VocalTriggerParameters = new VocalTriggerParameters(FloatToShort(TriggerTreshSlider.value),IntToShort(lSearchValue));
        }
        #endregion

        #region TabBeamforming
        private void OnDirectionBeam(int iInput)
        {
            foreach (GameObject img in Circles)
            {
                img.GetComponentsInChildren<Image>()[1].color = STATUS_OFF_RED_COLOR;
            }
           
            switch (iInput)
            {
                case 0:
                    mDirectionValue = 1;
                    break;
                case 1:
                    mDirectionValue = 2;
                    break;
                case 2:
                    mDirectionValue = 3;
                    break;
                case 3:
                    mDirectionValue = 4;
                    break;
                case 4:
                    mDirectionValue = 5;
                    break;
                case 5:
                    mDirectionValue = 6;
                    break;
                case 6:
                    mDirectionValue = 7;
                    break;
                case 7:
                    mDirectionValue = 8;
                    break;
                default:
                    mDirectionValue = 1;
                    break;
            }
            Circles[mDirectionValue-1].GetComponentsInChildren<Image>()[1].color = STATUS_ON_COLOR;
            Buddy.Sensors.Microphones.BeamformingParameters = new BeamformingParameters(IntToByte(mDirectionValue));
        }
        #endregion

        private byte IntToByte(int iInt)
        {
            return System.Convert.ToByte(iInt);
        }

        private short FloatToShort(float iFloat)
        {
            return System.Convert.ToInt16(iFloat);
        }

        private short IntToShort(int iInt)
        {
            return System.Convert.ToInt16(iInt);
        }

        private int FloatToInt(float iFloat)
        {
            return System.Convert.ToInt32(iFloat);
        }

        private void UpdateSoundLocalization()
        {
            AmbiantSoundLevelText.text = Buddy.Sensors.Microphones.AmbiantSound + " db";
            AmbiantSoundLevelSlider.value = Buddy.Sensors.Microphones.AmbiantSound;
            mSoundLocAngle = Buddy.Sensors.Microphones.SoundLocalization;
            // Done only for positive angles (as SLOC output angles)
            if (mSoundLocAngle >= 0)
            {
                LocalizationRad.SetActive(true);
                // Display untreated value
                LocalizationText.text = mSoundLocAngle.ToString() + " °";
                mSoundLocField.rectTransform.Rotate(0, 0, Buddy.Sensors.Microphones.SoundLocalization - mSoundLocPreviousTreatedAngle);
                mSoundLocPreviousTreatedAngle = Buddy.Sensors.Microphones.SoundLocalization;

                // Following code was supplied in the ECR_190218_BF_STUDIO_01 : (And then adapted here)
                // Adapt angle to get it in radians and 0° on left of buddy head 
                //float lAngle = mSoundLocAngle;  // angle is [0..+360[ 
                //lAngle -= 135;                  // angle is [-135..+225] 
                //if (lAngle < 0)
                //    lAngle = lAngle + 360;      // angle is [0..+360[
                //Debug.LogWarning("---- Sloc:" + mSoundLocAngle + " / Treated:" + lAngle + " / diff with previous to rotate:" + (lAngle - mSoundLocPreviousTreatedAngle) + " ----");
                //mSoundLocField.rectTransform.Rotate(0, 0, (lAngle - mSoundLocPreviousTreatedAngle));
                //mSoundLocPreviousTreatedAngle = lAngle;
                // This code is not necessary, we can directly rotate the object using transform
                //Angle = Angle * (float)Math.PI / 180.0F;    // Convert in rad 
                //x = centerX + radius * (float)Math.Cos(Angle);
                //y = centerY + radius * (float)Math.Sin(Angle);
            }
            else if (LocalizationRad.activeSelf)
            {
                // No value compute - disable UI
                LocalizationText.text = "0 °";
                LocalizationRad.SetActive(false);
            }
        }

        public void Update()
        {
            
            UpdateSoundLocalization();
            if (mBIsPlaying) // Playing record
            {
                if (null == ReplayAudioSource.clip || !ReplayAudioSource.isPlaying)
                {
                    ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "Record - Should start playing...");

                    if (mListAudio != null && mListAudio.Count > 0)
                    {
                        ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "Record - There is " + mListAudio.Count + " clip(s) to play...");
                        ReplayAudioSource.clip = mListAudio.Dequeue();
                        ReplayAudioSource.Play();
                    }
                    else
                    {
                        ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "Record - No clip to play...");
                        ReplayAudioSource.clip = null;
                        mListAudio.Clear();
                        OnPlayRecordButtonClick();
                    }
                }
            }
        }

        public void OnEnable()
        {
            // Tabs
            TabButton[(int)TAB.TRIGGER].onClick.AddListener(() => { OnClickTabs(TAB.TRIGGER); });
            TabButton[(int)TAB.LOCALIZATION].onClick.AddListener(() => { OnClickTabs(TAB.LOCALIZATION); });
            TabButton[(int)TAB.BEAMFORMING].onClick.AddListener(() => { OnClickTabs(TAB.BEAMFORMING); });
            TabButton[(int)TAB.ECHO_CANCELLATION].onClick.AddListener(() => { OnClickTabs(TAB.ECHO_CANCELLATION); });

            foreach (GameObject img in Circles)
            {
                img.GetComponentsInChildren<Image>()[1].color = STATUS_OFF_RED_COLOR;
            }

            List<GameObject> lContentsStart = new List<GameObject>();
            int lChildCounts = TabContent[(int)TAB.BEAMFORMING].transform.childCount;
            for (int i = 0; i < lChildCounts; i++)
                lContentsStart.Add(TabContent[(int)TAB.BEAMFORMING].transform.GetChild(i).gameObject);
            TabButton[(int)TAB.ECHO_CANCELLATION].GetComponentsInChildren<Image>()[2].color = STATUS_OFF_COLOR;
            TabButton[(int)TAB.BEAMFORMING].GetComponent<CanvasGroup>().alpha = 1F;
            foreach (GameObject lContent in lContentsStart)
                lContent.SetActive(false);
            lContentsStart[1].SetActive(true);


            // Trigger
            TriggerToggle.onValueChanged.AddListener((iValue) =>
            {
                Buddy.Vocal.EnableTrigger = iValue;
                if (iValue)
                    TabButton[(int)TAB.TRIGGER].GetComponentsInChildren<Image>()[2].color = STATUS_ON_COLOR;
                else
                    TabButton[(int)TAB.TRIGGER].GetComponentsInChildren<Image>()[2].color = STATUS_OFF_COLOR;
            });
            //TriggerTreshSlider.value = 1000;
            //TriggerTreshText.text = TriggerTreshSlider.value.ToString();
            //TriggerTreshSlider.onValueChanged.AddListener((iTresh) =>
            //{
            //    TriggerTreshText.text = TriggerTreshSlider.value.ToString();
            //    // -- ADD Treshold setting when available ---
            //});

            // Localization
            LocalizationToggle.onValueChanged.AddListener((iValue) =>
            {
                Buddy.Sensors.Microphones.EnableSoundLocalization = iValue;
                Debug.LogWarning("SOUNDLOC : " + Buddy.Sensors.Microphones.EnableSoundLocalization);
                if (iValue)
                    TabButton[(int)TAB.LOCALIZATION].GetComponentsInChildren<Image>()[2].color = STATUS_ON_COLOR;
                
                else
                    TabButton[(int)TAB.LOCALIZATION].GetComponentsInChildren<Image>()[2].color = STATUS_OFF_COLOR;
                    
                    
            });
            //LocalizationTreshSlider.value = 50;
            //LocalizationTreshText.text = LocalizationTreshSlider.value.ToString();
            //LocalizationTreshSlider.onValueChanged.AddListener((iTresh) =>
            //{
            //    LocalizationTreshText.text = LocalizationTreshSlider.value.ToString();
            //    // -- ADD Treshold setting when available ---
            //});

            // BeamForming
            BeamFormingToggle.onValueChanged.AddListener((iValue) =>
            {
                // Get all child gameobject of EchoCancellation for mutual exclusion
                List<GameObject> lContents = new List<GameObject>();
                int lChildCount = TabContent[(int)TAB.ECHO_CANCELLATION].transform.childCount;
                for (int i = 0; i < lChildCount; i++)
                    lContents.Add(TabContent[(int)TAB.ECHO_CANCELLATION].transform.GetChild(i).gameObject);
                Buddy.Sensors.Microphones.EnableBeamforming = iValue;
                Debug.LogWarning("BEAMFORMING : " + Buddy.Sensors.Microphones.EnableBeamforming);
                if (iValue) 
                {
                    Debug.Log("ON beam: off echo + color on");
                    Circles[mDirectionValue - 1].GetComponentsInChildren<Image>()[1].color = STATUS_ON_COLOR;
                    TabButton[(int)TAB.BEAMFORMING].GetComponentsInChildren<Image>()[2].color = STATUS_ON_COLOR;
                    TabButton[(int)TAB.ECHO_CANCELLATION].GetComponent<CanvasGroup>().alpha = 0.5F;
                    foreach (GameObject lContent in lContents)
                        lContent.SetActive(false);
                    lContents[0].SetActive(true);
                    TabContent[(int)TAB.BEAMFORMING].transform.GetChild(2).gameObject.SetActive(true);
                }
                else 
                {
                    Debug.Log("OFF beam: on echo + color off");
                    Circles[mDirectionValue - 1].GetComponentsInChildren<Image>()[1].color = STATUS_OFF_RED_COLOR;
                    TabButton[(int)TAB.BEAMFORMING].GetComponentsInChildren<Image>()[2].color = STATUS_OFF_COLOR;
                    TabButton[(int)TAB.ECHO_CANCELLATION].GetComponent<CanvasGroup>().alpha = 1F;
                    foreach (GameObject lContent in lContents)
                        lContent.SetActive(true);
                    lContents[0].SetActive(false);
                    TabContent[(int)TAB.BEAMFORMING].transform.GetChild(2).gameObject.SetActive(false);
                }
            });

            // EchoCancellation
            EchoCancellationToggle.onValueChanged.AddListener((iValue) =>
            {
                // Get all child gameobject of EchoCancellation for mutual exclusion
                List<GameObject> lContents = new List<GameObject>();
                int lChildCount = TabContent[(int)TAB.BEAMFORMING].transform.childCount;
                for (int i = 0; i < lChildCount; i++)
                    lContents.Add(TabContent[(int)TAB.BEAMFORMING].transform.GetChild(i).gameObject);
                Buddy.Sensors.Microphones.EnableEchoCancellation = iValue;
                Debug.LogWarning("ECHOCANCEL : " + Buddy.Sensors.Microphones.EnableEchoCancellation);

                if (iValue)
                {
                    Debug.Log("ON echo: off beam forming + color on");
                    TabButton[(int)TAB.ECHO_CANCELLATION].GetComponentsInChildren<Image>()[2].color = STATUS_ON_COLOR;
                    TabButton[(int)TAB.BEAMFORMING].GetComponent<CanvasGroup>().alpha = 0.5F;
                    foreach (GameObject lContent in lContents)
                        lContent.SetActive(false);
                    lContents[0].SetActive(true);
                }
                else
                {
                    Debug.Log("Off echo: on beam forming + color off");
                    TabButton[(int)TAB.ECHO_CANCELLATION].GetComponentsInChildren<Image>()[2].color = STATUS_OFF_COLOR;
                    TabButton[(int)TAB.BEAMFORMING].GetComponent<CanvasGroup>().alpha = 1F;
                    foreach (GameObject lContent in lContents)
                        lContent.SetActive(true);
                    lContents[0].SetActive(false);
                    lContents[2].SetActive(false);
                }
            });

            // Volume slider init & callback
            VolumeSlider.value = (int)(Buddy.Actuators.Speakers.Volume * 100F);
            VolumeSliderText.text = VolumeSlider.value.ToString();
            VolumeSlider.onValueChanged.AddListener((iVolume) =>
            {
                Buddy.Actuators.Speakers.Volume = iVolume / 100F;
                VolumeSliderText.text = iVolume.ToString();
            });

            // Initialize listen.
            Buddy.Vocal.DefaultInputParameters = new SpeechInputParameters()
            {
                Grammars = new string[] { "common" },
                RecognitionThreshold = 5000
            };

            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.OnEndListening.Add(OnEndListeningSpeechToText);

            // Text to speech
            TextToSpeechButton.onClick.AddListener(delegate
            {
                OnTextToSpeechButtonClick();
            });

            // Extract & Play Gimmick
            GimmickDropdown.AddOptions(new List<string>(Enum.GetNames(typeof(SoundSample))));
            GimmickPlayButton.onClick.AddListener(delegate
            {
                OnGimmickPlayButtonClick();
            });

            // Speech to text (Common Grammar)
            SpeechToTextGrammarButton.onValueChanged.AddListener((iValue) =>
            {
                if (iValue == true)
                    OnSpeechToTextGrammarButtonClick();
                else
                    Buddy.Vocal.StopAndClear();
            });

            // Speech to text (Freespeech)
            SpeechToTextFreeSpeechButton.onValueChanged.AddListener((iValue) =>
            {
                if (iValue == true)
                    OnSpeechToTextFreeSpeechButtonClick();
                else
                    Buddy.Vocal.StopAndClear();
            });

            //A tester
            SpeechToHybrid.onValueChanged.AddListener((iValue) =>
            {
                if (iValue)
                    OnHybrid();
                else
                    Buddy.Vocal.StopAndClear();
            });
            // Trigger : Play sound and switch to green for 1 second.
            Buddy.Vocal.EnableTrigger = true;

            SpeechToTextFreeSpeechButton.isOn = false;

            // BEGIN ACCRA COMP
            //Buddy.Vocal.OnTrigger.Clear();
            //Buddy.Vocal.OnTrigger.Add(
            //    (iInput) =>
            //    {
            //        TriggerText.SetTrigger("ON");
            //        //TriggerScore.text = ... waitCore
            //        Buddy.Actuators.Speakers.Media.Play(SoundSample.BEEP_1);

            //        // Display green for one second then switch red.
            //        mTimeTrigger = new Timer(1000);
            //        mTimeTrigger.Elapsed += OnTriggerTimedEvent;
            //        mTimeTrigger.Start();
            //    });

            try {
                CallbackArray<int> lCallback = (CallbackArray<int>)typeof(Vocal).GetProperty("OnTrigger").GetValue(Buddy.Vocal);
                lCallback.Clear();
                lCallback.Add((iInput) => {
                        TriggerText.SetTrigger("ON"); 
                        //TriggerScore.text = ... waitCore 
                        Buddy.Actuators.Speakers.Media.Play(SoundSample.BEEP_1);

                        // Display green for one second then switch red.
                        mTimeTrigger = new Timer(1000);
                        mTimeTrigger.Elapsed += OnTriggerTimedEvent;
                        mTimeTrigger.Start();
                    });
            } catch (Exception iException) {
                ACCRAComp.OnTrigger(() => {
                    TriggerText.SetTrigger("ON");
                    //TriggerScore.text = ... waitCore
                    Buddy.Actuators.Speakers.Media.Play(SoundSample.BEEP_1);

                    // Display green for one second then switch red.
                    mTimeTrigger = new Timer(1000);
                    mTimeTrigger.Elapsed += OnTriggerTimedEvent;
                    mTimeTrigger.Start();
                });
            }
            // END ACCRA COMP

            // Init Recorded audio dropdown list
            ReplayAudioSource.clip = null;
            if (RecordDropdown.options.Count <= 1)
                RecordDropdown.interactable = false;

            //Button Reset List
            ResetList.onClick.AddListener(OnClicResetList);

            // Recording
            mNoiseDetector = Buddy.Perception.NoiseDetector;
            RecordButton.GetComponentsInChildren<Text>()[0].text = "START RECORDING";
            RecordButton.onClick.AddListener(OnRecordingButtonClick);
            PlayRecordButton.onClick.AddListener(OnPlayRecordButtonClick);
            PlayRecordButton.GetComponentsInChildren<Text>()[0].text = "PLAY";

            //Play Music
            PlayMusic.GetComponentsInChildren<Text>()[0].text = "PLAY MUSIC";
            PlayMusic.onClick.AddListener(OnPlayMusic);
        }

        public void OnDisable()
        {
            // Tabs
            foreach (Button lButton in TabButton)
                lButton.onClick.RemoveAllListeners();
            // Trigger
            TriggerToggle.onValueChanged.RemoveAllListeners();
            TriggerTreshSlider.onValueChanged.RemoveAllListeners();

            // Localization
            LocalizationToggle.onValueChanged.RemoveAllListeners();
            LocalizationTreshSlider.onValueChanged.RemoveAllListeners();

            // BeamForming
            BeamFormingToggle.onValueChanged.RemoveAllListeners();

            //EchoCancellation
            EchoCancellationToggle.onValueChanged.RemoveAllListeners();

            //Volume
            VolumeSlider.onValueChanged.RemoveAllListeners();

            // Text to speech
            TextToSpeechButton.onClick.RemoveAllListeners();

            // Gimmicks
            GimmickDropdown.ClearOptions();
            GimmickPlayButton.onClick.RemoveAllListeners();

            TriggerTreshSlider.onValueChanged.RemoveAllListeners();
            LocalizationTreshSlider.onValueChanged.RemoveAllListeners();

            TriggerDropdown.onValueChanged.RemoveAllListeners();

            BeamFormingDropDown.onValueChanged.RemoveAllListeners();


            // Speech to text
            SpeechToTextGrammarButton.onValueChanged.RemoveAllListeners();
            SpeechToTextFreeSpeechButton.onValueChanged.RemoveAllListeners();
            SpeechToTextFreeSpeechButton.isOn = false;
            SpeechToTextGrammarButton.isOn = false;
            Buddy.Vocal.StopAndClear();

            // Trigger
            TriggerScore.text = "0";
            Buddy.Vocal.EnableTrigger = false;

            // BEGIN ACCRA COMP
            try {
                CallbackArray<int> lCallback = (CallbackArray<int>)typeof(Vocal).GetProperty("OnTrigger").GetValue(Buddy.Vocal);
                lCallback.Clear();
            } catch (Exception iException) {
                ACCRAComp.Clear();
            }
            // END ACCRA COMP

            // Record
            if (mBIsPlaying)
                OnPlayRecordButtonClick();
            RecordButton.onClick.RemoveAllListeners();
            PlayRecordButton.onClick.RemoveAllListeners();
            mListAudio.Clear();
            mNoiseDetector.OnDetect.Clear();

            // Remove end listening callbacks
            Buddy.Vocal.OnEndListening.Clear();
            ResetList.onClick.RemoveAllListeners();
        }

        private void OnTextToSpeechButtonClick()
        {
            Buddy.Vocal.StopAndClear();
            Buddy.Vocal.Say(TextToSpeechInputField.text);
        }

        private void OnClicResetList()
        {
            Debug.LogWarning("RESET LIST");
            RecordDropdown.options.Clear();
            mListRecorded.Clear();
        }

        private void OnGimmickPlayButtonClick()
        {
            Buddy.Actuators.Speakers.Media.Stop();
            Buddy.Actuators.Speakers.Media.Play((SoundSample)Enum.Parse(typeof(SoundSample), GimmickDropdown.options[GimmickDropdown.value].text));
        }
        
        private void OnPlayMusic()
        {
            AudioSource lAudioSource = ReplayAudioSource.GetComponent<AudioSource>();
            if (Equals("PLAY MUSIC", PlayMusicText.text))
            {
                PlayMusic.GetComponentsInChildren<Image>()[1].sprite = mStopBig;
                PlayMusicText.text = "STOP MUSIC";
                lAudioSource.clip = AudioClipMusic; 
                lAudioSource.Play();
            }
            else if(Equals("STOP MUSIC", PlayMusicText.text))
            {
                PlayMusic.GetComponentsInChildren<Image>()[1].sprite = mPlayBig;
                PlayMusicText.text = "PLAY MUSIC";
                lAudioSource.Stop();
                lAudioSource.clip = null;
            }
        }

        private void OnSpeechToTextGrammarButtonClick()
        {
            Buddy.Vocal.StopAndClear();

            // Reset button display of free speech
            //SpeechToTextFreeSpeechButton.isOn = false;
            //SpeechToHybrid.isOn = false;

            Buddy.Vocal.DefaultInputParameters.RecognitionMode = SpeechRecognitionMode.GRAMMAR_ONLY;
            if (!Buddy.Vocal.Listen())
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.STOPPING, "ERROR ON LISTEN !!!! ");
        }

        private void OnSpeechToTextFreeSpeechButtonClick()
        {
            Buddy.Vocal.StopAndClear();

            // Reset button display of grammar
            //SpeechToTextGrammarButton.isOn = false;
            //SpeechToHybrid.isOn = false;

            Buddy.Vocal.DefaultInputParameters.RecognitionMode = SpeechRecognitionMode.FREESPEECH_ONLY;
            if (!Buddy.Vocal.Listen())
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.STOPPING, "ERROR ON LISTEN !!!! ");
        }

        private void OnHybrid()
        {
            Buddy.Vocal.StopAndClear();
            
            Buddy.Vocal.DefaultInputParameters.RecognitionMode = SpeechRecognitionMode.GRAMMAR_THEN_FREESPEECH;
            if (!Buddy.Vocal.Listen())
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.STOPPING, "ERROR ON LISTEN !!!! ");

        }

        private void OnEndListeningSpeechToText(SpeechInput iSpeechInput)
        {
            if (SpeechToTextFreeSpeechButton.isOn)
            {
                if (iSpeechInput.IsInterrupted)
                    return;
                SpeechToTextFreeSpeechButton.isOn = false;
                SpeechToHybrid.isOn = false;
                return;
            }
            else if (SpeechToTextGrammarButton.isOn)
            {
                if (iSpeechInput.IsInterrupted)
                {
                    ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.INFO, LogInfo.BAD_ARGUMENT, "No speech was recognized.");
                    return;
                }
                //else if (iSpeechInput.Confidence <= 0)
                //    ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.INFO, LogInfo.BAD_ARGUMENT, "No speech was recognized.");
                SpeechToTextGrammarButton.isOn = false;
                SpeechToHybrid.isOn = false;
            }
            else if(SpeechToHybrid.isOn)
            {
                SpeechToTextFreeSpeechButton.isOn = false;
                SpeechToTextGrammarButton.isOn = false;
            }
        }

        private void OnTriggerTimedEvent(System.Object source, System.Timers.ElapsedEventArgs e)
        {
            mTimeTrigger.Stop();
            mTimeTrigger.Close();
        }

        private void OnRecordingButtonClick()
        {
            //avoir des ref des text plutot que de faire des getcomponentsinchildren a chaque clic qui reste bad niveau opti meme si ça ne change pas grand chose ici
            if (string.Equals("START RECORDING", RecordButton.GetComponentsInChildren<Text>()[0].text))
            {
                PlayRecordButton.interactable = false;
                RecordButton.GetComponentsInChildren<Text>()[0].text = "STOP RECORDING";
                RecordButton.GetComponentsInChildren<Image>()[1].sprite = mStop;

                // Save first AudioClip and callback when new sound detected.
                mListAudio.Clear();
                mAudioClip = null;
                mNoiseDetector.OnDetect.Add(
                (fFloat) =>
                {
                    if (!PlayRecordButton.interactable)
                    {
                        if (mNoiseDetector.MicrophoneIdx < mIPreviousMicroIndex && null != mAudioClip && mAudioClip.length > 1F)
                        {
                            mListAudio.Enqueue(mAudioClip);
                            mAudioClip = null;
                            mNoiseDetector.RecordClip.UnloadAudioData();
                        }

                        if (mNoiseDetector.MicrophoneData != null)
                        {
                            ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "Recording : Save new state" + mNoiseDetector.MicrophoneIdx);

                            mAudioClip = AudioClip.Create(mNoiseDetector.RecordClip.name, mNoiseDetector.RecordClip.samples, mNoiseDetector.RecordClip.channels, mNoiseDetector.RecordClip.frequency, false);
                            float[] samples = new float[mNoiseDetector.RecordClip.samples * mNoiseDetector.RecordClip.channels];
                            mNoiseDetector.RecordClip.GetData(samples, 0);
                            mAudioClip.SetData(samples, 0);
                        }

                        mIPreviousMicroIndex = mNoiseDetector.MicrophoneIdx;
                    }
                });

                return;
            }

            if (string.Equals("STOP RECORDING", RecordButton.GetComponentsInChildren<Text>()[0].text))
            {
                PlayRecordButton.interactable = true;
                RecordButton.GetComponentsInChildren<Text>()[0].text = "START RECORDING";
                RecordButton.GetComponentsInChildren<Image>()[1].sprite = mRecord;
                mNoiseDetector.OnDetect.Clear();
                if (null != mAudioClip && mAudioClip.length > 1F)
                    mListAudio.Enqueue(mAudioClip);
                if (mListAudio.Count > 0)
                {
                    mListRecorded.Add(new Queue<AudioClip>(mListAudio));
                    mListAudio.Clear();
                    RecordDropdown.options.Add(new Dropdown.OptionData("record_" + RecordDropdown.options.Count.ToString()));
                    if (RecordDropdown.interactable == false)
                        RecordDropdown.interactable = true;
                }
                return;
            }
        }

        private void OnPlayRecordButtonClick()
        {
            if (mBIsPlaying)
            {
                Debug.LogWarning("---- IS PLAYING ----");
                RecordButton.interactable = true;
                RecordDropdown.interactable = true;
                PlayRecordButton.GetComponentsInChildren<Text>()[0].text = "PLAY";
                PlayRecordButton.GetComponentsInChildren<Image>()[1].sprite = mPlay;
                ReplayAudioSource.Stop();
                mListAudio.Clear();
                mBIsPlaying = false;
            }
            else
            {
                if (RecordDropdown.value == 0)
                    return;
                Debug.LogWarning("---- IS NOT PLAYING ----");
                RecordButton.interactable = false;
                RecordDropdown.interactable = false;
                PlayRecordButton.GetComponentsInChildren<Text>()[0].text = "STOP";
                PlayRecordButton.GetComponentsInChildren<Image>()[1].sprite = mStop;
                // Ignore 0 index, because it's the default msg in list
                if (RecordDropdown.value > 0)
                {
                    Debug.LogWarning("-------- DropDownVal: " + RecordDropdown.value + " LisRecMax: " + mListRecorded.Count + " -----------");
                    mListAudio = new Queue<AudioClip>(mListRecorded[RecordDropdown.value - 1]);
                }
                mBIsPlaying = true;
            }
        }

        private IEnumerator GetFreespeechCredentials()
        {
            WWW lWWW = new WWW(CREDENTIAL_DEFAULT_URL);
            yield return lWWW;

            try {
                Buddy.Vocal.DefaultInputParameters.Credentials = lWWW.text;
                ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.ENABLED, "Free Speech enabled.");

                SpeechToTextFreeSpeechButton.interactable = true;
            } catch {
                SpeechToTextFreeSpeechButton.interactable = false;
            }
        }
    }
}
