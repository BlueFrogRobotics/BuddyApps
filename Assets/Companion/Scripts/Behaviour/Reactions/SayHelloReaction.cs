using UnityEngine;
using BuddyOS;
using BuddyOS.Command;
using System.Collections;


namespace BuddyApp.Companion
{
    public class SayHelloReaction : MonoBehaviour
    {
        private Dictionary mDictionary;
        private TextToSpeech mTTS;
        private float mTime;

        void Start()
        {
            mDictionary = BYOS.Instance.Dictionary;
            mTTS = BYOS.Instance.TextToSpeech;
        }

        void OnEnable()
        {
            StartCoroutine(StepBackCo());
            mTime = Time.time;
        }

        private IEnumerator StepBackCo()
        {
            new SetWheelsSpeedCmd(-200F, -200F, 500).Execute();

            yield return new WaitForSeconds(1F);

            new SetPosNoCmd(0F).Execute();
            new SetPosYesCmd(-10F).Execute();

            yield return new WaitForSeconds(1F);

            mTTS.Say(mDictionary.GetString("hello"));
        }

        void Update()
        {
            if (Time.time - mTime > 4F)
                enabled = false;
        }

        void OnDisable()
        {
            GetComponent<Reaction>().ActionFinished();
        }
    }
}