using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

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
            Debug.Log("Quizz: engagement phase state");
            mQuizzBehaviour.LastStateId = 1;

            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.StopAndClear();
            string[] grammars = { "commands", "funny_names" };
            Buddy.Vocal.DefaultInputParameters = new SpeechInputParameters
            {
                Grammars = grammars
            };
            Buddy.Vocal.OnEndListening.Add(OnEndListening);

            mHasSaidName = false;
            mGoodName = false;
            mActualName = "";
            mQuizzBehaviour.InitPlayers();

            StartCoroutine(Engagement());

        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.Vocal.OnEndListening.Clear();
        }

        /// <summary>
        /// Asks each player to repeat their nickname.
        /// Reacts depending on if they repeated it well
        /// </summary>
        /// <returns></returns>
        private IEnumerator Engagement()
        {            
            Buddy.Vocal.Say(Buddy.Resources.GetString("super").Replace("[n]", "" + QuizzData.Instance.NbQuestions).Replace("[numplayer]", "" + mQuizzBehaviour.NumPlayer));
            yield return new WaitUntil(() => !Buddy.Vocal.IsBusy);

            for (int i = 0; i < mQuizzBehaviour.NumPlayer; i++)
            {
                mActualName = mQuizzBehaviour.Players[i].Name;
                Buddy.Vocal.Say(Buddy.Resources.GetString("sayname").Replace("[num]", "" + (i + 1)) + mQuizzBehaviour.Players[i].Name);
                yield return new WaitUntil(() => !Buddy.Vocal.IsBusy);

                mHasSaidName = false;
                mGoodName = false;
                Buddy.Vocal.Listen();

                while (!mHasSaidName)
                    yield return null;

                if (mGoodName)
                {
                    //Buddy.Behaviour.SetMood(Mood.HAPPY);
                    // while (Buddy.Behaviour.Interpreter.IsBusy)
                        // yield return null;
                    Buddy.Vocal.Say(Buddy.Resources.GetRandomString("namerecognized").Replace("[name]", mQuizzBehaviour.Players[i].Name));
                }
                else
                {
                    //Buddy.Behaviour.SetMood(Mood.GRUMPY);
                    Buddy.Vocal.Say(Buddy.Resources.GetRandomString("namenotrecognized").Replace("[name]", mQuizzBehaviour.Players[i].Name));                    
                    // while (Buddy.Behaviour.Interpreter.IsBusy)
                        // yield return null;
                }
                yield return new WaitUntil(() => !Buddy.Vocal.IsBusy);
            }
            mQuizzBehaviour.Beginning = true;
            //mQuizzBehaviour.ListQuestionsIdAsked = new HashSet<int>();
            Trigger("CheckNumQuestion");
        }



        //private void EventVocon(VoconEvent iEvent)
        //{
        //    Debug.Log(iEvent);
        //}

        private void OnEndListening (SpeechInput iSpeechInput)
        {
            if (string.IsNullOrEmpty(iSpeechInput.Utterance))
            {
                //Buddy.Vocal.StopRecognition();
                mHasSaidName = true;
                //Buddy.Vocal.Listen();
            }
            else if (iSpeechInput.Rule == "commands_" + mQuizzBehaviour.Lang + "#quit")
            {
                Trigger("Exit");
            }
            else
            {
                if (iSpeechInput.Utterance.Contains(mActualName))
                    mGoodName = true;
                mHasSaidName = true;
            }
        }
    }
}