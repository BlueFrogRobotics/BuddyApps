using UnityEngine;
using System.Collections;
using BuddyOS;

namespace BuddyApp.IOT
{
    public class IOTListenState : AIOTStateMachineBehaviours
    {
        private int listenedTimes = 0;
        private string mMsg = "";
        private bool mRequested = false;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
            mRequested = false;
            mError = STTError.ERROR_INSUFFICIENT_PERMISSIONS;
            mMsg = "";
            CommonStrings["STT"] = "";

            mSTT.OnBestRecognition.Add(messageReceived);
            mSTT.OnErrorEnum.Add(errorReceived);
            mMood.Set(MoodType.LISTENING);
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
            mSTT.OnBestRecognition.Remove(messageReceived);
            mSTT.OnErrorEnum.Remove(errorReceived);
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
                    
                    if (CommonStrings.Count < 2)
                        CommonStrings.Add("PARAM", "");

                    mMsg = "";
                    listenedTimes = 0;
                }
                else
                {

                    listenedTimes++;
                    if (listenedTimes > 3)
                    {
                        iAnimator.SetTrigger(mHashList[(int)HashTrigger.NEXT]);
                        listenedTimes = 0;
                        return;
                    }
                }

                switch (mError)
                {
                    case STTError.ERROR_INSUFFICIENT_PERMISSIONS:
                        break;
                    case STTError.ERROR_NETWORK:
                        iAnimator.SetTrigger(mHashList[(int)HashTrigger.NETWORK_ERROR]);
                        break;
                    case STTError.ERROR_NO_MATCH:
                    case STTError.ERROR_SPEECH_TIMEOUT:
                        iAnimator.SetTrigger(mHashList[(int)HashTrigger.MATCH_ERROR]);
                        break;
                    case STTError.ERROR_NETWORK_TIMEOUT:
                        iAnimator.SetTrigger(mHashList[(int)HashTrigger.TIMEOUT_ERROR]);
                        break;
                    default:
                        iAnimator.SetTrigger(mHashList[(int)HashTrigger.BACK]);
                        break;
                }

                if (!mRequested)
                {
                    mSTT.Request();
                    mRequested = true;
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
