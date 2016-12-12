using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS.App;

namespace BuddyApp.HideAndSeek
{
    public class SavePlayersFaces : AStateMachineBehaviour
    {

        private FaceTrackerTest mFaceReco;
        private Button mButtonTrain;
        private bool mHasTrained = false;

        public override void Init()
        {
            mFaceReco = GetGameObject(1).GetComponent<FaceTrackerTest>();
            mButtonTrain = mFaceReco.mButtonTrain;
            mButtonTrain.onClick.AddListener(Train);
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GetGameObject(3).SetActive(true);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mFaceReco.IsTrained && !mHasTrained)
            {
                mHasTrained = true;
                mTTS.Say("J ai retenu les visages");
                iAnimator.SetTrigger("ChangeState");
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mButtonTrain.onClick.RemoveAllListeners();
            //GetComponent<Players>().NumPlayer = mFaceReco.NbLabel;
            mFace.SetExpression(MoodType.NEUTRAL);
            iAnimator.ResetTrigger("ChangeState");

        }

        private void Train()
        {
            mFace.SetExpression(MoodType.THINKING);
            GetGameObject(3).SetActive(false);
            mFaceReco.Train();
        }
    }
}