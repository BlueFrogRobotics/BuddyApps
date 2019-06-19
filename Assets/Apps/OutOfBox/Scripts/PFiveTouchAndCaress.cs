using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.OutOfBox
{
    public class PFiveTouchAndCaress : AStateMachineBehaviour
    {
        private float mTimer;
        private bool mActiveTimer;
        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            mTimer = 0F;
            mActiveTimer = false;
            Buddy.Vocal.Say("pfivelovecaress", (iOut) => 
            {
                Buddy.Behaviour.SetMood(Mood.ANGRY, false);
                StartCoroutine(OutOfBoxUtils.WaitTimeAsync(1F, () => {
                    Buddy.Vocal.Say("pfivedontlovecaress", (iOutTwo) =>
                    {
                        Buddy.Behaviour.ResetMood();
                        Buddy.Vocal.Say("pfivewhowantstotry", (iOutSpeech) => { Buddy.Behaviour.Face.OnTouch.Add(OnFaceTouched); mActiveTimer = true; });
                    });
                }));
                
            });
            
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (mActiveTimer && mTimer < 15F)
            {
                mTimer = Time.deltaTime;
                if (!Buddy.Behaviour.Interpreter.IsBusy)
                {
                    if (Buddy.Sensors.TouchSensors.BackHead.Value)
                    {
                        mTimer = 0F;
                        OutOfBoxUtils.DebugColor("BACKHEAD", "blue");
                        Buddy.Behaviour.Interpreter.Run("CenterHead01");
                    }
                    else if (Buddy.Sensors.TouchSensors.Heart.Value)
                    {
                        mTimer = 0F;
                        OutOfBoxUtils.DebugColor("Heart", "blue");
                        Buddy.Behaviour.Interpreter.Run("CenterHeart01");
                    }
                    else if (Buddy.Sensors.TouchSensors.LeftHead.Value)
                    {
                        mTimer = 0F;
                        OutOfBoxUtils.DebugColor("LeftHead", "blue");
                        Buddy.Behaviour.Interpreter.Run("LeftHead01");
                    }
                    else if (Buddy.Sensors.TouchSensors.LeftShoulder.Value)
                    {
                        mTimer = 0F;
                        OutOfBoxUtils.DebugColor("LeftShoulder", "blue");
                        Buddy.Behaviour.Interpreter.Run("LeftShoulder01");
                    }
                    else if (Buddy.Sensors.TouchSensors.RightHead.Value)
                    {
                        mTimer = 0F;
                        OutOfBoxUtils.DebugColor("RightHead", "blue");
                        Buddy.Behaviour.Interpreter.Run("RightHead01");
                    }
                    else if (Buddy.Sensors.TouchSensors.RightShoulder.Value)
                    {
                        mTimer = 0F;
                        OutOfBoxUtils.DebugColor("RightShoulder", "blue");
                        Buddy.Behaviour.Interpreter.Run("RightShoulder01");
                    }
                }
                else if (mTimer > 15F)
                {
                    //quit app apres BI
                    OutOfBoxUtils.PlayBIAsync(() => { Buddy.Vocal.Say("pfivetooshy", (OutSpeech) => 
                    {
                        OutOfBoxData.Instance.Phase = OutOfBoxData.PhaseId.PhaseSix;
                        Trigger("Base");
                    });
                    });
                }
            }
        }
        
        private void OnFaceTouched(FacialPart iFacial)
        {
            OutOfBoxUtils.DebugColor("Face touched : " , "blue");
            mTimer = 0F;
            if(iFacial == FacialPart.LEFT_EYE || iFacial == FacialPart.RIGHT_EYE)
            {
                Buddy.Behaviour.Interpreter.StopAndClear();
                Buddy.Behaviour.Interpreter.Run("BML/Angry01");
            }
            else if(iFacial == FacialPart.MOUTH)
            {
                if(!Buddy.Vocal.IsBusy)
                    Buddy.Vocal.Listen(SpeechRecognitionMode.GRAMMAR_ONLY);
            }
            else
                Buddy.Behaviour.Interpreter.Run("BML/Happy01");
        }
    }
}

