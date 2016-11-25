using UnityEngine;
using System.Collections;
using BuddyOS;

namespace BuddyApp.RLGL
{
    public class RestartScreenRLGL : MonoBehaviour
    {

        [SerializeField]
        private GameObject mListener;
        private TextToSpeech mTTS;

        void OnEnable()
        {
            mTTS = BYOS.Instance.TextToSpeech;
            mTTS.Say("Veux tu rejouer?");
            StartCoroutine(StartRequest());
        }
        IEnumerator StartRequest()
        {
            mTTS = BYOS.Instance.TextToSpeech;
            yield return new WaitForSeconds(2f);
            mListener.GetComponent<BuddyApp.RLGL.RLGLListener>().STTRequest();
        }
    }
}

