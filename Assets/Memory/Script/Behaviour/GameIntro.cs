using UnityEngine;
using BuddyOS;
using System;

namespace BuddyApp.Memory
{
    public class GameIntro : LinkStateMachineBehavior
    {
        private float mTTSTimer;

        public override void Init()
        {
            BYOS.Instance.VocalManager.enabled = false;
            mOnEnterDone = false;
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        protected override void OnEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            mTTSTimer = 0.0f;

            BYOS.Instance.Speaker.Voice.Play(VoiceSound.RANDOM_LAUGH);
            mTTS.Silence(1000, true);
            mTTS.Say(link.gameLevels.intro, true);
            mOnEnterDone = true;
        }


        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        protected override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (link.mUnloadingScene) {
                Debug.Log("Unloading");
                QuitApp();
            }

            if (mOnEnterDone) {
                mTTSTimer += Time.deltaTime;

                if (mTTS.HasFinishedTalking && mTTSTimer > 3.0f) {
                    animator.SetTrigger("IntroDone");
                }
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }
    }
}
