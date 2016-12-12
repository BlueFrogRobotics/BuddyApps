using UnityEngine;
using System.Collections;
using BuddyOS.App;

namespace BuddyApp.HideAndSeek
{
    public class SearchPeopleState : AStateMachineBehaviour
    {

        private FaceDetector mFaceDetector;
        private FaceTrackerTest mFaceReco;
        private int mLabelFound=-1;
        private double mDist = 0;

        public override void Init()
        {
            mFaceDetector = GetGameObject(1).GetComponent<FaceDetector>();
            mFaceReco = GetGameObject(1).GetComponent<FaceTrackerTest>();
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mFaceDetector.HasDetectedFace)
            {
                //double lDist = 0;
                mLabelFound = mFaceReco.Predict(out mDist);
               /* if (lLabel != -1 && lDist<100)
                {
                    mTTS.Say("connu");
                    Debug.Log("dist: " + lDist);
                }*/
                iAnimator.SetTrigger("ChangeState");
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //mTTS.Say("Je t ai trouvé");
            if (mLabelFound != -1 && mDist < 100)
            {
                mTTS.Say("J ai trouvé le joueur "+(mLabelFound+1));
                GetComponent<Players>().DeleteOnePlayer();
                Debug.Log("dist: " + mDist);
            }
            iAnimator.ResetTrigger("ChangeState");
        }
    }
}