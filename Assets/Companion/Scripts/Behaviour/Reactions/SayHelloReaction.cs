using UnityEngine;
using BuddyOS;
using BuddyOS.Command;
using System.Collections;


namespace BuddyApp.Companion
{
    public class SayHelloReaction : MonoBehaviour
    {
        private TextToSpeech mTTS;

        void Start()
        {
            mTTS = BYOS.Instance.TextToSpeech;
        }

        void OnEnable()
        {
            StartCoroutine(StepBackCo());
        }
            
        void Update()
        {

        }

        private IEnumerator StepBackCo()
        {
            new SetWheelsSpeedCmd(-200F, -200F, 300).Execute();

            yield return new WaitForSeconds(1F);

            new SetPosNoCmd(0F).Execute();
            new SetPosYesCmd(10F).Execute();

            yield return new WaitForSeconds(1F);

            mTTS.Say("Bonjour !");
        }
    }
}