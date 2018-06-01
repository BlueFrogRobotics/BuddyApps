using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Buddy;

namespace BuddyApp.ExperienceCenter
{


    public class QuestionsBehaviour : MonoBehaviour
    {
        private AnimatorManager mAnimatorManager;
        private AttitudeBehaviour mAttitudeBehaviour;
        private IdleBehaviour mIdleBehaviour;
        private TextToSpeech mTTS;
        private VocalManager mVocalManager;
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
            mVocalManager = BYOS.Instance.Interaction.VocalManager;
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
            mVocalManager.EnableTrigger = false;
            mVocalManager.EnableDefaultErrorHandling = false;

            // VOCON
            Debug.Log("VOCOOOOOOOOOOOOOOON");
            mVocalManager.UseVocon = true;
            mVocalManager.AddGrammar("experiencecenter", LoadContext.APP);
            mVocalManager.OnVoconBest = SpeechToTextCallback;

            //mVocalManager.OnEndReco = SpeechToTextCallback;
            mVocalManager.OnError = ErrorCallback;

            BYOS.Instance.Interaction.SphinxTrigger.SetThreshold(1E-24f);
            mTimeOutCount = 0;

            mTTS = BYOS.Instance.Interaction.TextToSpeech;

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
                "questionlangage",
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
                if (/*!mSpeechToText.HasFinished*/ !mVocalManager.RecognitionFinished && mStartSTTCoroutine)
                {
                    OnSphinxTrigger();

                    yield return new WaitForSeconds(0.5f);
                    yield return new WaitUntil(() => /*mSpeechToText.HasFinished*/mVocalManager.RecognitionFinished);
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
                BYOS.Instance.Interaction.BMLManager.StopAllBehaviors();
                BYOS.Instance.Interaction.BMLManager.LaunchByName("Reset01");
                mIdleBehaviour.headPoseInit = true;
            }
        }

        public void SpeechToTextCallback(VoconResult iSpeech)
        {
            if (!string.IsNullOrEmpty(iSpeech.Utterance))
            {
                Debug.LogFormat("VOCON [EXCENTER][QUESTIONBEHAVIOUR] SpeechToText : {0} | Confidence : {1} | StartRule : {2}", iSpeech.Utterance, iSpeech.Confidence, iSpeech.StartRule);
                bool lClauseFound = false;
                string lKey = "";
                foreach (string lElement in mKeyList)
                {
                    string[] lPhonetics = BYOS.Instance.Dictionary.GetPhoneticStrings(lElement);
                    foreach (string lClause in lPhonetics)
                    {
                        if (iSpeech.Utterance.Contains(lClause))
                        {
                            if (ExperienceCenterData.Instance.EnableHeadMovement)
                                mAttitudeBehaviour.MoveHeadWhileSpeaking(-10, 10);
                            mTTS.SayKey(lElement, true);
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
            yield return new WaitUntil(() => mTTS.HasFinishedTalking);
            if (key == "idlesee")
                mAnimatorManager.ActivateCmd((byte)(Command.Welcome));
            else if (key == "idleleg")
                mAnimatorManager.ActivateCmd((byte)(Command.ByeBye));
        }

        public void StopBehaviour()
        {
            Debug.LogWarning("[EXCENTER][QUESTIONBEHAVIOUR] Stop Question Behaviour");

            if (!mTTS.HasFinishedTalking)
                mTTS.Stop();
            behaviourEnd = true;

            //mSpeechToText.Stop();
            mVocalManager.StopAllCoroutines();
            this.StopAllCoroutines();

            mVocalManager.RemoveGrammar("experiencecenter", LoadContext.APP);
            mVocalManager.UseVocon = false;
        }

        private IEnumerator EnableSpeechToText()
        {
            mVocalManager.EnableTrigger = false;
            //mSphinxTrigger.StopRecognition();
            mStartSTTCoroutine = false;
            mRestartSTT = true;
            mLaunchSTTOnce = false;
            while (mRestartSTT)
            {
                if (!mLaunchSTTOnce)
                {
                    if (/*!mSpeechToText.HasFinished*/ !mVocalManager.RecognitionFinished)
                    {
                        // Recognition not finished yet, waiting until it ends cleanly
                        yield return new WaitUntil(() => /*!mSpeechToText.HasFinished*/mVocalManager.RecognitionFinished);
                    }
                    else if (!mTTS.HasFinishedTalking)
                    {
                        // Buddy is answering, wait until he ends its sentence
                        yield return new WaitUntil(() => mTTS.HasFinishedTalking);
                    }
                    else
                    {
                        // Initiating Vocal Manager instance reco
                        yield return new WaitForSeconds(2.0f);
                        mLaunchSTTOnce = true;
                        //mSpeechToText.Request();
                        mVocalManager.StartInstantReco();
                    }
                }
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForSeconds(1.0f);
            mStartSTTCoroutine = true;
            //mSphinxTrigger.LaunchRecognition();
            //mVocalManager.EnableTrigger = true;
        }

        public void ErrorCallback(STTError iError)
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
