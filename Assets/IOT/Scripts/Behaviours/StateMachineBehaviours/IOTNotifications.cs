using UnityEngine;
using System.Collections;
using System;
using BuddyOS;

namespace BuddyApp.IOT
{


    public class IOTNotifications : AIOTStateMachineBehaviours
    {
        public enum Notification { SIMPLE, CONFIRM, VALIDATION, MESSAGE, TIMER}
        [SerializeField]
        private Notification notification;
        [SerializeField]
        private string message;
        [SerializeField]
        private Action firstAction;
        [SerializeField]
        private Action secondAction;
        [SerializeField]
        private string sprite;
        [SerializeField]
        private int timer;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
            switch (notification)
            {
                case Notification.SIMPLE:
                    mNotManager.Display<SimpleNot>().With(message, mSpriteManager.GetSprite(sprite));
                    break;
                case Notification.CONFIRM:
                    mNotManager.Display<ConfirmationNot>().With(message, firstAction, secondAction, mSpriteManager.GetSprite(sprite));
                    break;
                case Notification.VALIDATION:
                    mNotManager.Display<ValidationNot>().With(message, firstAction, secondAction);
                    break;
                case Notification.MESSAGE:
                    mNotManager.Display<MessageNot>().With(message, firstAction, secondAction);
                    break;
                case Notification.TIMER:
                    mNotManager.Display<TimerNot>().With(message, timer, firstAction, secondAction);
                    break;

            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
        }
    }
}
