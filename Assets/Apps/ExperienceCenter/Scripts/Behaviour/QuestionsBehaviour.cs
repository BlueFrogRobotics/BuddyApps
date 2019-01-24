using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using BlueQuark;

namespace BuddyApp.ExperienceCenter
{


    public class QuestionsBehaviour : MonoBehaviour
    {
        private AnimatorManager mAnimatorManager;
        private AttitudeBehaviour mAttitudeBehaviour;
        private IdleBehaviour mIdleBehaviour;
        //private TextToSpeech mTTS;
        //private VocalManager mVocalManager;
        //private SpeechToText mSpeechToText;
        //private SphinxTrigger mSphinxTrigger;

        private List<string> mKeyList;

        private bool mLaunchSTTOnce;
        private int mTimeOutCount;

        private bool mRestartSTT;
        private bool mStartSTTCoroutine;

        public bool behaviourEnd;

        public void InitBehaviour()
        {
            //mVocalManager = BYOS.Instance.Interaction.VocalManager;
            //mSpeechToText = BYOS.Instance.Interaction.SpeechToText;
            //mSphinxTrigger = BYOS.Instance.Interaction.SphinxTrigger;
            mAnimatorManager = GameObject.Find("AIBehaviour").GetComponent<AnimatorManager>();
            mAttitudeBehaviour = GameObject.Find("AIBehaviour").GetComponent<AttitudeBehaviour>();
            mIdleBehaviour = GameObject.Find("AIBehaviour").GetComponent<IdleBehaviour>();
            behaviourEnd = false;
            //if (ExperienceCenterData.Instance.VoiceTrigger)
            //    BYOS.Instance.Interaction.SphinxTrigger.LaunchRecognition();
            //mSpeechToText.OnBestRecognition.Clear();
            //mSpeechToText.OnBestRecognition.Add(SpeechToTextCallback);
            //mSpeechToText.OnErrorEnum.Clear();
            //mSpeechToText.OnErrorEnum.Add(ErrorCallback);
            //mVocalManager.EnableTrigger = ExperienceCenterData.Instance.VoiceTrigger;
            Buddy.Vocal.EnableTrigger = true;
            //mVocalManager.EnableDefaultErrorHandling = false;

            // VOCON
            Debug.Log("VOCOOOOOOOOOOOOOOON");
            //mVocalManager.UseVocon = true;
            //mVocalManager.AddGrammar("experiencecenter", LoadContext.APP);
            Buddy.Vocal.Listen("experiencecenter", SpeechRecognitionMode.GRAMMAR_ONLY);
            Buddy.Vocal.OnEndListening.Add(SpeechToTextCallback);

            //mVocalManager.OnEndReco = SpeechToTextCallback;
            //mVocalManager.OnError = ErrorCallback;
            Buddy.Vocal.OnListeningStatus.Add(ErrorCallback);

            //BYOS.Instance.Interaction.SphinxTrigger.SetThreshold(1E-24f);
            mTimeOutCount = 0;

            //mTTS = BYOS.Instance.Interaction.TextToSpeech;

            StartCoroutine(WatchSphinxTrigger());

            InitKeyList();
        }

        private void InitKeyList()
        {
            mKeyList = new List<string> {
                "idlesee",
                "idleleg",
                "questiongreeting",
                "questionshy",
                "questionability",
                "questiondance",
                "questionlaguage",
                "questionvibe",
                "questionpresence",
                "questionpresentation"
            };
        }

        private IEnumerator WatchSphinxTrigger()
        {
            mStartSTTCoroutine = true;
            while (!behaviourEnd)
            {
                if (/*!mSpeechToText.HasFinished*/ Buddy.Vocal.IsListening && mStartSTTCoroutine)
                {
                    OnSphinxTrigger();

                    yield return new WaitForSeconds(0.5f);
                    yield return new WaitUntil(() => /*mSpeechToText.HasFinished*/!Buddy.Vocal.IsListening);
                }

                yield return new WaitForSeconds(0.5f);
            }
        }

        private void OnSphinxTrigger()
        {
            StartCoroutine(EnableSpeechToText());

            if (ExperienceCenterData.Instance.EnableHeadMovement && !mIdleBehaviour.headPoseInit)
            {
                mAttitudeBehaviour.IsWaiting = false;
                //BYOS.Instance.Interaction.BMLManager.StopAllBehaviors();
                //BYOS.Instance.Interaction.BMLManager.LaunchByName("Reset01");
                Buddy.Behaviour.Interpreter.Stop();
                Buddy.Behaviour.Interpreter.Run("reset.xml");
                mIdleBehaviour.headPoseInit = true;
            }
        }

        public void SpeechToTextCallback(SpeechInput iSpeech)
        {
            if (!string.IsNullOrEmpty(iSpeech.Rule))
            {
                Debug.LogFormat("VOCON [EXCENTER][QUESTIONBEHAVIOUR] SpeechToText : {0} | Confidence : {1} | StartRule : {2}", iSpeech.Utterance, iSpeech.Confidence, iSpeech.Rule);
                bool lClauseFound = false;
                string lKey = "";
                foreach (string lElement in mKeyList)
                {
                    string[] lPhonetics = Buddy.Resources.GetPhoneticStrings(lElement);
                    foreach (string lClause in lPhonetics)
                    {
                        if (iSpeech.Rule.Contains(lElement))
                        {
                            if (ExperienceCenterData.Instance.EnableHeadMovement)
                                mAttitudeBehaviour.MoveHeadWhileSpeaking(-10, 10);
                            if(lElement!="idlesee")
                                Buddy.Vocal.SayKey(lElement, true);
                            lClauseFound = true;
                            lKey = lElement;
                            break;
                        }
                    }
                    if (lClauseFound)
                    {
                        mTimeOutCount = 0;
                        break;
                    }
                }
                // Launch Vocal Command if any
                if (!lClauseFound)
                {
                    Debug.Log("[EXCENTER][QUESTIONBEHAVIOUR] SpeechToText : Not Found");
                    mTimeOutCount++;
                    Debug.Log("[EXCENTER][QUESTIONBEHAVIOUR] TimeOutCount : " + mTimeOutCount);
                    if (mTimeOutCount >= 3)
                    {
                        mTimeOutCount = 0;
                        mRestartSTT = false;
                    }
                    else
                        mLaunchSTTOnce = false;
                }
                else
                {
                    StartCoroutine(LaunchVocalCommand(lKey));
                    mLaunchSTTOnce = false;
                }
            }
        }

        private IEnumerator LaunchVocalCommand(string key)
        {
            yield return new WaitUntil(() => !Buddy.Vocal.IsSpeaking);
            if (key == "idlesee")
                mAnimatorManager.ActivateCmd((byte)(Command.Welcome));
            else if (key == "idleleg")
                mAnimatorManager.ActivateCmd((byte)(Command.ByeBye));
        }

        public void StopBehaviour()
        {
            Debug.LogWarning("[EXCENTER][QUESTIONBEHAVIOUR] Stop Question Behaviour");

            if (Buddy.Vocal.IsSpeaking)
                Buddy.Vocal.StopListening();
            behaviourEnd = true;

            //mSpeechToText.Stop();
            Buddy.Vocal.StopListening();
            Buddy.Vocal.OnEndListening.Remove(SpeechToTextCallback);
            Buddy.Vocal.OnListeningStatus.Remove(ErrorCallback);
            //Buddy.Vocal.OnEndListening.Clear();
            // Buddy.Vocal.OnListeningStatus.Clear();
            this.StopAllCoroutines();

            //mVocalManager.RemoveGrammar("experiencecenter", LoadContext.APP);
            //mVocalManager.UseVocon = false;
        }

        private IEnumerator EnableSpeechToText()
        {
            Buddy.Vocal.EnableTrigger = true;
            //mSphinxTrigger.StopRecognition();
            mStartSTTCoroutine = false;
            mRestartSTT = true;
            mLaunchSTTOnce = false;
            Debug.Log("dans begin enable speech to text");
            //Buddy.Vocal.Listen("experiencecenter", SpeechRecognitionMode.GRAMMAR_ONLY);
            //Buddy.Vocal.OnEndListening.Clear();
            //Buddy.Vocal.OnEndListening.Add(SpeechToTextCallback);
            //Buddy.Vocal.OnListeningStatus.Clear();
            //Buddy.Vocal.OnListeningStatus.Add(ErrorCallback);
            while (mRestartSTT)
            {
                if (!mLaunchSTTOnce)
                {
                    if (/*!mSpeechToText.HasFinished*/ Buddy.Vocal.IsListening)
                    {
                        // Recognition not finished yet, waiting until it ends cleanly
                        Debug.Log("dans vocal is listening");
                        yield return new WaitUntil(() => /*!mSpeechToText.HasFinished*/!Buddy.Vocal.IsListening);
                    }
                    else if (Buddy.Vocal.IsSpeaking)
                    {
                        // Buddy is answering, wait until he ends its sentence
                        Debug.Log("dans vocal is speaking");
                        yield return new WaitUntil(() => !Buddy.Vocal.IsSpeaking);
                    }
                    else
                    {
                        // Initiating Vocal Manager instance reco
                        Debug.Log("dans else");
                        yield return new WaitForSeconds(2.0f);
                        mLaunchSTTOnce = true;
                        //mSpeechToText.Request();
                        Buddy.Vocal.Listen("experiencecenter", SpeechRecognitionMode.GRAMMAR_ONLY);
                        //Buddy.Vocal.OnEndListening.Clear();
                        //Buddy.Vocal.OnEndListening.Add(SpeechToTextCallback);
                        //Buddy.Vocal.OnListeningStatus.Clear();
                        //Buddy.Vocal.OnListeningStatus.Add(ErrorCallback);
                    }
                }
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForSeconds(1.0f);
            Debug.Log("dans fin enable");
            mStartSTTCoroutine = true;
            //Buddy.Vocal.EnableTrigger = true;
            //mSphinxTrigger.LaunchRecognition();
            //mVocalManager.EnableTrigger = true;
        }

        public void ErrorCallback(SpeechInputStatus iError)
        {
            if (iError.IsError)
            {
                Debug.LogWarningFormat("[EXCENTER][QUESTIONBEHAVIOUR] ERROR STT: {0}", iError.ToString());
                mTimeOutCount++;
                Debug.Log("[EXCENTER][QUESTIONBEHAVIOUR] TimeOutCount : " + mTimeOutCount);
                if (mTimeOutCount >= 3)
                {
                    mTimeOutCount = 0;
                    mRestartSTT = false;
                }
                else
                    mLaunchSTTOnce = false;
            }
        }
    }
}
