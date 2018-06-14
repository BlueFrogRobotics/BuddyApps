using UnityEngine;
using Buddy;

namespace BuddyApp.Weather
{
    public class VocalW : AStateMachineBehaviour
    {

        private bool mListening;

        public override void Start()
        {
            mWeatherB = GetComponent<WeatherBehaviour>();
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mListening = false;
            //if (!string.IsNullOrEmpty(WeatherData.Instance.VocalRequest))
            //Trigger("Vocal");

            //Use vocon
            Interaction.VocalManager.UseVocon = true;
            Interaction.VocalManager.AddGrammar("weather", LoadContext.APP);
            Interaction.VocalManager.OnVoconBest = VoconBest;
            Interaction.VocalManager.OnVoconEvent = EventVocon;

            //Interaction.VocalManager.OnEndReco = GetAnswer;
            //Interaction.VocalManager.OnError = NoAnswer;
        }

        private void EventVocon(VoconEvent iEvent)
        {
            Debug.Log(iEvent);
        }

        private void VoconBest(VoconResult iBestResult)
        {
            Debug.Log(iBestResult.Utterance);
            WeatherData.Instance.VocalRequest = iBestResult.Utterance;
            Interaction.Mood.Set(MoodType.NEUTRAL);

            mListening = false;
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mListening)
                return;
            if (!mListening && string.IsNullOrEmpty(WeatherData.Instance.VocalRequest))
            {
                Interaction.VocalManager.StartInstantReco();
                mListening = true;
                return;
            }

            Trigger("Vocal");
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

            Interaction.VocalManager.UseVocon = false;
            Interaction.VocalManager.RemoveGrammar("weather", LoadContext.APP);
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
        }

    }
}
