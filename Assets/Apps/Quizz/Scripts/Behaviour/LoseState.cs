using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;

namespace BuddyApp.Quizz
{
    public class LoseState : AStateMachineBehaviour
    {

        private QuizzBehaviour mQuizzBehaviour;
        private SoundsManager mSoundsManager;

        public override void Start()
        {
            mQuizzBehaviour = GetComponent<QuizzBehaviour>();
            mSoundsManager = GetComponent<SoundsManager>();
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("lose state");
            StartCoroutine(Lose());
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        private IEnumerator Lose()
        {
            Interaction.VocalManager.StopListenBehaviour();
            //Interaction.Mood.Set(MoodType.SAD);
            Interaction.BMLManager.LaunchByName("Sad01");
            mSoundsManager.PlaySound(SoundsManager.Sound.BAD_ANSWER);
            while (!Interaction.BMLManager.DonePlaying)
                yield return null;
            while (mSoundsManager.IsPlaying)
                yield return null;
            Interaction.TextToSpeech.Say(Dictionary.GetRandomString("lose").Replace("[answer]", "" + mQuizzBehaviour.ActualQuestion.Answers[mQuizzBehaviour.ActualQuestion.GoodAnswer]));
            while (!Interaction.TextToSpeech.HasFinishedTalking)
                yield return null;
            Interaction.Mood.Set(MoodType.NEUTRAL);
            if (mQuizzBehaviour.ActualQuestion.AnswerComplement != "")
                Interaction.TextToSpeech.Say(mQuizzBehaviour.ActualQuestion.AnswerComplement);
            while (!Interaction.TextToSpeech.HasFinishedTalking)
                yield return null;
            Trigger("CheckNumQuestion");
        }
    }
}