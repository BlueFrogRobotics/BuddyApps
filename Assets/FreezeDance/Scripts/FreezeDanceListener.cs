using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS;
using BuddyOS.Command;

public class FreezeDanceListener : MonoBehaviour
{
    [SerializeField]
    private Text mAnswerText;

    [SerializeField]
    private GameObject Behave;

    [SerializeField]
    private GameObject mStartScreen;

    [SerializeField]
    private GameObject mPauseScreen;

    [SerializeField]
    private GameObject mScreenManager;

    private bool HasStartedFirstRequest;
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
        HasStartedFirstRequest = false;
        StartCoroutine(StartListenerAfterDelay());
    }

    // Update is called once per frame
    void Update()
    {
        mAnswerText.text = mSTT.LastAnswer;
    }

    public void VocalFreezeDanceInteraction(string iMsg)
    {
        if (mStartScreen.activeSelf) {
            if (iMsg.ToLower().Contains("jouer")) {
                Behave.GetComponent<MotionGameBehave>().StartMusic();
                mScreenManager.GetComponent<FadeManager>().StartFade();
            } else if (iMsg.ToLower().Contains("quitter")) {
                new HomeCmd().Execute();
            } else {
                mTTS.Say("je n'est pas compris");
                StartCoroutine(StartSTT());
            }
        } else if (mPauseScreen.activeSelf) {
            if (iMsg.ToLower().Contains("oui"))
                Behave.GetComponent<MotionGameBehave>().Restart();
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

    IEnumerator StartSTT()
    {
        yield return new WaitForSeconds(2f);
        mSTT.Request();
    }

    IEnumerator StartListenerAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        mTTS.Say("Que Veux tu faire?");
        StartCoroutine(StartSTT());
        HasStartedFirstRequest = true;
    }
}
