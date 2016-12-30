using UnityEngine;
using System.Collections;
using BuddyOS;
using BuddyOS.Command;
using UnityEngine.UI;

namespace BuddyApp.RLGL
{
    public class RLGLListener : MonoBehaviour
    {
        TextToSpeech mTTS;
        SpeechToText mSTT;
        Face mFace;

        NotificationManager mNotif;

        [SerializeField]
        private GameObject gameplay;

        [SerializeField]
        private GameObject background;

        [SerializeField]
        private GameObject windowMenu;

        [SerializeField]
        private GameObject windowQuestion;

        [SerializeField]
        private GameObject windowTuto;

        private string mLastSpeech;
        public string LastSpeech { get { return mLastSpeech; } set { mLastSpeech = value; } }

        private bool mNeedListen;
        public bool NeedListen { get { return mNeedListen; } set { mNeedListen = value; } }

        private int mErrorCount;
        public int ErrorCount { get { return mErrorCount; } set { mErrorCount = value; } }

        private int mIndex;
        
        void Awake()
        {
            mIndex = 0;
            mTTS = BYOS.Instance.TextToSpeech;
            mSTT = BYOS.Instance.SpeechToText;
            mFace = BYOS.Instance.Face;
            mNotif = BYOS.Instance.NotManager;
            mErrorCount = 0;
        }
        // Use this for initialization
        void Start()
        {
            mNeedListen = true;
            mSTT.OnBestRecognition.Add(FunctionTocallWhenBestRekon);
            mSTT.OnErrorEnum.Add(ErrorSTT);
            //StartCoroutine(StartRequestAfterDelay(5F));
        }

        void ErrorSTT(STTError iError)
        {


            if (mErrorCount == 2)
            {
                mFace.SetExpression(MoodType.SAD);
            }
            else if (mErrorCount == 5)
            {
                mTTS.Say("Ok I'll do others things, we will play together later!");
                new HomeCmd().Execute();
                //BYOS.Instance.AppManager.Quit();
                return;
            }

            if (gameplay.activeSelf)
            {
                if (gameplay.GetComponent<RLGLBehaviour>().IsClicked)
                {
                    gameplay.GetComponent<RLGLBehaviour>().IsClicked = false;
                    return;
                }
            }

            Debug.Log("ERROR STT : " + iError);
            string lError = "";
            if (gameplay.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("RulesState"))
            {
                switch (iError)
                {
                    case STTError.ERROR_SPEECH_TIMEOUT:
                        lError = "Can you repeat please?";
                        gameplay.GetComponent<Animator>().GetBehaviour<RulesState>().NeedListen = true;
                        ++mErrorCount;
                        break;
                    case STTError.ERROR_NO_MATCH:
                        lError = "Maybe you should click on my face";
                        gameplay.GetComponent<Animator>().GetBehaviour<RulesState>().NeedListen = true;
                        break;
                    default:
                        gameplay.GetComponent<Animator>().GetBehaviour<RulesState>().NeedListen = true;
                        break;
                }

                if (UnityEngine.Random.value > 0.8)
                {
                    mTTS.Say(lError);
                }
            }
            else if (gameplay.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Start"))
            {
                switch (iError)
                {
                    case STTError.ERROR_NO_MATCH:
                        lError = "Maybe you should click on my face";
                        gameplay.GetComponent<Animator>().GetBehaviour<StartState>().NeedListen = true;
                        break;
                    case STTError.ERROR_SPEECH_TIMEOUT:
                        lError = "Why nobody answer me?";
                        gameplay.GetComponent<Animator>().GetBehaviour<StartState>().NeedListen = true;
                        ++mErrorCount;
                        break;
                    default:
                        gameplay.GetComponent<Animator>().GetBehaviour<StartState>().NeedListen = true;
                        break;

                }

                if (UnityEngine.Random.value > 0.8)
                {
                    mTTS.Say(lError);
                }
            }
            else if (gameplay.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("ReplayState"))
            {
                switch (iError)
                {
                    case STTError.ERROR_NO_MATCH:
                        lError = "Maybe you should click on my face";
                        gameplay.GetComponent<Animator>().GetBehaviour<ReplayState>().NeedListen = true;
                        break;
                    case STTError.ERROR_SPEECH_TIMEOUT:
                        lError = "Why nobody answer me?";
                        gameplay.GetComponent<Animator>().GetBehaviour<ReplayState>().NeedListen = true;
                        ++mErrorCount;
                        break;
                    default:
                        gameplay.GetComponent<Animator>().GetBehaviour<ReplayState>().NeedListen = true;
                        break;
                }

                if (UnityEngine.Random.value > 0.8)
                {
                    mTTS.Say(lError);
                }
            }
            else if (mIndex == 5)
            {
                if (gameplay.GetComponent<RLGLBehaviour>().IsClicked)
                {
                    gameplay.GetComponent<RLGLBehaviour>().IsClicked = false;
                    return;
                }
                switch (iError)
                {
                    case STTError.ERROR_NO_MATCH:
                        lError = "Maybe you should click on my face";
                        //windowMenu.GetComponent<RLGLMenu>().STTNotif = mSTT.LastAnswer;
                        windowMenu.GetComponent<RLGLMenu>().NeedListen = true;
                        break;
                    case STTError.ERROR_SPEECH_TIMEOUT:
                        lError = "Why nobody answer me?";
                        windowMenu.GetComponent<RLGLMenu>().NeedListen = true;
                        ++mErrorCount;
                        break;
                    case STTError.ERROR_NETWORK:
                        lError = "There is some problems with the wifi. Touch the menu!";
                        windowMenu.GetComponent<RLGLMenu>().NeedListen = true;
                        break;
                    default:
                        windowMenu.GetComponent<RLGLMenu>().NeedListen = true;
                        break;
                }

                if (UnityEngine.Random.value > 0.8)
                {
                    mTTS.Say(lError);
                }
            }
            //StartCoroutine(StartRequestAfterDelay(5.0F));
        }

        void FunctionTocallWhenBestRekon(string iMsg)
        {
            Debug.Log(iMsg);
            mLastSpeech = iMsg;
            if (iMsg.ToLower().Contains("play") && windowMenu.activeSelf && mIndex == 5)
            {
                if (gameplay.GetComponent<RLGLBehaviour>().IsClicked)
                {
                    gameplay.GetComponent<RLGLBehaviour>().IsClicked = false;
                    return;
                }
                    
                windowMenu.GetComponent<RLGLMenu>().IsAnswerPlayYes = true;
                //background.GetComponent<Animator>().SetTrigger("Close_BG");
                //windowMenu.GetComponent<Animator>().SetTrigger("Close_WMenu3");
                //gameplay.SetActive(true);
            }
            else if ((iMsg.ToLower().Contains("tutorial") || (iMsg.ToLower().Contains("tutoriel"))) && windowMenu.activeSelf && mIndex == 5)
            {
                if (gameplay.GetComponent<RLGLBehaviour>().IsClicked)
                {
                    gameplay.GetComponent<RLGLBehaviour>().IsClicked = false;
                    return;
                }

                //windowMenu.GetComponent<RLGLMenu>().IsAnswerPlayYes = true;
                windowMenu.GetComponent<Animator>().SetTrigger("Close_WMenu3");
                windowTuto.GetComponent<Animator>().SetTrigger("Open_WTuto");
            }
            else if (iMsg.ToLower().Contains("quit") || iMsg.ToLower().Contains("close"))
                new HomeCmd().Execute();
            else if (iMsg.ToLower().Contains("yes") && mIndex == 0)
            {
                if (gameplay.GetComponent<RLGLBehaviour>().IsClicked)
                {
                    gameplay.GetComponent<RLGLBehaviour>().IsClicked = false;
                    return;
                }
                gameplay.GetComponent<Animator>().GetBehaviour<StartState>().IsAnswerYes = true;
            }
            else if (iMsg.ToLower().Contains("no") && mIndex == 0)
            {
                if (gameplay.GetComponent<RLGLBehaviour>().IsClicked)
                {
                    gameplay.GetComponent<RLGLBehaviour>().IsClicked = false;
                    return;
                }
                gameplay.GetComponent<Animator>().GetBehaviour<StartState>().IsAnswerNo = true;
            }
            else if (iMsg.ToLower().Contains("yes") && mIndex == 1)
            {
                if (gameplay.GetComponent<RLGLBehaviour>().IsClicked)
                {
                    gameplay.GetComponent<RLGLBehaviour>().IsClicked = false;
                    return;
                }
                gameplay.GetComponent<Animator>().GetBehaviour<RulesState>().IsAnswerRuleYes = true;
                
            }
            else if (iMsg.ToLower().Contains("yes") && mIndex == 1)
            {
                if (gameplay.GetComponent<RLGLBehaviour>().IsClicked)
                {
                    gameplay.GetComponent<RLGLBehaviour>().IsClicked = false;
                    return;
                }
                gameplay.GetComponent<Animator>().GetBehaviour<RulesState>().IsAnswerRuleNo = true;
            }
            else if (iMsg.ToLower().Contains("yes") && mIndex == 2)
            {
                if (gameplay.GetComponent<RLGLBehaviour>().IsClicked)
                {
                    gameplay.GetComponent<RLGLBehaviour>().IsClicked = false;
                    return;
                }
                gameplay.GetComponent<Animator>().GetBehaviour<ReplayState>().IsAnswerReplayYes = true;
            }
            else if (iMsg.ToLower().Contains("no") && mIndex == 2)
            {
                if (gameplay.GetComponent<RLGLBehaviour>().IsClicked)
                {
                    gameplay.GetComponent<RLGLBehaviour>().IsClicked = false;
                    return;
                }
                gameplay.GetComponent<Animator>().GetBehaviour<ReplayState>().IsAnswerReplayNo = true;
            }
            else
            {
                if (gameplay.GetComponent<RLGLBehaviour>().IsClicked)
                {
                    gameplay.GetComponent<RLGLBehaviour>().IsClicked = false;
                    return;
                }
                mTTS.Say("Can you repeat please, I don't understand what you said");
                if(gameplay.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("RulesState"))
                    gameplay.GetComponent<Animator>().GetBehaviour<RulesState>().NeedListen = true;
                else if (gameplay.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Start"))
                    gameplay.GetComponent<Animator>().GetBehaviour<StartState>().NeedListen = true;
                else if (gameplay.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("ReplayState"))
                    gameplay.GetComponent<Animator>().GetBehaviour<ReplayState>().NeedListen = true;
                else if (mIndex == 5)
                {
                    //windowMenu.GetComponent<RLGLMenu>().STTNotif = iMsg;
                    //mNotif.Display<SimpleNot>(2.5F).With(iMsg);
                    windowMenu.GetComponent<RLGLMenu>().NeedListen = true;
                }
                
                //StartCoroutine(StartRequestAfterDelay(5.0F));
            }

        }
        public void StartRequest()
        {
            mSTT.Request();
        }

        IEnumerator StartRequestAfterDelay(float iDelay)
        {
            yield return new WaitForSeconds(iDelay);
            mSTT.Request();
        }

        public void STTRequest(int iIndex)
        {
            mIndex = iIndex;
            mSTT.Request();
        }
    }
}
