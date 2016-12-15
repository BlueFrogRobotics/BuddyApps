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

        [SerializeField]
        private GameObject Gameplay;

        [SerializeField]
        private GameObject Background;

        [SerializeField]
        private GameObject WindowMenu;

        [SerializeField]
        private GameObject WindowQuestion;

        private int mIndex;
        
        void Awake()
        {
            mIndex = 0;
            mTTS = BYOS.Instance.TextToSpeech;
            mSTT = BYOS.Instance.SpeechToText;
        }
        // Use this for initialization
        void Start()
        {
            mSTT.OnBestRecognition.Add(FunctionTocallWhenBestRekon);
            //StartCoroutine(StartRequestAfterDelay(5F));
        }
        void FunctionTocallWhenBestRekon(string iMsg)
        {
            if (iMsg.ToLower().Contains("jouer") && WindowMenu.activeSelf && mIndex == 5)
            {
                WindowMenu.GetComponent<RLGLMenu>().IsAnswerPlayYes = true;
                Background.GetComponent<Animator>().SetTrigger("Close_BG");
                WindowMenu.GetComponent<Animator>().SetTrigger("Close_WMenu3");
                Gameplay.SetActive(true);
                //WindowMenu.SetActive(false);
            }
            else if (iMsg.ToLower().Contains("quitter"))
                new HomeCmd().Execute();
            else if (iMsg.ToLower().Contains("oui") && mIndex == 0)
            {
                //Gameplay.GetComponent<Animator>().SetBool("IsReplayDone", true);
                Gameplay.GetComponent<Animator>().GetBehaviour<StartState>().IsAnswerYes = true;
                Background.GetComponent<Animator>().SetTrigger("Close_BG");
                WindowQuestion.GetComponent<Animator>().SetTrigger("Close_WQuestion");
            }
            else if (iMsg.ToLower().Contains("non") && mIndex == 0)
            {
                //Gameplay.GetComponent<Animator>().SetBool("IsReplayDone", true);
                Gameplay.GetComponent<Animator>().GetBehaviour<StartState>().IsAnswerNo = true;
                Background.GetComponent<Animator>().SetTrigger("Close_BG");
                WindowQuestion.GetComponent<Animator>().SetTrigger("Close_WQuestion");
            }
            else if (iMsg.ToLower().Contains("oui") && mIndex == 1)
            {
                //Gameplay.GetComponent<Animator>().SetBool("IsReplayDone", true);
                Debug.Log("OUI DANS LE RULE STATE");
                Gameplay.GetComponent<Animator>().GetBehaviour<RulesState>().IsAnswerRuleYes = true;
                Background.GetComponent<Animator>().SetTrigger("Close_BG");
                WindowQuestion.GetComponent<Animator>().SetTrigger("Close_WQuestion");
            }
            else if (iMsg.ToLower().Contains("non") && mIndex == 1)
            {
                //Gameplay.GetComponent<Animator>().SetBool("IsReplayDone", true);
                Gameplay.GetComponent<Animator>().GetBehaviour<RulesState>().IsAnswerRuleNo = true;
                Background.GetComponent<Animator>().SetTrigger("Close_BG");
                WindowQuestion.GetComponent<Animator>().SetTrigger("Close_WQuestion");
            }
            else
            {
                mTTS.Say("Je n'ai pas compris, veux tu répéter?");
                StartCoroutine(StartRequestAfterDelay(5.0F));
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
