using UnityEngine;
using BuddyOS;
using BuddyOS.Command;
using System.Collections;


namespace BuddyApp.Companion
{
    public class SayHelloReaction : MonoBehaviour
    {
        private float mTime;
        private Dictionary mDictionary;
        private NoHinge mNoHinge;
        private TextToSpeech mTTS;
        private Wheels mWheels;
        private YesHinge mYesHinge;

        void Start()
        {
            mDictionary = BYOS.Instance.Dictionary;
            mNoHinge = BYOS.Instance.Motors.NoHinge;
            mTTS = BYOS.Instance.TextToSpeech;
            mWheels = BYOS.Instance.Motors.Wheels;
            mYesHinge = BYOS.Instance.Motors.YesHinge;
        }

        void OnEnable()
        {
            StartCoroutine(StepBackCo());
            mTime = Time.time;
        }

        private IEnumerator StepBackCo()
        {
            mWheels.SetWheelsSpeed(-200F, -200F, 500);

            yield return new WaitForSeconds(1F);
            
            mNoHinge.SetPosition(0F);
            mYesHinge.SetPosition(-10F);

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