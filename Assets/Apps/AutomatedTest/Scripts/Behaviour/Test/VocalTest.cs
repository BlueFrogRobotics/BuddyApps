using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.AutomatedTest
{
    public sealed class VocalTest : AModuleTest
    {
        public override string Name
        {
            get
            {
                return (Buddy.Resources.GetString("vocal"));
            }
        }

        private const string FREESPEECH_CREDENTIAL_DEFAULT_URL = "http://bfr-dev.azurewebsites.net/dev/BuddyDev-cmfc3b05c071.txt";

        private System.Random mRand;

        private bool mListeningLoop;

        public override void InitTestList()
        {
            mAvailableTest = new List<string>();
            mAvailableTest.Add("playgimmick");
            mAvailableTest.Add("tts");
            mAvailableTest.Add("sttgrammar");
            mAvailableTest.Add("sttfreespeech");
            return;
        }

        public override void InitPool()
        {
            mTestPool = new Dictionary<string, TestRoutine>();
            mTestPool.Add("playgimmick", PlayGimmick);
            mTestPool.Add("tts", TextToSpeechTest);
            mTestPool.Add("sttgrammar", SpeechToTextGrammarTest);
            mTestPool.Add("sttfreespeech", SpeechToTextFreeSpeechTest);
            return;
        }

        private void Awake()
        {
            Buddy.Vocal.DefaultInputParameters = new SpeechInputParameters()
            {
                Grammars = new string[] { "common" },
                RecognitionThreshold = 5000,
            };
            StartCoroutine(GetFreespeechCredentials());
        }
        public VocalTest()
        {
            mListeningLoop = true;
            mRand = new System.Random();
        }

        // All TestRoutine of this module:

        #region PLAY_GIMMICK
        public IEnumerator PlayGimmick()
        {
            //  --- INIT ---

            //  --- CODE ---
            DebugColor("PlayGimmick work in progress", "blue");

            // Show UI - And Implement all Ui callback
            DisplayTestUi("playgimmick",
            () =>   // --- OnClickRepeat ---
            {
                Array lValue = Enum.GetValues(typeof(SoundSample));
                Buddy.Actuators.Speakers.Media.Stop();
                Buddy.Actuators.Speakers.Media.Play((SoundSample)lValue.GetValue(mRand.Next(0, lValue.Length)), 60F);
            },
            () =>   // --- OnClickStop ---
            {
                DebugColor("-- stop --", "blue");
                Buddy.Actuators.Speakers.Media.Stop();
            }, null, null);

            // --- MODE ---
            // Automatic mode is not possible for this test
            if (!Buddy.Navigation.IsBusy)
            {
                Array lValue = Enum.GetValues(typeof(SoundSample));
                Buddy.Actuators.Speakers.Media.Stop();
                Buddy.Actuators.Speakers.Media.Play((SoundSample)lValue.GetValue(mRand.Next(0, lValue.Length)), 60F);
            }

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
        }
        #endregion

        #region TTS_TESTS
        public IEnumerator TextToSpeechTest()
        {
            //  --- INIT ---

            //  --- CODE ---
            DebugColor("TextToSpeechTest work in progress", "blue");

            // Show UI - And Implement all Ui callback
            DisplayTestUi("tts",
            () =>   // --- OnClickRepeat ---
            {
                Buddy.Vocal.StopAndClear();
                Buddy.Vocal.Say(Buddy.Resources.GetString("ttstest"));
            },
            () =>   // --- OnClickStop ---
            {
                DebugColor("-- stop --", "blue");
                Buddy.Vocal.StopAndClear();
            }, null, null);

            // --- MODE ---
            if (Mode == TestMode.M_MANUAL && !Buddy.Navigation.IsBusy)
            {
                Buddy.Vocal.StopAndClear();
                Buddy.Vocal.Say(Buddy.Resources.GetString("ttstest"));
            }
            else if (Mode == TestMode.M_AUTO && !Buddy.Navigation.IsBusy)
            {
                Buddy.Vocal.StopAndClear();
                Buddy.Vocal.Say(Buddy.Resources.GetString("ttstest"), (iSpeech) => 
                {
                    if (iSpeech.IsInterrupted)
                        return;
                    mResultPool.Add("tts", true);
                    mTestInProcess = false;
                });
            }
 
            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            Buddy.Vocal.StopAndClear();
        }
        #endregion

        #region STT_GRAM_TESTS
        public IEnumerator SpeechToTextGrammarTest()
        {
            //  --- INIT ---
            mListeningLoop = true;
            Buddy.Vocal.DefaultInputParameters.RecognitionMode = SpeechRecognitionMode.GRAMMAR_ONLY;
            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.OnEndListening.Add((iSpeechInput) => { OnEndListeningSpeechToText(iSpeechInput, "sttgrammar"); });

            //  --- CODE ---
            DebugColor("SpeechToTextGrammarTest work in progress", "blue");

            // Show UI - And Implement all Ui callback
            DisplayTestUi("sttgrammar",
            () =>   // --- OnClickRepeat ---
            {
                // Reset the header title
                Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("sttgrammar"));
                Buddy.Vocal.StopAndClear();
                if (!Buddy.Vocal.Listen())
                    ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.STOPPING, "ERROR ON LISTEN !!!! ");
            },
            () =>   // --- OnClickStop ---
            {
                if (Buddy.Vocal.IsBusy)
                {
                    mListeningLoop = false;
                    // Reset the header title
                    Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("sttgrammar"));
                    Buddy.Vocal.StopAndClear();
                    DebugColor("-- stop --", "blue");
                }
            }, null, null);

            Buddy.Vocal.StopAndClear();
            if (!Buddy.Vocal.Listen())
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.STOPPING, "ERROR ON LISTEN !!!! ");

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.StopAndClear();
        }
        #endregion

        #region STT_FREESPEECH_TESTS
        public IEnumerator SpeechToTextFreeSpeechTest()
        {
            //  --- INIT ---
            mListeningLoop = true;
            Buddy.Vocal.DefaultInputParameters.RecognitionMode = SpeechRecognitionMode.FREESPEECH_ONLY;
            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.OnEndListening.Add((iSpeechInput) => { OnEndListeningSpeechToText(iSpeechInput, "sttfreespeech"); });

            //  --- CODE ---
            DebugColor("SpeechToTextFreeSpeechTest work in progress", "blue");

            // Show UI - And Implement all Ui callback
            DisplayTestUi("sttfreespeech",
            () =>   // --- OnClickRepeat ---
            {
                // Reset the header title
                Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("sttfreespeech"));
                Buddy.Vocal.StopAndClear();
                if (!Buddy.Vocal.Listen())
                    ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.STOPPING, "ERROR ON LISTEN !!!! ");
            },
            () =>   // --- OnClickStop ---
            {
                if (Buddy.Vocal.IsBusy)
                {
                    mListeningLoop = false;
                    // Reset the header title
                    Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("sttfreespeech"));
                    Buddy.Vocal.StopAndClear();
                    DebugColor("-- stop --", "blue");
                }
            }, null, null);

            Buddy.Vocal.StopAndClear();
            if (!Buddy.Vocal.Listen())
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.STOPPING, "ERROR ON LISTEN !!!! ");

            // --- Wait for User ---
            while (mTestInProcess)
                yield return null;

            //  --- EXIT ---
            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.StopAndClear();
        }
        #endregion

        private void OnEndListeningSpeechToText(SpeechInput iSpeechInput, string iSpeechTest)
        {
            if (iSpeechInput.IsInterrupted || (iSpeechTest == "sttgrammar"/* && iSpeechInput.Confidence <= 0*/))
            {
                if (Mode == TestMode.M_AUTO && mListeningLoop)
                {
                    if (!Buddy.Vocal.Listen())
                        ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.STOPPING, "ERROR ON LISTEN !!!! ");
                }
                return;
            }
            // Display of the iSpeechInput
            if (!string.IsNullOrEmpty(iSpeechInput.Rule) && !string.IsNullOrEmpty(iSpeechInput.Utterance))
                Buddy.GUI.Header.DisplayComplexTitle(iSpeechInput.Utterance, iSpeechInput.Rule);
            else if (!string.IsNullOrEmpty(iSpeechInput.Utterance))
                Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("heard") + iSpeechInput.Utterance);
            if (Mode == TestMode.M_AUTO && !string.IsNullOrEmpty(iSpeechInput.Utterance))
            {
                mResultPool.Add(iSpeechTest, true);
                mTestInProcess = false;
            }
        }

        private IEnumerator GetFreespeechCredentials()
        {
            WWW lWWW = new WWW(FREESPEECH_CREDENTIAL_DEFAULT_URL);
            yield return lWWW;

            Buddy.Vocal.DefaultInputParameters.Credentials = lWWW.text;
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.ENABLED, "Free Speech enabled.");
        }
    }
}
