using UnityEngine;
using System.Collections;
using BuddyOS;

public class alarmListener : MonoBehaviour {

    private TextToSpeech mSynthesis;
    private SpeechToText mRecognition;

    public UnityEngine.UI.Text debugText;

    [SerializeField]
    private counter mCounter;


    void Awake()
    {
        mSynthesis = BYOS.Instance.TextToSpeech;
        mRecognition = BYOS.Instance.SpeechToText;
    }

    // Use this for initialization
    void Start () {

        mSynthesis = new TextToSpeech();
        mRecognition = new SpeechToText();
        mRecognition.OnBestRecognition.Add(callBackRecognition);
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    void callBackRecognition(string iMsg)
    {
        debugText.text = iMsg;
        // here we parse the result in order to use it
        int timeForCounter = 0;
        string[] words = iMsg.Split(' ');
        for (int i = 1; i < words.Length;i++ )
        {
            if (words[i].ToLower().Contains("seconde") || words[i].ToLower().Contains("secondes"))
            {
                int valueInSeconds = 0;
                if(words[i-1].ToLower().Contains("une"))
                {
                    timeForCounter = timeForCounter + 1;
                }
                if(System.Int32.TryParse(words[i-1], out valueInSeconds))
                {
                    timeForCounter = timeForCounter + valueInSeconds;
                }
            }
            else if (words[i].ToLower().Contains("minute") || words[i].ToLower().Contains("minutes"))
            {
                int valueInMinute = 0;
                if (words[i - 1].ToLower().Contains("une"))
                {
                    timeForCounter = timeForCounter + 60;
                }
                if (System.Int32.TryParse(words[i - 1], out valueInMinute))
                {
                    timeForCounter = timeForCounter + valueInMinute*60;
                }
            }
            else if (words[i].ToLower().Contains("heure") || words[i].ToLower().Contains("heures"))
            {
                int valueInHeure = 0;
                if (words[i - 1].ToLower().Contains("une"))
                {
                    timeForCounter = timeForCounter + 3600;
                }
                if (System.Int32.TryParse(words[i - 1], out valueInHeure))
                {
                    timeForCounter = timeForCounter + valueInHeure * 3600;
                }
            }
        }
        if(timeForCounter == 0)
        {
            askQuestion("Je n'ai pas compris, peux tu repeter ? ");
            return;
        }
        Debug.Log("test");

        say("Parfait, Je te reveil dans " + timeForCounter.ToString() + " secondes");
        mCounter.setCounter(timeForCounter);
        mCounter.startCounter();
    }
    public void STTRequest()
    {
        mRecognition.Request();
    }

    IEnumerator StartSTT()
    {
        yield return new WaitForSeconds(4f);
        mRecognition.Request();
    }

    public void askQuestion(string iQuestion)
    {
        mSynthesis.Say(iQuestion);
        StartCoroutine(StartSTT());
    }

    public void say(string iSentence)
    {
        mSynthesis.Say(iSentence);
    }
}
