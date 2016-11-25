using UnityEngine;
using System.Collections;
using BuddyOS;
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
        private GameObject WindowMenu;

        void Awake()
        {
            mTTS = BYOS.Instance.TextToSpeech;
            mSTT = BYOS.Instance.SpeechToText;
        }
        // Use this for initialization
        void Start()
        {
            mSTT.OnBestRecognition.Add(FunctionTocallWhenBestRekon);
            StartCoroutine(StartRequestAfterDelay(5f));
        }
        void FunctionTocallWhenBestRekon(string iMsg)
        {
            if (iMsg.ToLower().Contains("jouer"))
            {
                Gameplay.SetActive(true);
                WindowMenu.SetActive(false);
            }
            else if (iMsg.ToLower().Contains("quitter"))
                UnLoadAppCmd.Create().Execute();
            else if(iMsg.ToLower().Contains("oui"))
            {
                //Gameplay.GetComponent<Animator>().SetBool("IsReplayDone", true);
                Gameplay.GetComponent<Animator>().GetBehaviour<ReplayState>().IsAnswerYes = true;
            }
            else
            {
                mTTS.Say("Je n'ai pas compris, veux tu répéter?");
                StartCoroutine(StartRequestAfterDelay(2f));
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

        public void STTRequest()
        {
            mSTT.Request();
        }
    }
}
