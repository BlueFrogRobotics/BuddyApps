using UnityEngine;

using Buddy;
using Buddy.UI;

namespace BuddyApp.PlayMath{
    public class ResultState : AStateMachineBehaviour {

        private const float DURATION_NOT = 2.0f;

		private Animator mBackgroundAnimator;

        private bool mEndOnce;
        private string mTTSKey;

        private Result mResult;
        private Score mScore;

        private Sprite mIcon;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			mBackgroundAnimator = GameObject.Find("UI/Background_Black").GetComponent<Animator>();
			mBackgroundAnimator.SetTrigger("close");

            mResult = GameObject.Find("UI/Four_Answer").GetComponent<Result>();
            float lDuration = DURATION_NOT;
            MoodType lBuddyMood;
            if (mResult.isCorrect()) {
                lBuddyMood = MoodType.HAPPY;
                mTTSKey = "goodanswerspeech";
                mIcon = BYOS.Instance.Resources.GetSpriteFromAtlas("Icon_Check");
            }
            else {
                lBuddyMood = MoodType.SAD;
                mTTSKey = "badanswerspeech";
                mIcon = BYOS.Instance.Resources.GetSpriteFromAtlas("Icon_Close");
                lDuration += DURATION_NOT;
            }

            mScore = GameObject.Find("UI/EndGame_Score").GetComponent<Score>();
            mScore.AddResult(mResult);

            string lTTSMessage = BYOS.Instance.Dictionary.GetRandomString(mTTSKey) ;
            string lEquation = mResult.Equation + " = " + mResult.CorrectAnswer;
            BYOS.Instance.Notifier.Display<SimpleNot>(lDuration).With(lTTSMessage + lEquation, mIcon);

            BYOS.Instance.Interaction.Mood.Set(lBuddyMood);

            mEndOnce = false;
            BYOS.Instance.Interaction.TextToSpeech.Say(lTTSMessage,true);
            if( !mResult.isCorrect() )
                AnnounceResult(lEquation);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (BYOS.Instance.Interaction.TextToSpeech.HasFinishedTalking && !mEndOnce)
            {
                Debug.Log("Has finished talking");
                if (mResult.Last)
                {
					User.Instance.Scores.NewScore(mScore);
                    User.SaveUser();

                    //TODO Uncomment following if statement to generate certificate only once
                    if (mScore.IsPerfect() /*&& !User.Instance.HasCurrentCertificate()*/) {
                        BYOS.Instance.Interaction.TextToSpeech.SayKey("takephotospeech", true);
                        animator.SetTrigger("TakePhoto");
                    }
                    else
                        animator.SetTrigger("Score");
                }
                else
                    animator.SetTrigger("NextQuestion");
                mEndOnce = true;
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			mBackgroundAnimator.SetTrigger("open");
            BYOS.Instance.Interaction.Mood.Set(MoodType.NEUTRAL);
        }

        private void AnnounceResult(string statement)
        {
            if (statement.Contains("÷"))
                statement = statement.Replace("÷", BYOS.Instance.Dictionary.GetString("dividedby"));
            if (statement.Contains("×"))
                statement = statement.Replace("×", BYOS.Instance.Dictionary.GetString("xtimesy"));
            if (statement.Contains("-"))
                statement = statement.Replace("-", BYOS.Instance.Dictionary.GetString("minus"));
            if(statement.Contains("="))
                statement = statement.Replace("=",BYOS.Instance.Dictionary.GetString("equal"));

            BYOS.Instance.Interaction.TextToSpeech.Say(statement,true);
        }
    }
}
