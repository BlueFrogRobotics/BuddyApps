using UnityEngine;
using Buddy;

namespace BuddyApp.Weather
{
    public class VocalW : AStateMachineBehaviour
    {

        public override void Start()
        {
            mWeatherB = GetComponent<WeatherBehaviour>();
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("ENTER LISTEN test");
            //if (!string.IsNullOrEmpty(WeatherData.Instance.VocalRequest))
            //Trigger("Vocal");
            Interaction.VocalManager.OnEndReco = GetAnswer;
            Interaction.VocalManager.OnError = NoAnswer;
            Interaction.VocalManager.StartInstantReco();

        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("EXIT LISTEN");
        }

        private void GetAnswer(string iAnswer)
        {
            if (mWeatherB.mIsOk)
            {
                Utils.LogI(LogContext.APP, "GOT AN ANSWER: " + iAnswer);
                WeatherData.Instance.VocalRequest = iAnswer;
                Trigger("Vocal");
            }
        }

        private void NoAnswer(STTError iError)
        {
            Utils.LogI(LogContext.APP, "VM error");
            Debug.Log("GOT NO ANSWER");
        }

    }
}
