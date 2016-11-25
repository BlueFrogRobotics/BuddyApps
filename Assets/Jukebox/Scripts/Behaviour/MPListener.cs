using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS;
using BuddyOS.Command;

namespace BuddyApp.Jukebox
{
    public class MPListener : MonoBehaviour
    {
        [SerializeField]
        private Button playButton;

        private TextToSpeech mTTS;
        private SpeechToText mSTT;

        // Use this for initialization
        void Start()
        {
            mTTS = BYOS.Instance.TextToSpeech;
            mSTT = BYOS.Instance.SpeechToText;

            mSTT.OnBestRecognition.Add(FunctionTocallWhenBestRekon);
            StartCoroutine(StartRequestAfterDelay(5f));
        }

        private void FunctionTocallWhenBestRekon(string iMsg)
        {
            if (iMsg.ToLower().Contains("jouer")) {
                //TODO
                playButton.onClick.Invoke();
            } else if (iMsg.ToLower().Contains("quitter"))
                new UnLoadAppCmd().Execute();
            else {
                mTTS.Say("Je n'ai pas compris, veux tu répéter?");
                StartCoroutine(StartRequestAfterDelay(2f));
            }
        }

        private void StartRequest()
        {
            mSTT.Request();
        }

        private IEnumerator StartRequestAfterDelay(float iDelay)
        {
            yield return new WaitForSeconds(iDelay);
            mSTT.Request();
        }
    }
}