using UnityEngine;
using System.Collections;
using Buddy.Command;
using Buddy;

namespace BuddyApp.RedLightGreenLight
{
    public class RLGLQuit : MonoBehaviour
    {
        private TextToSpeech mTTS;
        void Start()
        {
            mTTS = BYOS.Instance.Interaction.TextToSpeech;
        }
        public void QuitApplication()
        {
            Debug.Log("Quit app");
            BYOS.Instance.Interaction.TextToSpeech.Silence(0);
            RedLightGreenLightActivity.QuitApp();
        }
    }
}

