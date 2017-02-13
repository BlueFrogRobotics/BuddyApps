using UnityEngine;
using System.Collections;
using BuddyOS;
using BuddyOS.Command;

namespace BuddyApp.Jukebox
{
    public class JukeboxQuit : MonoBehaviour
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
