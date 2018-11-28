using System;
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
        private Text TriggerText;


        [SerializeField]
        private Text LocalizationText;

        [SerializeField]
        private Text AmbiantSoundLevelText;


        [SerializeField]
        private Button RecordButton;

        [SerializeField]
        private Button PlayRecordButton;

        [SerializeField]
        private AudioSource ReplayAudioSource;



        private Timer mTimeTrigger;
        private NoiseDetector mNoiseDetector;

        private bool mBIsPlaying = false;
        private Queue<AudioClip> mListAudio = new Queue<AudioClip>();
        private AudioClip mAudioClip;
        private int mIPreviousMicroIndex = 0;

        public void Update()
        {
            LocalizationText.text = "Localization: " + Buddy.Sensors.Microphones.SoundLocalization + " degrees";
            AmbiantSoundLevelText.text = "Ambiant Sound Level: " + Buddy.Sensors.Microphones.AmbiantSound + " db";
            
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
            SpeechToTextGrammarButton.onClick.AddListener(delegate {
                OnSpeechToTextGrammarButtonClick();
            });

            // Speech to text (Freespeech)
            SpeechToTextFreeSpeechButton.onClick.AddListener(delegate {
                OnSpeechToTextFreeSpeechButtonClick();
            });


            // Trigger : Play sound and switch to green for 1 second.
            TriggerText.color = Color.red;
            Buddy.Vocal.EnableTrigger = true;
            Buddy.Vocal.OnTrigger.Clear();
            Buddy.Vocal.OnTrigger.Add(
                (iInput) =>
                {
                    TriggerText.color = Color.green;
                    
                    Buddy.Actuators.Speakers.Media.Play(SoundSample.BEEP_1);

                    // Display green for one second then switch red.
                    mTimeTrigger = new Timer(1000);
                    mTimeTrigger.Elapsed += OnTriggerTimedEvent;
                    mTimeTrigger.Start();
                });


            // Recording
            mNoiseDetector = Buddy.Perception.NoiseDetector;
            RecordButton.GetComponentsInChildren<Text>()[0].text = "Start recording";
            RecordButton.onClick.AddListener(OnRecordingButtonClick);
            PlayRecordButton.onClick.AddListener(OnPlayRecordButtonClick);
            PlayRecordButton.GetComponentsInChildren<Text>()[0].text = "Play";
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
        }

        public void OnDisable()
        {
            // Text to speech
            TextToSpeechButton.onClick.RemoveAllListeners();

            // Gimmicks
            GimmickDropdown.ClearOptions();
            GimmickPlayButton.onClick.RemoveAllListeners();

            // Speech to text
            SpeechToTextGrammarButton.onClick.RemoveAllListeners();
            SpeechToTextFreeSpeechButton.onClick.RemoveAllListeners();

            // Trigger
            Buddy.Vocal.EnableTrigger = false;
            Buddy.Vocal.OnTrigger.Clear();

            // Record
            RecordButton.onClick.RemoveAllListeners();
            PlayRecordButton.onClick.RemoveAllListeners();
            mListAudio.Clear();
            mNoiseDetector.OnDetect.Clear();
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

        private void OnSpeechToTextGrammarButtonClick ()
        {
            Buddy.Vocal.StopAndClear();

            if (!Buddy.Vocal.Listen("common", OnEndListeningSpeechToText, SpeechRecognitionMode.GRAMMAR_ONLY))
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.STOPPING, "ERROR ON LISTEN !!!! ");
        }

        private void OnSpeechToTextFreeSpeechButtonClick ()
        {
            Buddy.Vocal.StopAndClear();

            if (!Buddy.Vocal.Listen(OnEndListeningSpeechToText, SpeechRecognitionMode.FREESPEECH_ONLY))
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.STOPPING, "ERROR ON LISTEN !!!! ");
        }

        private void OnEndListeningSpeechToText(SpeechInput iSpeechInput)
        {
            if (iSpeechInput.IsInterrupted || 0 <= iSpeechInput.Confidence) // Error during recognition, forced StopListening or nothing recognized.
            {
                return;
            }

            SpeechToTextField.text = iSpeechInput.Utterance;
        }

        private void OnTriggerTimedEvent(System.Object source, System.Timers.ElapsedEventArgs e)
        {
            TriggerText.color = Color.red;
            mTimeTrigger.Stop();
            mTimeTrigger.Close();
        }

        private void OnRecordingButtonClick()
        {
            if (string.Equals("Start recording", RecordButton.GetComponentsInChildren<Text>()[0].text))
            {
                PlayRecordButton.interactable = false;
                RecordButton.GetComponentsInChildren<Text>()[0].text = "Stop recording";

                // Save first AudioClip and callback when new sound detected.
                mListAudio.Clear();

                return;
            }

            if (string.Equals("Stop recording", RecordButton.GetComponentsInChildren<Text>()[0].text))
            {
                PlayRecordButton.interactable = true;
                RecordButton.GetComponentsInChildren<Text>()[0].text = "Start recording";
                
                if (null != mAudioClip)
                    mListAudio.Enqueue(mAudioClip);
                
                return;
            }
        }

        private void OnPlayRecordButtonClick()
        {
            if (mBIsPlaying)
            {
                RecordButton.interactable = true;
                PlayRecordButton.GetComponentsInChildren<Text>()[0].text = "Play";
                ReplayAudioSource.Stop();
                
                mBIsPlaying = false;
            }
            else
            {
                RecordButton.interactable = false;
                PlayRecordButton.GetComponentsInChildren<Text>()[0].text = "Stop";
                mBIsPlaying = true;
            }
        }
    }
}
