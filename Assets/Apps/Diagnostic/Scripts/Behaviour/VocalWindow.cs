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
using System.IO;

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
        private Button SwitchMicrophone;

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
        private Text STTRule;

        [SerializeField]
        private Text STTScore;

        [SerializeField]
        private Text STTOutput;

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

        [SerializeField]
        private Slider MicroGainSlider;

        [SerializeField]
        private Text MicroGainText;

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

        private bool mIsRecording = false;
        private Sprite mRecord;
        private Sprite mStop;
        private Sprite mPlay;
        private Sprite mStopBig;
        private Sprite mPlayBig;
        private Color mBuddyBlue = new Color(0.0f, 0.831f, 0.819f);
        private Color mWhite = new Color(1f, 1f, 1f);

        private Timer mTimeTrigger;

        private bool mBIsPlaying = false;

        // List of all recorded audio
        private List<Queue<AudioClip>> mListRecorded = new List<Queue<AudioClip>>();

        // Last audio recorded
        private Queue<AudioClip> mListAudio = new Queue<AudioClip>();

        private Text mRecordButtonText;
        private Image mRecordButtonIcon;
        private AudioClip mAudioClip;

        private Image mSoundLocField;
        private int mSoundLocAngle;
        private float mSoundLocPreviousTreatedAngle;

        public void OnClickTabs(TAB iClickedTab)
        {
            if (iClickedTab < 0 && iClickedTab >= TAB.COUNT)
                return;
            if (TabContent[(int)iClickedTab].activeSelf)
                return;
            for (TAB iTab = 0; iTab < TAB.COUNT; iTab++) {
                TabContent[(int)iTab].SetActive(false);
                TabButton[(int)iTab].GetComponent<Image>().color = TAB_IDLE_COLOR;
            }
            TabContent[(int)iClickedTab].SetActive(true);
            TabButton[(int)iClickedTab].GetComponent<Image>().color = Color.white;
        }

        private void Start()
        {
            mSoundLocPreviousTreatedAngle = 0F;
            mSoundLocField = LocalizationRad.GetComponent<Image>();
            mRecord = Buddy.Resources.Get<Sprite>("os_icon_micro_on");
            mStop = Buddy.Resources.Get<Sprite>("os_icon_stop");
            mPlay = Buddy.Resources.Get<Sprite>("os_icon_play");
            mStopBig = Buddy.Resources.Get<Sprite>("os_icon_stop_big");
            mPlayBig = Buddy.Resources.Get<Sprite>("os_icon_play_big");
            SpeechToTextFreeSpeechButton.interactable = true;

            InitRecordDropDown();

            MicroGainSlider.onValueChanged.RemoveAllListeners();
            MicroGainSlider.onValueChanged.AddListener((iInput) => OnSliderGainChange(iInput));

            TriggerTreshSlider.wholeNumbers = true;
            TriggerTreshSlider.minValue = 0F;
            TriggerTreshSlider.maxValue = 200F;
            TriggerTreshSlider.value = 80F;
            TriggerTreshText.text = (TriggerTreshSlider.value * 10).ToString();
            TriggerTreshSlider.onValueChanged.RemoveAllListeners();
            TriggerTreshSlider.onValueChanged.AddListener((iInput) => OnSliderThresholdChange(iInput));


            LocalizationTreshSlider.wholeNumbers = true;
            LocalizationTreshSlider.minValue = 10F;
            LocalizationTreshSlider.maxValue = 128F;
            LocalizationTreshSlider.value = 40F;
            LocalizationTreshText.text = LocalizationTreshSlider.value.ToString();
            LocalizationTreshSlider.onValueChanged.RemoveAllListeners();
            LocalizationTreshSlider.onValueChanged.AddListener((iInput) => OnChangeThresholdLocalization(iInput));

            TriggerDropdown.value = 3;
            TriggerDropdown.onValueChanged.AddListener((iInput) => OnSearchTriggerOption(iInput));

            BeamFormingDropDown.value = 5;
            BeamFormingDropDown.onValueChanged.AddListener((iInput) => OnDirectionBeam(iInput));

            StartCoroutine(GetFreespeechCredentials());
            PlayMusic.GetComponentsInChildren<Image>()[1].sprite = mPlayBig;

            mRecordButtonText = RecordButton.GetComponentsInChildren<Text>()[0];
            mRecordButtonIcon = RecordButton.GetComponentsInChildren<Image>()[1];

            TriggerScore.text = "--";
        }

        private void OnSliderGainChange(float iInput)
        {
            MicroGainText.text = iInput.ToString();
            byte lValue = (byte)iInput;
            Buddy.Sensors.Microphones.SoundOutputParameters = new SoundOutputParameters(lValue);
            Debug.LogWarning("Byte converter " + lValue);
            if (Buddy.Sensors.Microphones.SoundOutputParameters != null)
                Debug.LogWarning("Gain is set at " + Buddy.Sensors.Microphones.SoundOutputParameters.Volume.ToString());
        }

        private void InitRecordDropDown()
        {
            RecordDropdown.options.Add(new Dropdown.OptionData("AUDIO FILES"));

            if (Directory.Exists(Buddy.Resources.AppRawDataPath))
                foreach (string lPath in Directory.GetFiles(Buddy.Resources.AppRawDataPath)) {
                    if (lPath.Contains("record") && lPath.Contains(".wav")) {
                        RecordDropdown.options.Add(new Dropdown.OptionData(lPath.Substring(lPath.IndexOf("record"), lPath.IndexOf(".wav") - lPath.IndexOf("record"))));
                    }
                }

            if (RecordDropdown.options.Count > 1)
                RecordDropdown.interactable = true;
        }

        // Manage warning text
        private void OnToggleActivated()
        {
            if (EchoCancellationToggle.isOn && TabContent[2].activeSelf) {
                BeamFormingText.SetActive(true);
            } else if (BeamFormingToggle.isOn && TabContent[3].activeSelf) {
                EchoCancellationText.SetActive(true);
            } else {
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
            TriggerTreshText.text = FloatToShort(iInput * 10).ToString();
            Buddy.Sensors.Microphones.VocalTriggerParameters = new VocalTriggerParameters(FloatToShort(iInput * 10));

        }

        private void OnSearchTriggerOption(int iInput)
        {
            int lSearchValue = 0;
            switch (iInput) {
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
            Buddy.Sensors.Microphones.VocalTriggerParameters = new VocalTriggerParameters(FloatToShort(TriggerTreshSlider.value * 10), IntToShort(lSearchValue));
        }
        #endregion

        #region TabBeamforming
        private void OnDirectionBeam(int iInput)
        {
            foreach (GameObject img in Circles) {
                img.GetComponentsInChildren<Image>()[1].color = STATUS_OFF_RED_COLOR;
            }

            Circles[iInput].GetComponentsInChildren<Image>()[1].color = STATUS_ON_COLOR;
            Buddy.Sensors.Microphones.BeamformingParameters = new BeamformingParameters(IntToByte(iInput));
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
            if (mSoundLocAngle != Microphones.NO_SOUND_LOCALIZATION) {
                LocalizationRad.SetActive(true);
                // Display untreated value
                LocalizationText.text = mSoundLocAngle.ToString() + " °";
                if (mSoundLocAngle < 0)
                    mSoundLocAngle += 360;
                mSoundLocField.rectTransform.Rotate(0, 0, mSoundLocAngle - mSoundLocPreviousTreatedAngle);
                mSoundLocPreviousTreatedAngle = mSoundLocAngle;

            } else {
                // No value compute - disable UI
                LocalizationText.text = "-- °";
                LocalizationRad.SetActive(false);
            }
        }

        public void Update()
        {
            UpdateSoundLocalization();

            // Ensure consistency with menu OS
            if (Buddy.Sensors.Microphones.GetAudioInputDevices().Contains("USB-Audio")) {
                SwitchMicrophone.GetComponentsInChildren<Image>()[1].sprite = Buddy.Resources.Get<Sprite>("os_icon_play_big");
                SwitchMicrophone.GetComponent<Image>().color = STATUS_ON_COLOR;
                if (Buddy.Sensors.Microphones.CurrentMicrophone.Code != "DEVICE_IN_USB_DEVICE")
                    SwitchMicrophone.GetComponentsInChildren<Text>()[0].text = "FRONTAL MICROPHONE ACTIVATED";
                else
                    SwitchMicrophone.GetComponentsInChildren<Text>()[0].text = "MICRO ARRAY ACTIVATED";
            } else {
                SwitchMicrophone.GetComponentsInChildren<Text>()[0].text = "!!!MICRO ARRAY UNPLUGGED!!!";
                SwitchMicrophone.GetComponentsInChildren<Image>()[1].sprite = Buddy.Resources.Get<Sprite>("os_icon_cancel_big");
                SwitchMicrophone.GetComponent<Image>().color = STATUS_OFF_RED_COLOR;
            }

            if (mBIsPlaying) // Playing record
                if (null == ReplayAudioSource.clip || !ReplayAudioSource.isPlaying)
                    // turn off
                    OnPlayRecordButtonClick();



            //if (mIsRecording) {
            //    if (!PlayRecordButton.interactable) {

            //        int lMicroIdx = mNoiseDetector.MicrophoneIdx;

            //        if (lMicroIdx < mIPreviousMicroIndex && null != mAudioClip && mAudioClip.length > 1F) {
            //            mListAudio.Enqueue(mAudioClip);
            //            mAudioClip = null;
            //        }


            //        if (mNoiseDetector.MicrophoneData != null) {
            //            ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "Recording : Save new state" + lMicroIdx);
            //            mAudioClip = AudioClip.Create(mNoiseDetector.RecordClip.name, mNoiseDetector.RecordClip.samples, mNoiseDetector.RecordClip.channels, mNoiseDetector.RecordClip.frequency, false);

            //            float[] samples = new float[mNoiseDetector.RecordClip.samples * mNoiseDetector.RecordClip.channels];

            //            mNoiseDetector.RecordClip.GetData(samples, 0);
            //            mAudioClip.SetData(samples, 0);
            //        }
            //        mIPreviousMicroIndex = lMicroIdx;
            //    }
            //}
        }

        public void OnEnable()
        {
            StartCoroutine(GetFreespeechCredentials());

            // Tabs
            TabButton[(int)TAB.TRIGGER].onClick.AddListener(() => { OnClickTabs(TAB.TRIGGER); });
            TabButton[(int)TAB.LOCALIZATION].onClick.AddListener(() => { OnClickTabs(TAB.LOCALIZATION); });
            TabButton[(int)TAB.BEAMFORMING].onClick.AddListener(() => { OnClickTabs(TAB.BEAMFORMING); });
            TabButton[(int)TAB.ECHO_CANCELLATION].onClick.AddListener(() => { OnClickTabs(TAB.ECHO_CANCELLATION); });

            foreach (GameObject img in Circles) {
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
            TriggerToggle.onValueChanged.AddListener((iValue) => {
                Buddy.Vocal.EnableTrigger = iValue;
                if (iValue)
                    TabButton[(int)TAB.TRIGGER].GetComponentsInChildren<Image>()[2].color = STATUS_ON_COLOR;
                else
                    TabButton[(int)TAB.TRIGGER].GetComponentsInChildren<Image>()[2].color = STATUS_OFF_COLOR;
            });

            TriggerDropdown.value = 3;
            TriggerDropdown.onValueChanged.AddListener((iInput) => OnSearchTriggerOption(iInput));
            TriggerTreshText.text = FloatToShort(TriggerTreshSlider.value * 10).ToString();
            TriggerTreshSlider.onValueChanged.AddListener((iInput) => OnSliderThresholdChange(iInput));

            // Localization
            Buddy.Sensors.Microphones.EnableSoundLocalization = LocalizationToggle.isOn;
            LocalizationToggle.onValueChanged.AddListener((iValue) => {
                Buddy.Sensors.Microphones.EnableSoundLocalization = iValue;
                if (iValue)
                    TabButton[(int)TAB.LOCALIZATION].GetComponentsInChildren<Image>()[2].color = STATUS_ON_COLOR;

                else
                    TabButton[(int)TAB.LOCALIZATION].GetComponentsInChildren<Image>()[2].color = STATUS_OFF_COLOR;


            });

            LocalizationTreshText.text = LocalizationTreshSlider.value.ToString();
            LocalizationTreshSlider.onValueChanged.AddListener((iInput) => OnChangeThresholdLocalization(iInput));

            // BeamForming            
            BeamFormingDropDown.value = 5;
            BeamFormingDropDown.onValueChanged.AddListener((iInput) => OnDirectionBeam(iInput));
            BeamFormingToggle.onValueChanged.AddListener((iValue) => {
                // Get all child gameobject of EchoCancellation for mutual exclusion
                List<GameObject> lContents = new List<GameObject>();
                int lChildCount = TabContent[(int)TAB.ECHO_CANCELLATION].transform.childCount;
                for (int i = 0; i < lChildCount; i++)
                    lContents.Add(TabContent[(int)TAB.ECHO_CANCELLATION].transform.GetChild(i).gameObject);
                Buddy.Sensors.Microphones.EnableBeamforming = iValue;
                if (iValue) {
                    Circles[BeamFormingDropDown.value].GetComponentsInChildren<Image>()[1].color = STATUS_ON_COLOR;
                    TabButton[(int)TAB.BEAMFORMING].GetComponentsInChildren<Image>()[2].color = STATUS_ON_COLOR;
                    TabButton[(int)TAB.ECHO_CANCELLATION].GetComponent<CanvasGroup>().alpha = 0.5F;
                    foreach (GameObject lContent in lContents)
                        lContent.SetActive(false);
                    lContents[0].SetActive(true);
                    TabContent[(int)TAB.BEAMFORMING].transform.GetChild(2).gameObject.SetActive(true);
                } else {
                    Circles[BeamFormingDropDown.value].GetComponentsInChildren<Image>()[1].color = STATUS_OFF_RED_COLOR;
                    TabButton[(int)TAB.BEAMFORMING].GetComponentsInChildren<Image>()[2].color = STATUS_OFF_COLOR;
                    TabButton[(int)TAB.ECHO_CANCELLATION].GetComponent<CanvasGroup>().alpha = 1F;
                    foreach (GameObject lContent in lContents)
                        lContent.SetActive(true);
                    lContents[0].SetActive(false);
                    TabContent[(int)TAB.BEAMFORMING].transform.GetChild(2).gameObject.SetActive(false);
                }
            });

            // EchoCancellation
            EchoCancellationToggle.onValueChanged.AddListener((iValue) => {
                // Get all child gameobject of EchoCancellation for mutual exclusion
                List<GameObject> lContents = new List<GameObject>();
                int lChildCount = TabContent[(int)TAB.BEAMFORMING].transform.childCount;
                for (int i = 0; i < lChildCount; i++)
                    lContents.Add(TabContent[(int)TAB.BEAMFORMING].transform.GetChild(i).gameObject);
                Buddy.Sensors.Microphones.EnableEchoCancellation = iValue;

                if (iValue) {
                    TabButton[(int)TAB.ECHO_CANCELLATION].GetComponentsInChildren<Image>()[2].color = STATUS_ON_COLOR;
                    TabButton[(int)TAB.BEAMFORMING].GetComponent<CanvasGroup>().alpha = 0.5F;
                    foreach (GameObject lContent in lContents)
                        lContent.SetActive(false);
                    lContents[0].SetActive(true);
                } else {
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
            VolumeSlider.onValueChanged.AddListener((iVolume) => {
                Buddy.Actuators.Speakers.Volume = iVolume / 100F;
                VolumeSliderText.text = iVolume.ToString();
            });

            // Initialize listen.
            Buddy.Vocal.DefaultInputParameters = new SpeechInputParameters() {
                Grammars = new string[] { "common", "companion_commands", "companion_questions" },
                RecognitionThreshold = 1000
            };

            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.OnEndListening.Add(OnEndListeningSpeechToText);

            // Text to speech
            TextToSpeechButton.onClick.AddListener(delegate {
                OnTextToSpeechButtonClick();
            });

            // Extract & Play Gimmick
            GimmickDropdown.AddOptions(new List<string>(Enum.GetNames(typeof(SoundSample))));
            GimmickPlayButton.onClick.AddListener(delegate {
                OnGimmickPlayButtonClick();
            });

            // Speech to text (Common Grammar)
            SpeechToTextGrammarButton.onValueChanged.AddListener((iValue) => {
                if (iValue)
                    OnSpeechToTextGrammarButtonClick();
                else
                    Buddy.Vocal.StopAndClear();
            });

            // Speech to text (Freespeech)
            SpeechToTextFreeSpeechButton.onValueChanged.AddListener((iValue) => {
                if (iValue)
                    OnSpeechToTextFreeSpeechButtonClick();
                else
                    Buddy.Vocal.StopAndClear();
            });

            //A tester
            SpeechToHybrid.onValueChanged.AddListener((iValue) => {
                if (iValue)
                    OnHybrid();
                else
                    Buddy.Vocal.StopAndClear();
            });
            // Trigger : Play sound and switch to green for 1 second.
            Buddy.Vocal.EnableTrigger = true;

            Buddy.Vocal.OnTrigger.Add((iInput) => {
                TriggerText.SetTrigger("ON");
                TriggerScore.text = "--";
                Buddy.Actuators.Speakers.Media.Play(SoundSample.BEEP_1);

                // Display green for one second then switch red.
                mTimeTrigger = new Timer(1000);
                mTimeTrigger.Elapsed += OnTriggerTimedEvent;
                mTimeTrigger.Start();
            });

            Buddy.Vocal.OnCompleteTrigger.Add((iInput) => {
                TriggerScore.text = iInput.RecognitionScore.ToString();
            });

            // Init Recorded audio dropdown list
            ReplayAudioSource.clip = null;
            if (RecordDropdown.options.Count <= 1)
                RecordDropdown.interactable = false;
            else
                RecordDropdown.interactable = true;


            //Button Reset List
            ResetList.onClick.AddListener(OnClicResetList);

            // Recording
            //mNoiseDetector = Buddy.Perception.NoiseDetector;
            RecordButton.GetComponentsInChildren<Text>()[0].text = "START RECORDING";
            RecordButton.onClick.AddListener(OnRecordingButtonClick);
            PlayRecordButton.onClick.AddListener(OnPlayRecordButtonClick);
            PlayRecordButton.GetComponentsInChildren<Text>()[0].text = "PLAY";

            //Switch Microphone

            if (Buddy.Sensors.Microphones.GetAudioInputDevices().Contains("USB-Audio")) {
                SwitchMicrophone.GetComponentsInChildren<Image>()[1].sprite = Buddy.Resources.Get<Sprite>("os_icon_play_big");
                SwitchMicrophone.GetComponent<Image>().color = STATUS_ON_COLOR;
                if (Buddy.Sensors.Microphones.CurrentMicrophone.Code != "DEVICE_IN_USB_DEVICE")
                    SwitchMicrophone.GetComponentsInChildren<Text>()[0].text = "FRONTAL MICROPHONE ACTIVATED";
                else
                    SwitchMicrophone.GetComponentsInChildren<Text>()[0].text = "MICRO ARRAY ACTIVATED";
            } else {
                SwitchMicrophone.GetComponentsInChildren<Text>()[0].text = "!!!MICRO ARRAY UNPLUGGED!!!";
                SwitchMicrophone.GetComponentsInChildren<Image>()[1].sprite = Buddy.Resources.Get<Sprite>("os_icon_cancel_big");
                SwitchMicrophone.GetComponent<Image>().color = STATUS_OFF_RED_COLOR;
            }

            SwitchMicrophone.onClick.AddListener(OnSwitchMicrophone);

            //Play Music
            PlayMusic.GetComponentsInChildren<Text>()[0].text = "PLAY MUSIC";
            PlayMusic.GetComponentsInChildren<Image>()[1].sprite = Buddy.Resources.Get<Sprite>("os_icon_play_big");
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
            TriggerScore.text = "--";
            Buddy.Vocal.EnableTrigger = false;

            Buddy.Vocal.OnTrigger.Clear();
            Buddy.Vocal.OnCompleteTrigger.Clear();

            // Record
            if (mBIsPlaying)
                OnPlayRecordButtonClick();
            RecordButton.onClick.RemoveAllListeners();
            PlayRecordButton.onClick.RemoveAllListeners();
            mListAudio.Clear();
            //mNoiseDetector.OnDetect.Clear();

            // Remove end listening callbacks
            Buddy.Vocal.OnEndListening.Clear();
            ResetList.onClick.RemoveAllListeners();

            PlayMusic.onClick.RemoveAllListeners();
            SwitchMicrophone.onClick.RemoveAllListeners();
        }

        private void OnTextToSpeechButtonClick()
        {
            Buddy.Vocal.StopAndClear();
            Buddy.Vocal.Say(TextToSpeechInputField.text);
        }

        private void OnClicResetList()
        {
            for (int i = 1; i < RecordDropdown.options.Count; ++i) {
                Utils.DeleteFile(Buddy.Resources.AppRawDataPath + RecordDropdown.options[i].text + ".wav");
            }
            RecordDropdown.options.Clear();
            RecordDropdown.options.Add(new Dropdown.OptionData("PREVIOUSLY RECORDED AUDIO FILES"));
            RecordDropdown.captionText.text = "PREVIOUSLY RECORDED AUDIO FILES";
            mListRecorded.Clear();
        }

        private void OnGimmickPlayButtonClick()
        {
            Buddy.Actuators.Speakers.Media.Stop();
            Buddy.Actuators.Speakers.Media.Play((SoundSample)Enum.Parse(typeof(SoundSample), GimmickDropdown.options[GimmickDropdown.value].text));
        }

        private void OnSwitchMicrophone()
        {


            if (Buddy.Sensors.Microphones.GetAudioInputDevices().Contains("USB-Audio")) {
                SwitchMicrophone.GetComponent<Image>().color = STATUS_ON_COLOR;
                SwitchMicrophone.GetComponentsInChildren<Image>()[1].sprite = Buddy.Resources.Get<Sprite>("os_icon_play_big");
                if (Buddy.Sensors.Microphones.CurrentMicrophone.Code == "DEVICE_IN_USB_DEVICE") {
                    Buddy.Sensors.Microphones.SwitchMicrophone("DEVICE_IN_USB_DEVICE", false);
                    Buddy.Sensors.Microphones.SwitchMicrophone("DEVICE_IN_WIRED_HEADSET", true);
                    SwitchMicrophone.GetComponentsInChildren<Text>()[0].text = "FRONTAL MICROPHONE ACTIVATED";
                } else {
                    Buddy.Sensors.Microphones.SwitchMicrophone("DEVICE_IN_WIRED_HEADSET", false);
                    Buddy.Sensors.Microphones.SwitchMicrophone("DEVICE_IN_USB_DEVICE", true);
                    SwitchMicrophone.GetComponentsInChildren<Text>()[0].text = "MICRO ARRAY ACTIVATED";
                }
            } else {
                SwitchMicrophone.GetComponentsInChildren<Text>()[0].text = "!!!MICRO ARRAY UNPLUGGED!!!";
                SwitchMicrophone.GetComponentsInChildren<Image>()[1].sprite = Buddy.Resources.Get<Sprite>("os_icon_cancel_big");
                SwitchMicrophone.GetComponent<Image>().color = STATUS_OFF_RED_COLOR;
            }

            
        }

        private void OnPlayMusic()
        {
            AudioSource lAudioSource = ReplayAudioSource.GetComponent<AudioSource>();
            if (Equals("PLAY MUSIC", PlayMusicText.text)) {
                PlayMusic.GetComponentsInChildren<Image>()[1].sprite = mStopBig;
                PlayMusicText.text = "STOP MUSIC";

                //Select random song
                int lRand = UnityEngine.Random.Range(1, 16);

                lAudioSource.clip = Buddy.Resources.Get<AudioClip>("os_dance_" + lRand.ToString("00"));
                lAudioSource.Play();
            } else if (Equals("STOP MUSIC", PlayMusicText.text)) {
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
            Buddy.Vocal.DefaultInputParameters.RecognitionMode = SpeechRecognitionMode.GRAMMAR_ONLY;
            STTScore.text = "--";
            STTRule.text = "--";
            STTOutput.text = "--";
            if (!Buddy.Vocal.Listen())
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.STOPPING, "ERROR ON LISTEN !!!! ");
        }

        private void OnSpeechToTextFreeSpeechButtonClick()
        {
            Buddy.Vocal.StopAndClear();

            // Reset button display of grammar
            Buddy.Vocal.DefaultInputParameters.RecognitionMode = SpeechRecognitionMode.FREESPEECH_ONLY;
            STTScore.text = "--";
            STTRule.text = "--";
            STTOutput.text = "--";
            if (!Buddy.Vocal.Listen())
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.STOPPING, "ERROR ON LISTEN !!!! ");
        }

        private void OnHybrid()
        {
            Buddy.Vocal.StopAndClear();

            Buddy.Vocal.DefaultInputParameters.RecognitionMode = SpeechRecognitionMode.GRAMMAR_THEN_FREESPEECH;
            STTScore.text = "--";
            STTRule.text = "--";
            STTOutput.text = "--";
            if (!Buddy.Vocal.Listen())
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.STOPPING, "ERROR ON LISTEN !!!! ");
        }

        private void OnEndListeningSpeechToText(SpeechInput iSpeechInput)
        {
            if (!string.IsNullOrWhiteSpace(iSpeechInput.Rule) && iSpeechInput.Rule.Contains("#"))
                STTRule.text = iSpeechInput.Rule;
            else
                STTRule.text = Utils.GetRule(iSpeechInput);

            STTOutput.text = iSpeechInput.Utterance;
            STTScore.text = (iSpeechInput.Confidence * 10000).ToString();
            SpeechToTextFreeSpeechButton.isOn = false;
            SpeechToTextGrammarButton.isOn = false;
            SpeechToHybrid.isOn = false;
        }

        private void OnTriggerTimedEvent(System.Object source, System.Timers.ElapsedEventArgs e)
        {
            mTimeTrigger.Stop();
            mTimeTrigger.Close();
        }

        private void OnRecordingButtonClick()
        {
            if (string.Equals("START RECORDING", mRecordButtonText.text)) {
                if (!mIsRecording) {
                    PlayRecordButton.interactable = false;
                    mRecordButtonText.text = "STOP RECORDING";
                    mRecordButtonIcon.sprite = mStop;

                    // Save first AudioClip and callback when new sound detected.
                    mListAudio.Clear();
                    mAudioClip = null;
                    mIsRecording = true;

                    StartRecording();
                }
                return;
            }

            if (string.Equals("STOP RECORDING", mRecordButtonText.text)) {
                mRecordButtonText.text = "START RECORDING";
                mRecordButtonIcon.sprite = mRecord;
                StopRecording();
            }
        }

        private void StartRecording()
        {
            mAudioClip = Microphone.Start(Microphone.devices[0], false, 10, 44100);
        }

        private void StopRecording()
        {
            Microphone.End(Microphone.devices[0]);

            mIsRecording = false;
            PlayRecordButton.interactable = true;

            if (mAudioClip != null)
                mListAudio.Enqueue(mAudioClip);

            if (mListAudio.Count > 0) {
                mListRecorded.Add(new Queue<AudioClip>(mListAudio));
                Utils.Save(Buddy.Resources.AppRawDataPath + "record_" + RecordDropdown.options.Count.ToString(), Utils.Combine(mListAudio.ToArray()));
                mListAudio.Clear();
                RecordDropdown.options.Add(new Dropdown.OptionData("record_" + RecordDropdown.options.Count.ToString()));
                RecordDropdown.value = RecordDropdown.options.Count - 1;
                if (RecordDropdown.interactable == false)
                    RecordDropdown.interactable = true;
            }
        }

        private void OnPlayRecordButtonClick()
        {
            if (!mBIsPlaying) {
                if (RecordDropdown.value == 0)
                    return;
                RecordButton.interactable = false;
                RecordDropdown.interactable = false;
                PlayRecordButton.GetComponentsInChildren<Text>()[0].text = "STOP";
                PlayRecordButton.GetComponentsInChildren<Image>()[1].sprite = mStop;

                // Get Audio from file and play
                StartCoroutine(PlayRecord());

            } else if (mBIsPlaying) {
                AudioSource lAudioSource = ReplayAudioSource.GetComponent<AudioSource>();
                RecordButton.interactable = true;
                RecordDropdown.interactable = true;
                PlayRecordButton.GetComponentsInChildren<Text>()[0].text = "PLAY RECORDED";
                PlayRecordButton.GetComponentsInChildren<Image>()[1].sprite = mPlay;
                lAudioSource.Stop();
                lAudioSource.clip = null;
                mBIsPlaying = false;
            }
        }

        private IEnumerator PlayRecord()
        {
            if (!File.Exists(Buddy.Resources.AppRawDataPath + RecordDropdown.captionText.text + ".wav")) {
                // Stop
                OnPlayRecordButtonClick();
            } else {
                AudioSource lAudioSource = ReplayAudioSource.GetComponent<AudioSource>();
                using (var www = new WWW("file://" + Buddy.Resources.AppRawDataPath + RecordDropdown.captionText.text + ".wav")) {
                    yield return www;
                    lAudioSource.clip = www.GetAudioClip();
                }

                while (lAudioSource.clip.loadState != AudioDataLoadState.Loaded && lAudioSource.clip.loadState != AudioDataLoadState.Failed) {
                    yield return new WaitForSeconds(0.1F);
                }

                if (lAudioSource.clip.loadState == AudioDataLoadState.Loaded) {
                    lAudioSource.Play();
                    mBIsPlaying = true;
                } else {
                    // Stop
                    mBIsPlaying = true;
                    OnPlayRecordButtonClick();
                }
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
