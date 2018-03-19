using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using Buddy.UI;
using System;

namespace BuddyApp.SandboxApp
{
    public class QAManager : AStateMachineBehaviour
    {
        [Header("Binary Question Parameters: ")]
        [SerializeField]
        private bool IsBinaryQuestion;
        [SerializeField]
        private bool IsBinaryToaster;
        [SerializeField]
        private string LeftButton;
        [SerializeField]
        private string RightButton;
        [SerializeField]
        private string TriggerLeftButton;
        [SerializeField]
        private string TriggerRightButton;
        [SerializeField]
        private string KeyQuestion;
        [SerializeField]
        private bool IsSoundForButton;
        [SerializeField]
        private FXSound FxSound;
        [Header("Multiple Question Parameters : ")]
        [SerializeField]
        private bool IsMultipleQuestion;
        [SerializeField]
        private bool IsMultipleToaster;

        private bool mIsDisplayed;
        ButtonInfo mButtonLeft;
        ButtonInfo mButtonRight;

        public override void Start()
        {

        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if(IsBinaryQuestion && !mIsDisplayed)
            {
                mIsDisplayed = true;
                if (IsBinaryToaster)
                {
                    if (string.IsNullOrEmpty(RightButton))
                        RightButton = Dictionary.GetString("yes");
                    if (string.IsNullOrEmpty(LeftButton))
                        RightButton = Dictionary.GetString("no");
                    if (string.IsNullOrEmpty(KeyQuestion))
                        KeyQuestion = " ";
                    mButtonRight = new ButtonInfo
                    {
                        Label = RightButton,
                        OnClick = PressedRightButton
                    };
                    mButtonLeft = new ButtonInfo
                    {
                        Label = LeftButton,
                        OnClick = PressedLeftButton
                    };
                    //BYOS.Instance.Toaster.Display<BinaryQuestionToast>().With(Dictionary.GetString(KeyQuestion), mButtonLeft, mButtonRight);
                    Debug.Log("bug qamanager");
                }
                    
            }
            else if (IsMultipleQuestion && !mIsDisplayed)
            {
                mIsDisplayed = true;
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        } 

        private void PressedRightButton()
        {
            Toaster.Hide();
            if(IsSoundForButton)
                Primitive.Speaker.FX.Play(FxSound);
            Trigger(TriggerRightButton);
        }

        private void PressedLeftButton()
        {
            Toaster.Hide();
            if (IsSoundForButton)
                Primitive.Speaker.FX.Play(FxSound);
            Trigger(TriggerLeftButton);
            
        }


    }

}
