using Buddy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.Companion
{
    public class Connection : AStateMachineBehaviour
    {
        private bool mIsPing;
        private bool mEnd;

        // Use this for initialization
        public override void Start()
        {
            mIsPing = false;
            mEnd = false;

        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iAnimatorStateInfo, int iLayerIndex)
        {
            BYOS.Instance.Interaction.TextToSpeech.Say(BYOS.Instance.Dictionary.GetRandomString("lookingfor"));
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iAnimatorStateInfo, int iLayerIndex)
        {
            if (BYOS.Instance.Interaction.TextToSpeech.HasFinishedTalking)
            {
                if (!mIsPing)
                    StartCoroutine(CheckConnection());

                if (mEnd)
                    Trigger("VOCALCOMMAND");
            }
        }

        private IEnumerator CheckConnection()
        {
            mIsPing = true;
            const float lTimeout = 10f;
            float lStartTime = Time.timeSinceLevelLoad;
            Ping lPing = new Ping("8.8.8.8");

            BYOS.Instance.Primitive.WiFi.StartWifiScan();

            while (true)
            {
                if (lPing.isDone)
                {
                    BYOS.Instance.Interaction.TextToSpeech.Say(BYOS.Instance.Dictionary.GetRandomString("connected").Replace("[name]", BYOS.Instance.Primitive.WiFi.GetCurrentWifiAPName()), false);
                    mEnd = true;
                    yield break;
                }
                if (Time.timeSinceLevelLoad - lStartTime > lTimeout)
                {
                    BYOS.Instance.Interaction.TextToSpeech.Say(BYOS.Instance.Dictionary.GetRandomString("notconnected"));
                    mEnd = true;
                    yield break;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iAnimatorStateInfo, int iLayerIndex)
        {
            mIsPing = false;
            mEnd = false;
        }
    }
}