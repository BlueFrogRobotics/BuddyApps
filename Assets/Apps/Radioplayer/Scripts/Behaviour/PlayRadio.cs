using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.Radioplayer
{
    public class PlayRadio : AStateMachineBehaviour
    {

        private RadioStream mStream;
        private float mTimer;
        private const float REFRESH_TIME = 5.0F;

        public override void Start()
        {
            mStream = GetComponent<RadioStream>();
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer = 0.0F;
            StartCoroutine(SearchAndPlay());
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;
            if (mTimer > REFRESH_TIME && !mStream.IsUpdatingLiveInfos)
            {
                mStream.UpdateLiveInformations();
                mTimer = 0.0F;
            }

        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }

        private IEnumerator SearchAndPlay()
        {
            if (RadioplayerData.Instance.DefaultRadio != null && RadioplayerData.Instance.DefaultRadio != "")
            {
                Debug.Log("1 truc");
                yield return mStream.SearchRadioName(RadioplayerData.Instance.DefaultRadio);
                Debug.Log("permalink: " + mStream.Permalink);
                //RadioplayerData.Instance.DefaultRadio = mStream.Permalink;
                mStream.Play(mStream.Permalink);
            }
        }
    }
}