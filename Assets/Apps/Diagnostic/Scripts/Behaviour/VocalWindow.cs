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
        private InputField TextToSpeechInputField;

        [SerializeField]
        private Button TextToSpeechButton;


        [SerializeField]
        private Dropdown GimmickDropdown;

        [SerializeField]
        private Button GimmickPlayButton;
        

        [SerializeField]
        private Text SpeechToTextField;

        [SerializeField]
        private Button SpeechToTextFreeSpeechButton;

        [SerializeField]
        private Button SpeechToTextGrammarButton;


        [SerializeField]
        private Animator TriggerText;


        [SerializeField]
        private Text LocalizationText;
        [SerializeField]
        private Image LocalizationRad;
        [SerializeField]
        private Text AmbiantSoundLevelText;
        [SerializeField]
        private Slider AmbiantSoundLevelSlider;


        [SerializeField]
        private Button RecordButton;
        [SerializeField]
        private Button PlayRecordButton;

        [SerializeField]
        private AudioSource ReplayAudioSource;


        private const string CREDENTIAL_DEFAULT_URL = "http://bfr-dev.azurewebsites.net/dev/BuddyDev-cmfc3b05c071.txt";

        private Sprite mRecord = Buddy.Resources.Get<Sprite>("os_icon_micro_on");
        private Sprite mStop = Buddy.Resources.Get<Sprite>("os_icon_stop");
        private Sprite mPlay = Buddy.Resources.Get<Sprite>("os_icon_play");
        private Color mBuddyBlue = new Color(0.0f, 0.831f, 0.819f);
        private Color mWhite = new Color(1f, 1f, 1f);

        private Timer mTimeTrigger;
        private NoiseDetector mNoiseDetector;

        private bool mBIsPlaying = false;
        private Queue<AudioClip> mListAudio = new Queue<AudioClip>();
        private AudioClip mAudioClip;
        private int mIPreviousMicroIndex = 0;

        private int lprevious = Buddy.Sensors.Microphones.SoundLocalization;

        public void Update()
        {
            LocalizationText.text = Buddy.Sensors.Microphones.SoundLocalization + " °";
            LocalizationRad.rectTransform.Rotate(0, 0, Buddy.Sensors.Microphones.SoundLocalization - lprevious);
            lprevious = Buddy.Sensors.Microphones.SoundLocalization;
            AmbiantSoundLevelText.text = Buddy.Sensors.Microphones.AmbiantSound + " db";
            AmbiantSoundLevelSlider.value = Buddy.Sensors.Microphones.AmbiantSound;

            if (mBIsPlaying) // Playing record
            {
                if (null == ReplayAudioSource.clip || !ReplayAudioSource.isPlaying)
                {
                    ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "Record - Should start playing...");

                    if (0 < mListAudio.Count)
                    {
                        ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "Record - There is " + mListAudio.Count + " clip(s) to play...");
                        ReplayAudioSource.clip = mListAudio.Dequeue();
                        ReplayAudioSource.Play();
                    }
                    else
                    {
                        ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "Record - No clip to play...");
                        OnPlayRecordButtonClick();
                    }
                }
            }
        }

        public void OnEnable()
        {
            // Initialize listen.
            Buddy.Vocal.DefaultInputParameters = new SpeechInputParameters()
            {
                Grammars = new string[] { "common" },
                RecognitionThreshold = 5000
            };

            //SpeechToTextFreeSpeechButton.interactable = false;
            //StartCoroutine(GetFreespeechCredentials());

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
            //SpeechToTextGrammarButton.onClick.AddListener(delegate {
            //    OnSpeechToTextGrammarButtonClick();
            //});

            // Speech to text (Freespeech)
            //SpeechToTextFreeSpeechButton.onClick.AddListener(delegate {
            //    OnSpeechToTextFreeSpeechButtonClick();
            //});


            // Trigger : Play sound and switch to green for 1 second.
            //TriggerText.color = Color.red;
            Buddy.Vocal.EnableTrigger = true;
            Buddy.Vocal.OnTrigger.Clear();
            Buddy.Vocal.OnTrigger.Add(
                (iInput) =>
                {
                    //TriggerText.color = Color.green;
                    TriggerText.SetTrigger("ON");
                    Buddy.Actuators.Speakers.Media.Play(SoundSample.BEEP_1);

                    // Display green for one second then switch red.
                    mTimeTrigger = new Timer(1000);
                    mTimeTrigger.Elapsed += OnTriggerTimedEvent;
                    mTimeTrigger.Start();
                });


            // Recording
            mNoiseDetector = Buddy.Perception.NoiseDetector;
            RecordButton.GetComponentsInChildren<Text>()[0].text = "START RECORDING";
            RecordButton.onClick.AddListener(OnRecordingButtonClick);
            PlayRecordButton.onClick.AddListener(OnPlayRecordButtonClick);
            PlayRecordButton.GetComponentsInChildren<Text>()[0].text = "PLAY";
        }

        public void OnDisable()
        {
            // Text to speech
            TextToSpeechButton.onClick.RemoveAllListeners();

            // Gimmicks
            GimmickDropdown.ClearOptions();
            GimmickPlayButton.onClick.RemoveAllListeners();

            // Speech to text
            //SpeechToTextGrammarButton.onClick.RemoveAllListeners(); 
            //SpeechToTextFreeSpeechButton.onClick.RemoveAllListeners();

            // Trigger
            Buddy.Vocal.EnableTrigger = false;
            Buddy.Vocal.OnTrigger.Clear();

            // Record
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

        private void OnGimmickPlayButtonClick ()
        {
            Buddy.Actuators.Speakers.Media.Stop();
            Buddy.Actuators.Speakers.Media.Play((SoundSample)Enum.Parse(typeof(SoundSample), GimmickDropdown.options[GimmickDropdown.value].text));
        }

        //private void OnSpeechToTextGrammarButtonClick ()
        //{
        //    Buddy.Vocal.StopAndClear();
        //    SpeechToTextGrammarButton.GetComponent<Image>().color = mBuddyBlue;
        //    SpeechToTextFreeSpeechButton.GetComponent<Image>().color = mWhite;

        //    Buddy.Vocal.DefaultInputParameters.RecognitionMode = SpeechRecognitionMode.GRAMMAR_ONLY;
        //    if (!Buddy.Vocal.Listen())
        //        ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.STOPPING, "ERROR ON LISTEN !!!! ");
        //}

        //private void OnSpeechToTextFreeSpeechButtonClick ()
        //{
        //    Buddy.Vocal.StopAndClear();
        //    SpeechToTextGrammarButton.GetComponent<Image>().color = mWhite;
        //    SpeechToTextFreeSpeechButton.GetComponent<Image>().color = mBuddyBlue;

        //    Buddy.Vocal.DefaultInputParameters.RecognitionMode = SpeechRecognitionMode.FREESPEECH_ONLY;
        //    if (!Buddy.Vocal.Listen())
        //        ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.STOPPING, "ERROR ON LISTEN !!!! ");
        //}

        private void OnEndListeningSpeechToText(SpeechInput iSpeechInput)
        {
            SpeechToTextGrammarButton.GetComponent<Image>().color = mWhite;
            SpeechToTextFreeSpeechButton.GetComponent<Image>().color = mWhite;

            if (iSpeechInput.IsInterrupted || 0 <= iSpeechInput.Confidence)
            {
                ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.INFO, LogInfo.BAD_ARGUMENT, "No speech was recognized.");
        //        return;
            }

            SpeechToTextField.text = iSpeechInput.Utterance;
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

                mNoiseDetector.OnDetect.Add(
                (fFloat) =>
                {
                    if (!PlayRecordButton.interactable)
                    {
                        if (mNoiseDetector.MicrophoneIdx < mIPreviousMicroIndex && null != mAudioClip)
                            mListAudio.Enqueue(mAudioClip);

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

                mNoiseDetector.OnDetect.Clear();

                return;
            }
        }

        private void OnPlayRecordButtonClick()
        {
            if (mBIsPlaying)
            {
                RecordButton.interactable = true;
                PlayRecordButton.GetComponentsInChildren<Text>()[0].text = "PLAY";
                PlayRecordButton.GetComponentsInChildren<Image>()[1].sprite = mPlay;
                ReplayAudioSource.Stop();
                
                mBIsPlaying = false;
            }
            else
            {
                RecordButton.interactable = false;
                PlayRecordButton.GetComponentsInChildren<Text>()[0].text = "STOP";
                PlayRecordButton.GetComponentsInChildren<Image>()[1].sprite = mStop;
                mBIsPlaying = true;
            }
        }
        
        //private IEnumerator GetFreespeechCredentials()
        //{
        //    WWW lWWW = new WWW(CREDENTIAL_DEFAULT_URL);
        //    yield return lWWW;
            
        //    Buddy.Vocal.DefaultInputParameters.Credentials = lWWW.text;
        //    ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.ENABLED, "Free Speech enabled.");

        //    SpeechToTextFreeSpeechButton.interactable = true;
        //}
    }
}
