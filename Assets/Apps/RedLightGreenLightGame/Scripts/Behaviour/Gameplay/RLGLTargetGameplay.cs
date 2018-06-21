using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;

namespace BuddyApp.RedLightGreenLightGame
{
    public class RLGLTargetGameplay : AStateMachineBehaviour
    {
        private RedLightGreenLightGameBehaviour mRLGLBehaviour;
        private LevelManager mLevelManager;
        private int mIdLevel;
        private float mLimit;
        private bool mSentenceDone;
        private bool mIsMovementDone;
        private bool mAnimIsOpen;

        public override void Start()
        {
            mRLGLBehaviour = GetComponentInGameObject<RedLightGreenLightGameBehaviour>(0);
            mLevelManager = GetComponentInGameObject<LevelManager>(0);
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(!GetGameObject(2).GetComponent<RLGLTargetMovement>().enabled)
                GetGameObject(2).GetComponent<RLGLTargetMovement>().enabled = true;
            GetGameObject(2).GetComponent<RLGLTargetMovement>().ResetTargetMovement();
            
            if (!mRLGLBehaviour.IsPlaying)
            {
                mRLGLBehaviour.TargetClicked = false;
                mAnimIsOpen = false;
            }
                
            Interaction.Face.SetEvent(FaceEvent.CLOSE_EYES);
            Interaction.Mood.Set(MoodType.TIRED);
            //mlimit a determiner en fonction du xml
            mLimit = mLevelManager.LevelData.WaitingTime;//8F;
            mRLGLBehaviour.Timer = 0F;
            mSentenceDone = false;
            mIsMovementDone = false;
            //quand on aura le parsing xml, prendre le idlevel directement de celui ci
            mIdLevel = mLevelManager.LevelData.Level;
            mRLGLBehaviour.TargetClicked = false;
            GetGameObject(1).GetComponent<Animator>().ResetTrigger("close");
            GetGameObject(1).GetComponent<Animator>().ResetTrigger("open");
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (mRLGLBehaviour.Timer < mLimit)
            {
               // Debug.Log("CLICK TARGET");
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
                {
                    //GetGameObject(1).GetComponent<Animator>().ResetTrigger("close");
                    GetGameObject(1).GetComponent<Animator>().SetTrigger("close");
                    Trigger("StartGame");
                    
                }
                    
            }
                
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //GetGameObject(1).GetComponent<Animator>().ResetTrigger("close");
            //GetGameObject(1).GetComponent<Animator>().ResetTrigger("open");
            //GetGameObject(1).GetComponent<Animator>().SetTrigger("close");
            //GetGameObject(1).SetActive(false);
            mRLGLBehaviour.TargetClicked = false;
            Interaction.Face.SetEvent(FaceEvent.OPEN_EYES);
            Interaction.Mood.Set(MoodType.NEUTRAL);

        }

        private void ClickTarget()
        {
            //faire apparaitre la cible suivant taille du xml / vitesse, pour le moment la cible apparait au milieu avec taille définie
            //if (!GetGameObject(1).activeSelf)
            //{
            //    Debug.Log("activation");


            //    GetGameObject(1).SetActive(true);
            //}
            if (!mAnimIsOpen)
            {
                GetGameObject(1).GetComponent<Animator>().SetTrigger("open");
                GetGameObject(2).transform.localScale = new Vector3(mLevelManager.LevelData.Target.Size, mLevelManager.LevelData.Target.Size, 1);
                mAnimIsOpen = true;
            }

            if (mRLGLBehaviour.TargetClicked && (Primitive.Motors.Wheels.Status == MovingState.REACHED_GOAL || Primitive.Motors.Wheels.Status == MovingState.MOTIONLESS))
            {
                Debug.Log("/////////////////TARGET PROBLEME BRO-----------------");
                mRLGLBehaviour.FirstTurn = false;
                GetGameObject(1).GetComponent<Animator>().SetTrigger("close");
                //GetGameObject(1).SetActive(false);
                Interaction.Mood.Set(MoodType.HAPPY);
                if(mLevelManager.LevelData.Level < 3)
                    Primitive.Motors.Wheels.TurnAngle(-180F, 250F, 1F);
                mRLGLBehaviour.IsGameplayDone = false;
                Trigger("Victory");
                
            }
        }
    }
}

