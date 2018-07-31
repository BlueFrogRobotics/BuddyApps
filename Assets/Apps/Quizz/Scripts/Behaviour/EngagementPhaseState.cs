using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;

namespace BuddyApp.Quizz
{
    /// <summary>
    /// State in which each player has to repeat the nickname to engage
    /// </summary>
    public class EngagementPhaseState : AStateMachineBehaviour
    {

        private QuizzBehaviour mQuizzBehaviour;
        private bool mHasSaidName = false;
        private bool mGoodName = false;
        private string mActualName = "";

        public override void Start()
        {
            mQuizzBehaviour = GetComponent<QuizzBehaviour>();
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("debut engagement");
            Interaction.VocalManager.UseVocon = true;
            Interaction.VocalManager.ClearGrammars();
            Interaction.VocalManager.AddGrammar("commands", Buddy.LoadContext.APP);
            Interaction.VocalManager.AddGrammar("funny_names", Buddy.LoadContext.APP);
            Interaction.VocalManager.OnVoconBest = VoconBest;
            Interaction.VocalManager.OnVoconEvent = EventVocon;
            mHasSaidName = false;
            mGoodName = false;
            mActualName = "";
            mQuizzBehaviour.LastStateId = 1;
            mQuizzBehaviour.InitPlayers();
            StartCoroutine(Engagement());
            mQuizzBehaviour.OnLanguageChange = OnLanguageChange;
            foreach (Player player in mQuizzBehaviour.Players)
            {
                Debug.Log("player: " + player.Name);
            }
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mQuizzBehaviour.OnLanguageChange = null;
        }

        private IEnumerator Engagement()
        {
            
            Interaction.TextToSpeech.Say(Dictionary.GetString("super").Replace("[numplayer]", "" + mQuizzBehaviour.NumPlayer));
            while (!Interaction.TextToSpeech.HasFinishedTalking)
                yield return null;
            for (int i = 0; i < mQuizzBehaviour.NumPlayer; i++)
            {
                mActualName = mQuizzBehaviour.Players[i].Name;
                Interaction.TextToSpeech.Say(Dictionary.GetString("sayname").Replace("[num]", "" + (i + 1)) + mQuizzBehaviour.Players[i].Name);
                while (!Interaction.TextToSpeech.HasFinishedTalking)
                    yield return null;
                mHasSaidName = false;
                mGoodName = false;
                Interaction.VocalManager.StartInstantReco();
                while (!mHasSaidName)
                    yield return null;
                if (mGoodName)
                {
                    Interaction.BMLManager.LaunchByName("Happy02");
                    while (!Interaction.BMLManager.DonePlaying)
                        yield return null;
                    Interaction.TextToSpeech.Say(Dictionary.GetRandomString("namerecognized").Replace("[name]", mQuizzBehaviour.Players[i].Name));
                }
                else
                {
                    Interaction.TextToSpeech.Say(Dictionary.GetRandomString("namenotrecognized").Replace("[name]", mQuizzBehaviour.Players[i].Name));
                    Interaction.BMLManager.LaunchByName("Grumpy01");
                    while (!Interaction.BMLManager.DonePlaying)
                        yield return null;
                }
                while (!Interaction.TextToSpeech.HasFinishedTalking)
                    yield return null;
            }
            mQuizzBehaviour.Beginning = true;
            mQuizzBehaviour.ListQuestionsIdAsked = new HashSet<int>();
            Trigger("CheckNumQuestion");
        }



        private void EventVocon(VoconEvent iEvent)
        {
            Debug.Log(iEvent);
        }

        private void VoconBest(VoconResult iBestResult)
        {
            Debug.Log("le best result: " + iBestResult.Utterance + " confidence: " + iBestResult.Confidence + " best rule: " + iBestResult.StartRule);
            if (iBestResult.Utterance == null || iBestResult.Utterance == "" || iBestResult.Confidence == 0)
            {
                //Interaction.VocalManager.StopRecognition();
                mHasSaidName = true;
                //Interaction.VocalManager.StartInstantReco();
            }
            else if (iBestResult.StartRule == "commands_" + mQuizzBehaviour.Lang + "#quit" && iBestResult.Confidence > 6000)
            {
                Trigger("Exit");
            }
            else
            {
                if (iBestResult.Utterance.Contains(mActualName))
                    mGoodName = true;
                mHasSaidName = true;
            }
        }
    }
}