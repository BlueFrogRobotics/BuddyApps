using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BlueQuark;

using System;

namespace BuddyApp.Calcul
{

    public class ResultState : AStateMachineBehaviour
    {
        private Result mResult;
        private Score mScore;
        private bool mNextStepActivated;

        public override void Start()
        {
            Buddy.GUI.Header.OnClickParameters.Add(() =>
            {
                Trigger("Parameters");
            });
            mScore = GetComponent<CalculBehaviour>().Score;
        }


        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mNextStepActivated = false;
            mResult = mScore.LastResult();
            if (mResult == null)
            {
                // TODO manage null result
                Debug.Log("Result null");
                return;
            }
            Mood lBuddyMood;
            string key;
            if (mResult.isCorrect())
            {
                lBuddyMood = Mood.HAPPY;
                key = "goodanswerspeech";
            }
            else
            {
                lBuddyMood = Mood.SAD;
                key = "badanswerspeech";
            }

            string lTTSMessage = Buddy.Resources.GetRandomString(key);
            string lEquation = mResult.Equation + " = " + mResult.CorrectAnswer;
            if (!mResult.isCorrect())
                lTTSMessage += CalculBehaviour.GetVocalEquation(lEquation);
            
            //Buddy.GUI.Header.DisplayLightTitle(lEquation);
            Buddy.Behaviour.SetMood(lBuddyMood);

            Buddy.Vocal.StopAndClear();
            Buddy.Vocal.Say(lTTSMessage, (iOutput) =>
            {
                NextStep();
            });
        }

        private void NextStep()
        {
            if (mNextStepActivated)
                return;
            if (mResult.Last)
            {
                User.Instance.Scores.NewScore(mScore);
                //User.SaveUser();

                mNextStepActivated = true;
                Trigger("Score");
            }
            else
            {
                mNextStepActivated = true;
                Trigger("NextQuestion");
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
        }
    }
}
