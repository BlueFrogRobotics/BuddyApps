using UnityEngine;
using BuddyOS.App;

namespace BuddyApp.Recipe
{
    public class NoRecipeFound : AStateMachineBehaviour
    {

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTTS.Say("Désolé je n'ai pas trouvé aucune recette correspondant à ces critères");
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mTTS.HasFinishedTalking)
                iAnimator.SetTrigger("AskCategoryAgain");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }
    }
}