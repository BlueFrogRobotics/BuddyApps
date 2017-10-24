using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;

namespace BuddyApp.RedLightGreenLightGame
{
    public class RLGLTargetGameplay : AStateMachineBehaviour
    {
        private RedLightGreenLightGameBehaviour mRLGLBehaviour;
        private int mIdLevel;
        private float mLimit;
        private bool mSentenceDone;

        public override void Start()
        {
            mRLGLBehaviour = GetComponentInGameObject<RedLightGreenLightGameBehaviour>(0);
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("ON STATE ENTER TARGET GAMEPLAY");
            Interaction.Face.SetExpression(MoodType.TIRED);
            //mlimit a determiner en fonction du xml
            mLimit = 5F;
            mRLGLBehaviour.Timer = 0F;
            mSentenceDone = false;
            //quand on aura le parsing xml, prendre le idlevel directement de celui ci
            //mIdLevel = mRLGLBehaviour.IdLevel;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("ON UPDATE TARGET GAMEPLAY");

            if (mRLGLBehaviour.Timer < mLimit)
            {
                ClickTarget();
            }
            else if(!Interaction.TextToSpeech.HasFinishedTalking && !mSentenceDone)
            {
                Interaction.TextToSpeech.SayKey("top");
                mSentenceDone = true;
            }
            if(mSentenceDone && mRLGLBehaviour.Timer > mLimit)
            {
                mRLGLBehaviour.Gameplay = true;
                Trigger("StartGame");
            }
                


        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("ON EXIT TARGET GAMEPLAY");
            GetGameObject(1).SetActive(false);
            mRLGLBehaviour.TargetClicked = false;
        }

        private void ClickTarget()
        {
            mRLGLBehaviour.FirstTurn = false;
            if (mIdLevel >= 0 && mIdLevel < 3)
            {
                Primitive.Motors.Wheels.TurnAngle(-180F, 250F, 1F);
            }
            else if (mIdLevel > 2)
            {
                //il faudra open les yeux quand on pourra les fermer
                Debug.Log("open the eyes!");
            }
            //faire apparaitre la cible suivant taille du xml / vitesse, pour le moment la cible apparait au milieu avec taille définie
            if (!GetGameObject(1).activeSelf)
                GetGameObject(1).SetActive(true);
            if (mRLGLBehaviour.TargetClicked && (Primitive.Motors.Wheels.Status == MovingState.REACHED_GOAL || Primitive.Motors.Wheels.Status == MovingState.MOTIONLESS))
            {
                GetGameObject(1).SetActive(false);
                Trigger("Victory");

            }
        }
    }
}

