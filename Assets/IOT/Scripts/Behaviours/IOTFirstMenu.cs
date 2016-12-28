using UnityEngine;
using System.Collections;
using BuddyOS;

namespace BuddyApp.IOT
{
    public class IOTFirstMenu : MonoBehaviour
    {
        [SerializeField]
        private Animator IABehaviour;
        private SpeechToText mSTT;
        private TextToSpeech mTTS;

        private int mAsking = 0;

        void OnEnable()
        {
            mSTT = BYOS.Instance.SpeechToText;
            mTTS = BYOS.Instance.TextToSpeech;

            mSTT.OnBestRecognition.Add(ParseMsg);
            mAsking = 0;
        }

        private void ParseMsg(string iMsg)
        {
            iMsg = iMsg.ToLower();
            if (iMsg.Contains("connected") || iMsg.Contains("object") || iMsg.Contains("objet"))
                IABehaviour.SetInteger("Choice", 0);
            else if (iMsg.Contains("account") || iMsg.Contains("compte"))
                mTTS.Say("this feature is not implemented yet");
        }
        // Update is called once per frame
        void Update()
        {
            if (mTTS.HasFinishedTalking)
            {
                if (mSTT.HasFinished && mAsking < 3)
                {
                    mAsking++;
                    mSTT.Request();
                }
            }
        }
    }
}
