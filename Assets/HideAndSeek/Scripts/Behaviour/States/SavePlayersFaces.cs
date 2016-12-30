using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS;
using BuddyOS.App;
using BuddyTools;
using OpenCVUnity;

namespace BuddyApp.HideAndSeek
{
    public class SavePlayersFaces : AStateMachineBehaviour
    {
        private SaveFacesWindow mSaveFacesWindow;
        private FaceRecognition mFaceReco;
        private FaceDetector mFaceDetector;
        private Button mButtonTrain;
        private bool mHasStarted = false;
        private Texture2D mTexture;
        private int mNumFacesSaved = 0;
        private const float NUM_FACE_MAX = 150.0f;
        private bool mHasClosed = false;
        private bool mIsListening = false;
        private float mTimer = 0.0f;

        public override void Init()
        {
            mSaveFacesWindow = GetGameObject((int)HideAndSeekData.ObjectsLinked.WINDOW_LINKER).GetComponentInChildren<SaveFacesWindow>();
            mFaceReco = GetGameObject((int)HideAndSeekData.ObjectsLinked.FACE_RECO).GetComponent<FaceRecognition>();
            mFaceDetector = GetGameObject((int)HideAndSeekData.ObjectsLinked.FACE_RECO).GetComponent<FaceDetector>();
            //mButtonTrain = mFaceReco.mButtonTrain;
            //mButtonTrain.onClick.AddListener(Train);
            mHasStarted = false;
            
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GetGameObject((int)HideAndSeekData.ObjectsLinked.WINDOW_LINKER).GetComponent<WindowLinker>().SetAppWhite();
            if (!mRGBCam.IsOpen)
                mRGBCam.Open();
            GetGameObject((int)HideAndSeekData.ObjectsLinked.FACE_RECO).SetActive(true);
            mSaveFacesWindow.Open();
            mSaveFacesWindow.ButtonGo.gameObject.SetActive(true); //interactable = true;
            mSaveFacesWindow.ButtonGo.onClick.AddListener(StartLabel);
            mSaveFacesWindow.ScrollLoading.size = 0;
            mSaveFacesWindow.ImageToDisplay.texture = new Texture2D(100, 100);
            mNumFacesSaved = 0;
            mHasStarted = false;
            mHasClosed = false;
            mIsListening = false;
            mTTS.Say(mDictionary.GetString("askStartReco"));
            mSTT.OnBestRecognition.Add(VocalProcessing);
            mSTT.OnBeginning.Add(StartListening);
            mSTT.OnEnd.Add(StopListening);
            //mVocalActivation.VocalProcessing = VocalProcessing;
            //mVocalActivation.StartListenBehaviour = StartListening;
            //mVocalActivation.StopListenBehaviour = StopListening;
            mTimer = 0.0f;
            mYesHinge.SetPosition(-10);
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
            mTimer += Time.deltaTime;
            mSaveFacesWindow.ImageToDisplay.texture = Utils.MatToTexture2D(mFaceDetector.CamView);//mTexture;
            if (!mHasStarted && mTTS.HasFinishedTalking)
            {
                if (!mIsListening && mTimer > 3.0f)
                {
                    //mVocalActivation.StartInstantReco();
                    mSTT.Request();
                    mTimer = 0.0f;
                }
            }
            else if (mHasStarted && mNumFacesSaved <= NUM_FACE_MAX)
            {
                //Debug.Log("1 ");
                if (mFaceReco.FaceAct != null && mFaceReco.NumFacesSaved>0)
                {
                    mNumFacesSaved = mFaceReco.NumFacesSaved;
                    //Debug.Log("num label: " + mNumFacesSaved);
                    float lProportionFace=mNumFacesSaved / NUM_FACE_MAX;
                    mSaveFacesWindow.ScrollLoading.size = lProportionFace;
                    //Utils.MatToTexture2D(mFaceReco.FaceAct, mTexture);
                    //mSaveFacesWindow.ImageToDisplay.texture = Utils.MatToTexture2D(mFaceReco.FaceAct);//mTexture;
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
            mSTT.OnBestRecognition.Remove(VocalProcessing);
            mSTT.OnBeginning.Remove(StartListening);
            mSTT.OnEnd.Remove(StopListening);
            mSaveFacesWindow.ButtonGo.onClick.RemoveAllListeners();
            iAnimator.ResetTrigger("ChangeState");

        }

        private void StartLabel()
        {
            mSpeaker.FX.Play(FXSound.BEEP_1);
            mHasStarted = true;
            mFaceReco.StartLabel();
            mSaveFacesWindow.ButtonGo.gameObject.SetActive(false);// interactable = false;
        }

        private void Train()
        {
            mFace.SetExpression(MoodType.THINKING);
            //GetGameObject(3).SetActive(false);
            mFaceReco.Train();
        }

        private void VocalProcessing(string iRequest)
        {
            string lRequest = iRequest.ToLower();
            if (lRequest.Contains("part") || lRequest.Contains("go") || lRequest.Contains("aller"))
            {
                if (!mHasStarted)
                    StartLabel();
            }
        }

        private void StartListening()
        {
            mIsListening = true;
        }

        private void StopListening()
        {
            mIsListening = false;
        }
    }
}