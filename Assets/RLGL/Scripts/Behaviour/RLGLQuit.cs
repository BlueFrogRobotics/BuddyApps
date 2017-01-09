using UnityEngine;
using System.Collections;
using BuddyOS.Command;
using BuddyOS;

namespace BuddyApp.RLGL
{
    public class RLGLQuit : MonoBehaviour
    {
        private TextToSpeech mTTS;
        void Start()
        {
            mTTS = BYOS.Instance.TextToSpeech;
        }
        public void QuitApplication()
        {
            Debug.Log("Quit app");
            mTTS.Silence(0);
            new HomeCmd().Execute();
        }
    }
}

