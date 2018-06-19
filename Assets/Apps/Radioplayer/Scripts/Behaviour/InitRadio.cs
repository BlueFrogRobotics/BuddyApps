using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;

namespace BuddyApp.Radioplayer
{
    public class InitRadio : AStateMachineBehaviour
    {

        private RadioStream mStream;

        public override void Start()
        {
            mStream = GetComponent<RadioStream>();
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if ((RadioplayerData.Instance.Token == null || RadioplayerData.Instance.Token == "") || (RadioplayerData.Instance.TokenCreationDate!=null && RadioplayerData.Instance.TokenCreationDate != "" && (DateTime.Now - DateTime.ParseExact(RadioplayerData.Instance.TokenCreationDate, "yyyy-MM-dd", CultureInfo.InvariantCulture)).Days>=1))
            {
                RadioplayerData.Instance.Token = mStream.GetToken();
                RadioplayerData.Instance.MyValue = 10;
                RadioplayerData.Instance.TokenCreationDate = DateTime.Now.ToString("yyyy-MM-dd");
            }
            Trigger("Play");
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }
    }
}