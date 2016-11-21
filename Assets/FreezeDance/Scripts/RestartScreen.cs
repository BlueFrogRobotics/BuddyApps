using UnityEngine;
using System.Collections;
using BuddyOS;

public class RestartScreen : MonoBehaviour {

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
        mListener.GetComponent<FreezeDanceListener>().STTRequest();
    }
}
