﻿using UnityEngine;
using System.Collections;
using BuddyTools;
using BuddyOS.App;

namespace BuddyApp.HideAndSeek
{
    public class SearchPeopleState : AStateMachineBehaviour
    {

        private FaceDetector mFaceDetector;
        private FaceRecognition mFaceReco;
        private SearchFacesWindow mSearchWindow;
        private Players mPlayers;
        private int mLabelFound=-1;
        private double mDist = 0;
        private float mTimer = 0.0f;
        private bool mHasFoundFace = false;

        public override void Init()
        {
            
            mFaceDetector = GetGameObject((int)HideAndSeekData.ObjectsLinked.FACE_RECO).GetComponent<FaceDetector>();
            mFaceReco = GetGameObject((int)HideAndSeekData.ObjectsLinked.FACE_RECO).GetComponent<FaceRecognition>();
            mSearchWindow = GetGameObject((int)HideAndSeekData.ObjectsLinked.WINDOW_LINKER).GetComponentInChildren<SearchFacesWindow>();
            mPlayers = GetComponent<Players>();
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer = 0.0f;
            mTTS.Say(mDictionary.GetString("showFace"));//"Montre ton visage");
            //mFace.SetExpression(MoodType.THINKING);
            mMood.Set(MoodType.THINKING);
            //GetGameObject(1).SetActive(true);
            mSearchWindow.Open();
            mHasFoundFace = false;
            mYesHinge.SetPosition(-10);
            if (!mRGBCam.IsOpen)
                mRGBCam.Open();
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;
            mSearchWindow.CamView.texture = Utils.MatToTexture2D(mFaceDetector.CamView);
            if (mTimer > 3.0f)
            {
                if (mFaceDetector.HasDetectedFace)
                {
                    //double lDist = 0;
                    mHasFoundFace = true;
                    mLabelFound = mFaceReco.Predict(out mDist);
                    Debug.Log("label: " + mLabelFound + "dist: " + mDist);
                    /* if (lLabel != -1 && lDist<100)
                     {
                         mTTS.Say("connu");
                         Debug.Log("dist: " + lDist);
                     }*/
                    iAnimator.SetTrigger("ChangeState");
                }
            }
            if (mTimer > 5.0f)
            {
                mLabelFound = -1;
                iAnimator.SetTrigger("ChangeState");
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //mTTS.Say("Je t ai trouvé");
            mSearchWindow.Close();
            if (mRGBCam.IsOpen)
                mRGBCam.Close();
            //GetGameObject(1).SetActive(false);
            //mFace.SetExpression(MoodType.NEUTRAL);
            mMood.Set(MoodType.NEUTRAL);
            if (mLabelFound != -1 && mDist < 100)
            {
                
                bool lHasAlreadyFound=GetComponent<Players>().DeleteOnePlayer(mLabelFound);
                if (!lHasAlreadyFound)
                    mTTS.Say(mDictionary.GetString("iFound") + " " + mPlayers.GetPlayer(mLabelFound).Name);
                else
                    mTTS.Say(mDictionary.GetString("alreadyFound"));//"T'es plus dans le game");
                Debug.Log("dist: " + mDist);
            }
            else if(mHasFoundFace)
                mTTS.Say(mDictionary.GetString("cantReco"));

            iAnimator.ResetTrigger("ChangeState");
            mYesHinge.SetPosition(20);
        }
    }
}