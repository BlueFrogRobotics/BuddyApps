using UnityEngine;
using Buddy;

namespace BuddyApp.Recipe
{
    public class NoAnswer : AStateMachineBehaviour
    {
        bool mCheck;

        public override void Start()
        {
        }

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("ENTER NO ANSWER");
            mCheck = false;
			/*if (GetComponent<RecipeBehaviour>().NoAnswerCount == 0)
            {
                Debug.Log("First no answer");
                mFace.SetExpression(MoodType.SAD);
                mCheck = true;
                mTTS.Say(mDictionary.GetString("noanswerrecipe"));
            }
            else
            {*/
			Interaction.Mood.Set(MoodType.TIRED, false, true);
            //}
        }

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!mCheck && Primitive.Speaker.Voice.Status == SoundChannelStatus.FINISH)
            {
                Debug.Log("NO ANSWER 2");
                Interaction.TextToSpeech.Say(Dictionary.GetRandomString("noanswerrecipe2"));
                mCheck = true;
            }
            else if (Interaction.TextToSpeech.HasFinishedTalking && mCheck)
            {
                Debug.Log("Finished Talking");
                iAnimator.SetTrigger("AskRecipeAgain");
            }
        }

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("EXIT NO ANSWER");
            GetComponent<RecipeBehaviour>().NoAnswerCount++;
        }
    }
}