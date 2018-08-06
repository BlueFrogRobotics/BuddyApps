using UnityEngine;
using BlueQuark;

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
            //Interaction.VocalManager.UseVocon = true;
            //Interaction.VocalManager.AddGrammar("weather", LoadContext.APP);
            //Interaction.VocalManager.OnVoconBest = VoconBest;
            //Interaction.VocalManager.OnVoconEvent = EventVocon;

            Buddy.Vocal.OnEndListening.Add((iInput) => { VoconBest(iInput); });
            Buddy.Vocal.OnListeningEvent.Add((iInput) => { EventVocon(iInput); });
            Buddy.Vocal.OnListeningEvent.Add((iInput) => { NoAnswer(iInput); });

            //Buddy.Vocal.Listen(NameVoconGrammarFile, SpeechRecognitionMode.OFFLINE_ONLY);

            //Interaction.VocalManager.OnEndReco = GetAnswer;
            //Interaction.VocalManager.OnError = NoAnswer;
        }

        private void EventVocon(SpeechEvent iEvent)
        {
            Debug.Log(iEvent);
        }

        private void VoconBest(SpeechInput iBestResult)
        {
            Debug.Log(iBestResult.Utterance);
            WeatherData.Instance.VocalRequest = iBestResult.Utterance;
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);

            mListening = false;
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mListening)
                return;
            if (!mListening && string.IsNullOrEmpty(WeatherData.Instance.VocalRequest)) {
                Buddy.Vocal.Listen("weather");
                mListening = true;
                return;
            }

            Trigger("Vocal");
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.Vocal.Stop();
            //Interaction.VocalManager.UseVocon = false;
            //Buddy.Vocal..RemoveGrammar("weather", LoadContext.APP);
        }

        private void GetAnswer(string iAnswer)
        {
            if (mWeatherB.mIsOk) {
                ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.SUCCESS, LogInfo.OUTPUT, "GOT AN ANSWER: " + iAnswer);
                WeatherData.Instance.VocalRequest = iAnswer;
                Trigger("Vocal");
            }
        }

        private void NoAnswer(SpeechEvent iEvent)
        {
            if (iEvent.IsError)
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.INFO, LogInfo.OUTPUT, "VM error...");
        }
    }
}
