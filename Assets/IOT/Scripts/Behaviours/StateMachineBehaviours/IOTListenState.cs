﻿using UnityEngine;
using System.Collections;
using BuddyOS;

namespace BuddyApp.IOT
{
    public class IOTListenState : AIOTStateMachineBehaviours
    {
        private int listenedTimes = 0;
        private string mMsg = "";

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
            mSTT.OnBestRecognition.Add(messageReceived);
            mSTT.Request();

            listenedTimes++;
            if (listenedTimes > 3)
            {
                iAnimator.SetTrigger(HashList[(int)HashTrigger.NEXT]);
                listenedTimes = 0;
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
            mSTT.OnBestRecognition.Remove(messageReceived);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
            if (mSTT.HasFinished)
            {
                if (mMsg != "")
                {
                    iAnimator.SetTrigger(mHashList[(int)HashTrigger.LISTENED]);
                    mNotManager.Display<SimpleNot>().With(mMsg, mSpriteManager.GetSprite("Message"));
                    if (CommonStrings.Count > 0)
                        CommonStrings["STT"] = mMsg;
                    else
                        CommonStrings.Add("STT", mMsg);
                    mMsg = "";
                    listenedTimes = 0;
                }
                else
                {
                    switch (mError)
                    {
                        case STTError.ERROR_NETWORK:
                            iAnimator.SetTrigger((int)HashTrigger.NETWORK_ERROR);
                            break;
                        case STTError.ERROR_NO_MATCH:
                            iAnimator.SetTrigger((int)HashTrigger.MATCH_ERROR);
                            break;
                        case STTError.ERROR_NETWORK_TIMEOUT:
                            iAnimator.SetTrigger((int)HashTrigger.TIMEOUT_ERROR);
                            break;
                    }
                }
            }
        }

        private void messageReceived(string iMsg)
        {
            mMsg = iMsg;
        }

        private void errorReceived(STTError iError)
        {
            mError = iError;
        }
    }
}
