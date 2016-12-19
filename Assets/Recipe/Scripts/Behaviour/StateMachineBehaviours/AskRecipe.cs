using UnityEngine;
using BuddyOS.App;
using BuddyOS;

namespace BuddyApp.Recipe
{
    public class AskRecipe : AStateMachineBehaviour
    {

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GetGameObject(2).SetActive(true);
            mNotManager.Display<SimpleNot>().With("Alors qu'est ce que l'on prépare ? ", mSpriteManager.GetSprite("Message"));
            mTTS.Say("Alors qu'est ce que l'on prépare ?");
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mTTS.HasFinishedTalking())
                iAnimator.SetTrigger("QuestionFinished");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }
    }
}