using UnityEngine;
using BlueQuark;
using System.Collections;

namespace BuddyApp.MemoryGame
{
    public class QuitMemoryState : AStateMachineBehaviour
    {        
        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.Vocal.Say(Buddy.Resources.GetRandomString("bye"), (iOutput) =>
            {
                QuitApp();
            });
        }
    }
}

