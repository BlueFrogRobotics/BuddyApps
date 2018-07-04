using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;

namespace BuddyApp.Quizz
{
    public class EngagementPhaseState : AStateMachineBehaviour
    {

        private QuizzBehaviour mQuizzBehaviour;
        private bool mHasSaidName = false;
        private bool mGoodName = false;
        private string mActualName = "";

        public override void Start()
        {
            mQuizzBehaviour = GetComponent<QuizzBehaviour>();
            //HashSet<int> hashset = new HashSet<int>();
            //hashset.Add(3);
            //hashset.Add(4);
            //hashset.Add(5);
            //hashset.Add(3);
            //hashset.Add(2);
            //hashset.Add(2);
            //foreach(int a in hashset)
            //{
            //    Debug.Log("dans hash: " + a);
            //}
            //mQuizzBehaviour.NumPlayer = 3;

        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("debut engagement");
            Interaction.VocalManager.UseVocon = true;
            Interaction.VocalManager.ClearGrammars();
            Interaction.VocalManager.AddGrammar("funny_names", Buddy.LoadContext.APP);
            Interaction.VocalManager.OnVoconBest = VoconBest;
            Interaction.VocalManager.OnVoconEvent = EventVocon;
            mHasSaidName = false;
            mGoodName = false;
            mActualName = "";
            mQuizzBehaviour.InitPlayers();
            StartCoroutine(Engagement());
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
                    Interaction.TextToSpeech.Say(Dictionary.GetRandomString("namerecognized").Replace("[name]", mQuizzBehaviour.Players[i].Name));
                else
                    Interaction.TextToSpeech.Say(Dictionary.GetRandomString("namenotrecognized").Replace("[name]", mQuizzBehaviour.Players[i].Name));
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
            Debug.Log("le best result: " + iBestResult.Utterance + " confidence: " + iBestResult.Confidence+ " best rule: "+ iBestResult.StartRule);
            if (iBestResult.Utterance == null || iBestResult.Utterance == "" || iBestResult.Confidence == 0)
            {
                //Interaction.VocalManager.StopRecognition();
                Interaction.VocalManager.StartInstantReco();
            }
            else if (iBestResult.StartRule == "funny_names_fr#quit")
            {
                Trigger("Quit");
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