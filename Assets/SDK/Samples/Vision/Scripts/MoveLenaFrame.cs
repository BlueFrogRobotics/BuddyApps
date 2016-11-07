using UnityEngine;
using System.Collections;

namespace BuddySample
{
    public class MoveLenaFrame : MonoBehaviour
    {
        private Vector3 mMaxPos;
        private Vector3 mMinPos;
        private float mElapsedTime;
        private bool mForward;

        void Start()
        {
            mForward = true;
            mMinPos = new Vector3(-0.5F, 0.517F, 0.425F);
            mMaxPos = new Vector3(1.1F, 0.517F, 0.425F);
        }

        // Update is called once per frame
        void Update()
        {
            mElapsedTime += Time.deltaTime;

            if (mElapsedTime >= 10F) {
                mElapsedTime = 0;
                mForward = !mForward;
            }

            if (mForward)
                transform.localPosition = Vector3.Lerp(mMinPos, mMaxPos, mElapsedTime / 10F);
            else
                transform.localPosition = Vector3.Lerp(mMaxPos, mMinPos, mElapsedTime / 10F);
        }
    }
}