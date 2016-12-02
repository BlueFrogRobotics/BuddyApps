using UnityEngine;
using BuddyOS;
using BuddyOS.Command;
using System.Collections;

namespace BuddyApp.Companion
{
    internal delegate void ReactionFinished();

    public class Reaction : MonoBehaviour
    {
        internal ReactionFinished ActionFinished;
        private TextToSpeech mTTS;

        void Start()
        {
            mTTS = BYOS.Instance.TextToSpeech;
        }

        void Update()
        {

        }

        public void StopWheels()
        {
            new SetWheelsSpeedCmd(0F, 0F).Execute();
        }

        public void StepBackHelloReaction()
        {
            Debug.Log("Someone is there. I say hello !");
            new SetWheelsSpeedCmd(-200F, -200F, 60).Execute();
            new SetPosYesCmd(0).Execute();
            mTTS.Say("Bonjour !");
            ActionFinished();
        }

        public void Pout()
        {
            Debug.Log("Face smashed ! I will pout");
            StartCoroutine(PoutCo());
        }

        public IEnumerator PoutCo()
        {
            new SetMoodFaceCmd(FaceMood.ANGRY).Execute();
            new SetWheelsSpeedCmd(200F, -200F, 90).Execute();
            yield return new WaitForSeconds(5F);

            new SetMoodFaceCmd(FaceMood.GRUMPY).Execute();
            new SetWheelsSpeedCmd(200F, 200F, 600).Execute();
            yield return new WaitForSeconds(5F);

            new SetMoodFaceCmd(FaceMood.NEUTRAL).Execute();
            ActionFinished();
        }

        public void FollowFace()
        {

        }

        public IEnumerator FollowFaceCo()
        {
            //Write here some code to make sure that one face is centered in the camera
            yield return new WaitForSeconds(0.1F);
            ActionFinished();
        }
    }
}