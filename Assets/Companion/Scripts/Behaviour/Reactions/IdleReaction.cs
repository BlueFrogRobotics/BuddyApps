using UnityEngine;
using BuddyOS;
using BuddyOS.Command;
using System.Collections;

namespace BuddyApp.Companion
{
    public class IdleReaction : MonoBehaviour
    {
        private float mHeadMoveTime;
        private float mEmoteTime;

        void OnEnable()
        {
            mHeadMoveTime = Time.time;
            mEmoteTime = Time.time;
            new SetMoodCmd(MoodType.NEUTRAL).Execute();
            //StartCoroutine(IdleBehaviorCo());
        }

        void Update()
        {
            if (Time.time - mHeadMoveTime < 1.5F)
                return;

            mHeadMoveTime = Time.time;

            if (Random.Range(0, 2) == 0)
            {
                float lRandomAngle = Random.Range(-45F, 45F);
                new SetPosNoCmd(lRandomAngle).Execute();
            }
            else
            {
                float lRandomAngle = Random.Range(-20F, 15F);
                new SetPosYesCmd(lRandomAngle).Execute();
            }
            
            if (Time.time - mEmoteTime > 15F)
            {
                new SetMouthEvntCmd(MouthEvent.SMILE).Execute();
                mEmoteTime = Time.time;
            }
        }

        void OnDisable()
        {
            new SetPosNoCmd(0F).Execute();
            new SetPosYesCmd(0F).Execute();
        }

        private IEnumerator IdleBehaviorCo()
        {
            new SetMoodCmd(MoodType.NEUTRAL);

            Debug.Log("Can move head " + CompanionData.Instance.CanMoveHead);

            while(enabled && CompanionData.Instance.CanMoveHead) {
                if(Random.Range(0, 1) == 0) {
                    float lRandomAngle = Random.Range(-45F, 45F);
                    new SetPosNoCmd(lRandomAngle).Execute();
                }
                else {
                    float lRandomAngle = Random.Range(-30F, 15F);
                    new SetPosYesCmd(lRandomAngle).Execute();
                }

                yield return new WaitForSeconds(1.5F);

                if(Time.time - mEmoteTime > 15F) {
                    new SetMouthEvntCmd(MouthEvent.SMILE);
                    mEmoteTime = Time.time;
                }
            }
        }
    }
}