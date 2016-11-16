using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS;

public class MPListener : MonoBehaviour {

    TextToSpeech mTTS;
    SpeechToText mSTT;
    [SerializeField]
    private Button mPlayButton;

    // Use this for initialization
    void Start()
    {
        mTTS = BYOS.Instance.TextToSpeech;
        mSTT = BYOS.Instance.SpeechToText;

        mSTT.OnBestRecognition.Add(FunctionTocallWhenBestRekon);
        StartCoroutine(StartRequestAfterDelay(5f));
    }
    void FunctionTocallWhenBestRekon(string iMsg)
    {
        if (iMsg.ToLower().Contains("jouer"))
        {
            //TODO
            mPlayButton.onClick.Invoke();
        }
        else if (iMsg.ToLower().Contains("quitter"))
            UnLoadAppCmd.Create().Execute();
        else
        {
            mTTS.Say("Je n'ai pas compris, veux tu répéter?");
            StartCoroutine(StartRequestAfterDelay(2f));
        }
    }
    void StartRequest()
    {
        mSTT.Request();
    }
    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator StartRequestAfterDelay(float iDelay)
    {
        yield return new WaitForSeconds(iDelay);
        mSTT.Request();
    }
}
