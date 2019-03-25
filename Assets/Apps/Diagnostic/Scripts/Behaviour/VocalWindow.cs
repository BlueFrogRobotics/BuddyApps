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
        private Toggle SpeechToTextFreeSpeechButton;

        [SerializeField]
        private Toggle SpeechToTextGrammarButton;


        [SerializeField]
        private Animator TriggerText;

        [SerializeField]
        private Text TriggerScore;

        [SerializeField]
        private Text LocalizationText;

        [SerializeField]
        private Image LocalizationRad;

        [SerializeField]
        private Text AmbiantSoundLevelText;

        [SerializeField]
        private Slider AmbiantSoundLevelSlider;

        [SerializeField]
        private Dropdown RecordDropdown;

        [SerializeField]
        private Button RecordButton;

        [SerializeField]
        private Button PlayRecordButton;

        [SerializeField]
        private AudioSource ReplayAudioSource;

        [Header("TRIGGER TAB")]
        [SerializeField]
        private Button TiggerTabButton;

        [SerializeField]
        private GameObject TiggerContent;

        [SerializeField]
        private Slider TiggerThreshSlider;

        [SerializeField]
        private Text TriggerTreshText;

        [SerializeField]
        private Dropdown TriggerDropdown;

        [Header("LOCALIZATION TAB")]
        [SerializeField]
        private Button LocalizationTabButton;

        [SerializeField]
        private GameObject LocalizationContent;

        [Header("BEAMFORMING TAB")]
        [SerializeField]
        private Button BeamFormingTabButton;

        [SerializeField]
        private GameObject BeamFormingContent;

        [Header("ECHO CANCELLATION TAB")]
        [SerializeField]
        private Button EchoCancellationTabButton;

        [SerializeField]
        private GameObject EchoCancellationContent;

        private readonly Color TAB_IDLE_COLOR = new Color(221, 221, 221);
        private readonly Color TAB_DISABLE_COLOR = new Color(221, 221, 221);
        private readonly Color STATUS_OFF_COLOR = new Color(56, 56, 56);
        private readonly Color STATUS_ON_COLOR = new Color(0, 212, 209);

        private const string CREDENTIAL_DEFAULT_URL = "http://bfr-dev.azurewebsites.net/dev/BuddyDev-cmfc3b05c071.txt";

        private Sprite mRecord;
        private Sprite mStop;
        private Sprite mPlay;
        private Color mBuddyBlue = new Color(0.0f, 0.831f, 0.819f);
        private Color mWhite = new Color(1f, 1f, 1f);

        private Timer mTimeTrigger;
        private NoiseDetector mNoiseDetector;

        private bool mBIsPlaying = false;

        private List<Queue<AudioClip>> mListRecorded = new List<Queue<AudioClip>>();

        // Last audio recorded
        private Queue<AudioClip> mListAudio = new Queue<AudioClip>();

        private AudioClip mAudioClip;

        private int mIPreviousMicroIndex = 0;

        private int mPrevious;

        public enum TABS
        {
            TRIGGER_TAB,
            LOCALIZATION_TAB,
            BEAMFORMINF_TAB,
            ECHO_CANCELLATION_TAB,
        }

        public void OnClickTabs(int iTab)
        {

        }

        private void Start()
        {
            mRecord = Buddy.Resources.Get<Sprite>("os_icon_micro_on");
            mStop = Buddy.Resources.Get<Sprite>("os_icon_stop");
            mPlay = Buddy.Resources.Get<Sprite>("os_icon_play");
            mPrevious = Buddy.Sensors.Microphones.SoundLocalization;
            SpeechToTextFreeSpeechButton.interactable = false;
            StartCoroutine(GetFreespeechCredentials());
        }

        public void Update()
        {
            LocalizationText.text = Buddy.Sensors.Microphones.SoundLocalization + " °";
            LocalizationRad.rectTransform.Rotate(0, 0, Buddy.Sensors.Microphones.SoundLocalization - mPrevious);
            mPrevious = Buddy.Sensors.Microphones.SoundLocalization;
            AmbiantSoundLevelText.text = Buddy.Sensors.Microphones.AmbiantSound + " db";
            AmbiantSoundLevelSlider.value = Buddy.Sensors.Microphones.AmbiantSound;

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

            // Trigger : Play sound and switch to green for 1 second.
            Buddy.Vocal.EnableTrigger = true;
            Buddy.Vocal.OnTrigger.Clear();
            Buddy.Vocal.OnTrigger.Add(
                (iInput) =>
                {
                    TriggerText.SetTrigger("ON");
                    TriggerScore.text = iInput.ToString();
                    Buddy.Actuators.Speakers.Media.Play(SoundSample.BEEP_1);

                    // Display green for one second then switch red.
                    mTimeTrigger = new Timer(1000);
                    mTimeTrigger.Elapsed += OnTriggerTimedEvent;
                    mTimeTrigger.Start();
                });

            // Init Recorded audio dropdown list
            ReplayAudioSource.clip = null;
            if (RecordDropdown.options.Count <= 1)
                RecordDropdown.interactable = false;

            // Recording
            mNoiseDetector = Buddy.Perception.NoiseDetector;
            RecordButton.GetComponentsInChildren<Text>()[0].text = "START RECORDING";
            RecordButton.onClick.AddListener(OnRecordingButtonClick);
            PlayRecordButton.onClick.AddListener(OnPlayRecordButtonClick);
            PlayRecordButton.GetComponentsInChildren<Text>()[0].text = "PLAY";
        }

        public void OnDisable()
        {

            //Volume
            VolumeSlider.onValueChanged.RemoveAllListeners();

            // Text to speech
            TextToSpeechButton.onClick.RemoveAllListeners();

            // Gimmicks
            GimmickDropdown.ClearOptions();
            GimmickPlayButton.onClick.RemoveAllListeners();

            // Speech to text
            SpeechToTextGrammarButton.onValueChanged.RemoveAllListeners();
            SpeechToTextFreeSpeechButton.onValueChanged.RemoveAllListeners();
            SpeechToTextFreeSpeechButton.isOn = false;
            SpeechToTextGrammarButton.isOn = false;
            Buddy.Vocal.StopAndClear();

            // Trigger
            TriggerScore.text = "0";
            Buddy.Vocal.EnableTrigger = false;
            Buddy.Vocal.OnTrigger.Clear();

            // Record
            if (mBIsPlaying)
                OnPlayRecordButtonClick();
            RecordButton.onClick.RemoveAllListeners();
            PlayRecordButton.onClick.RemoveAllListeners();
            mListAudio.Clear();
            mNoiseDetector.OnDetect.Clear();

            // Remove end listening callbacks
            Buddy.Vocal.OnEndListening.Clear();
        }

        private void OnTextToSpeechButtonClick()
        {
            Buddy.Vocal.StopAndClear();
            Buddy.Vocal.Say(TextToSpeechInputField.text);
        }

        private void OnGimmickPlayButtonClick()
        {
            Buddy.Actuators.Speakers.Media.Stop();
            Buddy.Actuators.Speakers.Media.Play((SoundSample)Enum.Parse(typeof(SoundSample), GimmickDropdown.options[GimmickDropdown.value].text));
        }

        private void OnSpeechToTextGrammarButtonClick()
        {
            Buddy.Vocal.StopAndClear();

            // Reset button display of free speech
            SpeechToTextFreeSpeechButton.isOn = false;

            Buddy.Vocal.DefaultInputParameters.RecognitionMode = SpeechRecognitionMode.GRAMMAR_ONLY;
            if (!Buddy.Vocal.Listen())
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.STOPPING, "ERROR ON LISTEN !!!! ");
        }

        private void OnSpeechToTextFreeSpeechButtonClick()
        {
            Buddy.Vocal.StopAndClear();

            // Reset button display of grammar
            SpeechToTextGrammarButton.isOn = false;

            Buddy.Vocal.DefaultInputParameters.RecognitionMode = SpeechRecognitionMode.FREESPEECH_ONLY;
            if (!Buddy.Vocal.Listen())
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.STOPPING, "ERROR ON LISTEN !!!! ");
        }

        private void OnEndListeningSpeechToText(SpeechInput iSpeechInput)
        {
            if (SpeechToTextFreeSpeechButton.isOn == true)
            {
                if (iSpeechInput.IsInterrupted)
                    return;
                SpeechToTextFreeSpeechButton.isOn = false;
                return;
            }
            else if (SpeechToTextGrammarButton.isOn == true)
            {
                if (iSpeechInput.IsInterrupted)
                {
                    ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.INFO, LogInfo.BAD_ARGUMENT, "No speech was recognized.");
                    return;
                }
                else if (iSpeechInput.Confidence <= 0)
                    ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.INFO, LogInfo.BAD_ARGUMENT, "No speech was recognized.");
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
                        if (mNoiseDetector.MicrophoneIdx < mIPreviousMicroIndex && null != mAudioClip)
                        {
                            mListAudio.Enqueue(mAudioClip);
                            mAudioClip = null;
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
                if (null != mAudioClip)
                    mListAudio.Enqueue(mAudioClip);
                mListRecorded.Add(new Queue<AudioClip>(mListAudio));
                mListAudio.Clear();
                RecordDropdown.options.Add(new Dropdown.OptionData("record_" + RecordDropdown.options.Count.ToString()));
                if (RecordDropdown.interactable == false)
                    RecordDropdown.interactable = true;
                mNoiseDetector.OnDetect.Clear();
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

            Buddy.Vocal.DefaultInputParameters.Credentials = lWWW.text;
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.ENABLED, "Free Speech enabled.");

            SpeechToTextFreeSpeechButton.interactable = true;
        }
    }
}
