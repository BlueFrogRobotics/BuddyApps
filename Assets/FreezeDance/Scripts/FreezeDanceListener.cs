using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS;

public class FreezeDanceListener : MonoBehaviour {
    private SpeechToText mSTT;
    private TextToSpeech mTTS;
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

//    private BuddyFeature.Face.Regular.FaceManager mFace;
    // Use this for initialization
    void Awake()
    {
        mSTT = BYOS.Instance.SpeechToText;
        mTTS = BYOS.Instance.TextToSpeech;
    }
    void Start ()
    {
        mSTT.OnBestRecognition.Add(VocalFreezeDanceInteraction);
        HasStartedFirstRequest = false;
        StartCoroutine(StartListenerAfterDelay());
	}
	public void VocalFreezeDanceInteraction(string iMsg)
    {
        if (mStartScreen.activeSelf)
        {
            if (iMsg.ToLower().Contains("jouer"))
            {
                Behave.GetComponent<MotionGameBehave>().StartMusic();
                mScreenManager.GetComponent<FadeManager>().StartFade();
            }
            else if (iMsg.ToLower().Contains("quitter"))
            {
                UnLoadAppCmd.Create().Execute();
                //HomeCmd.Create().Execute();
            }
            else
            {
                mTTS.Say("je n'est pas compris");
                StartCoroutine(StartSTT());
            }
        }
        else if (mPauseScreen.activeSelf)
        {
            if (iMsg.ToLower().Contains("oui"))
                Behave.GetComponent<MotionGameBehave>().Restart();
            else if (iMsg.ToLower().Contains("non"))
                UnLoadAppCmd.Create().Execute();
            else
            {
                mTTS.Say("je n'ai pas compris");
                StartCoroutine(StartSTT());
            }
        }
    }
    public void STTRequest()
    {
        mSTT.Request();
    }
    // Update is called once per frame
    void Update ()
    {
        //BuddyOS.BuddyOperatingSystem.Instance.FaceManager.Speak(mTTS.IsSpeaking());
        mAnswerText.text = mSTT.LastAnswer;
        //if (mSTT.HasFinished() && !mTTS.IsSpeaking() && HasStartedFirstRequest && (mPauseScreen.activeSelf || mStartScreen.activeSelf))
        //    mSTT.Request();
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
