using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS.App;
using BuddyTools;
using OpenCVUnity;

namespace BuddyApp.HideAndSeek
{
    public class SavePlayersFaces : AStateMachineBehaviour
    {
        private SaveFacesWindow mSaveFacesWindow;
        private FaceRecognition mFaceReco;
        private Button mButtonTrain;
        private bool mHasTrained = false;
        private bool mHasStarted = false;
        private Texture2D mTexture;
        private int mNumFacesSaved = 0;
        private const float NUM_FACE_MAX = 150.0f;
        private bool mHasClosed = false;

        public override void Init()
        {
            mSaveFacesWindow = GetGameObject((int)HideAndSeekData.ObjectsLinked.WINDOW_LINKER).GetComponentInChildren<SaveFacesWindow>();
            mFaceReco = GetGameObject((int)HideAndSeekData.ObjectsLinked.FACE_RECO).GetComponent<FaceRecognition>();
            //mButtonTrain = mFaceReco.mButtonTrain;
            //mButtonTrain.onClick.AddListener(Train);
            mHasStarted = false;
            
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            
            if (!mRGBCam.IsOpen)
                mRGBCam.Open();
            GetGameObject((int)HideAndSeekData.ObjectsLinked.FACE_RECO).SetActive(true);
            mSaveFacesWindow.Open();
            mSaveFacesWindow.ButtonGo.interactable = true;
            mSaveFacesWindow.ButtonGo.onClick.AddListener(StartLabel);
            mSaveFacesWindow.ScrollLoading.size = 0;
            mSaveFacesWindow.ImageToDisplay.texture = new Texture2D(100, 100);
            mNumFacesSaved = 0;
            mHasStarted = false;
            mHasClosed = false;
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //if (mFaceReco.IsTrained && !mHasTrained)
            //{
            //    mHasTrained = true;
            //    mTTS.Say("J ai retenu les visages");
            //    iAnimator.SetTrigger("ChangeState");
            //}
            //Debug.Log("num face: " + mNumFacesSaved);
            if (mHasStarted && mNumFacesSaved <= NUM_FACE_MAX)
            {
                //Debug.Log("1 ");
                if (mFaceReco.FaceAct != null && mFaceReco.NumFacesSaved>0)
                {
                    mNumFacesSaved = mFaceReco.NumFacesSaved;
                    //Debug.Log("num label: " + mNumFacesSaved);
                    float lProportionFace=mNumFacesSaved / NUM_FACE_MAX;
                    mSaveFacesWindow.ScrollLoading.size = lProportionFace;
                    //Utils.MatToTexture2D(mFaceReco.FaceAct, mTexture);
                    mSaveFacesWindow.ImageToDisplay.texture = Utils.MatToTexture2D(mFaceReco.FaceAct);//mTexture;
                }
            }

            else if(mNumFacesSaved > NUM_FACE_MAX && !mHasClosed)
            {
                //Debug.Log("2 ");
                Player lPlayer = new Player("", mFaceReco.FaceAct);
                GetComponent<Players>().AddPlayer(lPlayer);
                Debug.Log("stop");
                mFaceReco.StopLabel();
                //GetGameObject((int)HideAndSeekData.ObjectsLinked.FACE_RECO).SetActive(false);
                mSaveFacesWindow.Close();
                mHasClosed = true;
            }
            else if(mHasClosed && mSaveFacesWindow.IsOff())
            {
                //Debug.Log("3" );
                if(mRGBCam.IsOpen)
                    mRGBCam.Close();
                iAnimator.SetTrigger("ChangeState");
            }

        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //mButtonTrain.onClick.RemoveAllListeners();
            //GetComponent<Players>().NumPlayer = mFaceReco.NbLabel;
            //mFace.SetExpression(MoodType.NEUTRAL);
            mSaveFacesWindow.ButtonGo.onClick.RemoveAllListeners();
            iAnimator.ResetTrigger("ChangeState");

        }

        private void StartLabel()
        {
            mHasStarted = true;
            mFaceReco.StartLabel();
            mSaveFacesWindow.ButtonGo.interactable = false;
        }

        private void Train()
        {
            mFace.SetExpression(MoodType.THINKING);
            GetGameObject(3).SetActive(false);
            mFaceReco.Train();
        }
    }
}