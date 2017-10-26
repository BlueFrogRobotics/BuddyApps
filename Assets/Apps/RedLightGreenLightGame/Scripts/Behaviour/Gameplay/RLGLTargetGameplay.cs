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
        private bool mIsMovementDone;

        public override void Start()
        {
            mRLGLBehaviour = GetComponentInGameObject<RedLightGreenLightGameBehaviour>(0);
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("ON STATE ENTER TARGET GAMEPLAY");
            Interaction.Mood.Set(MoodType.TIRED);
            //mlimit a determiner en fonction du xml
            mLimit = 8F;
            mRLGLBehaviour.Timer = 0F;
            mSentenceDone = false;
            mIsMovementDone = false;
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
            else if(!mSentenceDone)
            {
                Interaction.TextToSpeech.SayKey("top");
                mSentenceDone = true;
            }
            if(mSentenceDone && mRLGLBehaviour.Timer > mLimit && Interaction.TextToSpeech.HasFinishedTalking)
            {
                mRLGLBehaviour.Gameplay = true;
                if (mIdLevel >= 0 && mIdLevel < 3 && !mIsMovementDone)
                {
                    //tourner de - la valeur du xml
                    Primitive.Motors.Wheels.TurnAngle(-180F, 250F, 1F);
                    mIsMovementDone = true;
                }
                else if (mIdLevel > 2)
                {
                    //il faudra open les yeux quand on pourra les fermer
                    Debug.Log("open the eyes!");
                }
                if((Primitive.Motors.Wheels.Status == MovingState.REACHED_GOAL || Primitive.Motors.Wheels.Status == MovingState.MOTIONLESS))
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
            
            Debug.Log("CLICJ TARGET 1------------------------------------------------");
            //faire apparaitre la cible suivant taille du xml / vitesse, pour le moment la cible apparait au milieu avec taille définie
            if (!GetGameObject(1).activeSelf)
                GetGameObject(1).SetActive(true);
            if (mRLGLBehaviour.TargetClicked && (Primitive.Motors.Wheels.Status == MovingState.REACHED_GOAL || Primitive.Motors.Wheels.Status == MovingState.MOTIONLESS))
            {
                Debug.Log("CLICJ TARGET 2 ---------------------------------------------");
                mRLGLBehaviour.FirstTurn = false;
                GetGameObject(1).SetActive(false);
                Trigger("Victory");

            }
        }
    }
}

