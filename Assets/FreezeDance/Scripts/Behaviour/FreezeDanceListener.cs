using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS;
using BuddyOS.Command;

namespace BuddyApp.FreezeDance
{
    public class FreezeDanceListener : MonoBehaviour
    {
        [SerializeField]
        private Text answerText;

        [SerializeField]
        private GameObject behave;

        [SerializeField]
        private GameObject startScreen;

        [SerializeField]
        private GameObject pauseScreen;

        [SerializeField]
        private GameObject screenManager;

        private SpeechToText mSTT;
        private TextToSpeech mTTS;

        // Use this for initialization
        void Awake()
        {
            mSTT = BYOS.Instance.SpeechToText;
            mTTS = BYOS.Instance.TextToSpeech;
        }

        void Start()
        {
            mSTT.OnBestRecognition.Add(VocalFreezeDanceInteraction);
            StartCoroutine(StartListenerAfterDelay());
        }

        // Update is called once per frame
        void Update()
        {
            answerText.text = mSTT.LastAnswer;
        }

        public void VocalFreezeDanceInteraction(string iMsg)
        {
            if (startScreen.activeSelf) {
                if (iMsg.ToLower().Contains("jouer")) {
                    behave.GetComponent<MotionGameBehave>().StartMusic();
                    screenManager.GetComponent<FadeManager>().StartFade();
                } else if (iMsg.ToLower().Contains("quitter")) {
                    new HomeCmd().Execute();
                } else {
                    mTTS.Say("je n'est pas compris");
                    StartCoroutine(StartSTT());
                }
            } else if (pauseScreen.activeSelf) {
                if (iMsg.ToLower().Contains("oui"))
                    behave.GetComponent<MotionGameBehave>().Restart();
                else if (iMsg.ToLower().Contains("non"))
                    new HomeCmd().Execute();
                else {
                    mTTS.Say("je n'ai pas compris");
                    StartCoroutine(StartSTT());
                }
            }
        }
        public void STTRequest()
        {
            mSTT.Request();
        }

        private IEnumerator StartSTT()
        {
            yield return new WaitForSeconds(2f);
            mSTT.Request();
        }

        private IEnumerator StartListenerAfterDelay()
        {
            yield return new WaitForSeconds(3f);
            mTTS.Say("Que Veux tu faire?");
            StartCoroutine(StartSTT());
        }
    }
}