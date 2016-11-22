using UnityEngine;
using System.Collections;
using BuddyOS;

namespace BuddyApp.FreezeDance
{
    public class RestartScreen : MonoBehaviour
    {
        [SerializeField]
        private GameObject listener;

        private TextToSpeech mTTS;

        void OnEnable()
        {
            mTTS = BYOS.Instance.TextToSpeech;
            mTTS.Say("Veux tu rejouer?");
            StartCoroutine(StartRequest());
        }

        private IEnumerator StartRequest()
        {
            mTTS = BYOS.Instance.TextToSpeech;
            yield return new WaitForSeconds(2f);
            listener.GetComponent<FreezeDanceListener>().STTRequest();
        }
    }
}