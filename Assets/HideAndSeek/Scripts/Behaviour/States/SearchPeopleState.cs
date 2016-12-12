using UnityEngine;
using System.Collections;
using BuddyOS.App;

namespace BuddyApp.HideAndSeek
{
    public class SearchPeopleState : AStateMachineBehaviour
    {

        private FaceDetector mFaceDetector;
        private FaceTrackerTest mFaceReco;
        private Players mPlayers;
        private int mLabelFound=-1;
        private double mDist = 0;
        private float mTimer = 0.0f;

        public override void Init()
        {
            mFaceDetector = GetGameObject(1).GetComponent<FaceDetector>();
            mFaceReco = GetGameObject(1).GetComponent<FaceTrackerTest>();
            mPlayers = GetComponent<Players>();
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer = 0.0f;
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;
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

            else if (mTimer > 5.0f)
            {
                mLabelFound = -1;
                iAnimator.SetTrigger("ChangeState");
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //mTTS.Say("Je t ai trouvé");
            if (mLabelFound != -1 && mDist < 100)
            {
                
                bool lHasAlreadyFound=GetComponent<Players>().DeleteOnePlayer(mLabelFound);
                if (!lHasAlreadyFound)
                    mTTS.Say("J'ai trouvé  " + mPlayers.NamesPlayers[mLabelFound]);
                else
                    mTTS.Say("T'es plus dans le game");
                Debug.Log("dist: " + mDist);
            }
            iAnimator.ResetTrigger("ChangeState");
        }
    }
}