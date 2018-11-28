using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

using OpenCVUnity;
using UnityEngine.UI;

namespace BuddyApp.HumanCounter
{
    public sealed class ObservationTimeSettingsState : AStateMachineBehaviour
    {
        private const int MINIMUM_TIME = 10;
        private const int MAXIMUM_TIME = 3600;
        private const int DEFAULT_OBSERVATION_TIME = 30;
        private const int TIME_INCREMENT = 10;

        private TText mSettingMessage;
        private TToggle mToggleDetect;
        private TButton mButtonEnum;
        private string mTimeInfo;
        private string mNameDetectOption;

        private delegate void DrawLine(Mat iMat, Point iFirstPoint, Point iSecondPoint, Scalar iColor, SkeletonJoint iJoint);
        private Dictionary<Tuple<int, int>, string> mLinksDico = new Dictionary<Tuple<int, int>, string>();

        /*
         *  Temporary parameter toaster to set the observation time.
         *  This will be replace by carrousel toast when available.
         */
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mLinksDico.Add(new Tuple<int, int>(0, 0), "PAIR: 0, 0");
            mLinksDico.Add(new Tuple<int, int>(0, 1), "PAIR: 0, 1");
            mLinksDico.Add(new Tuple<int, int>(0, 2), "PAIR: 0, 2");
            mLinksDico.Add(new Tuple<int, int>(5, 5), "PAIR: 5, 5");
            try
            {
                Debug.Log("TEST 0,0: " + mLinksDico[new Tuple<int, int>(0, 0)]);
                Debug.Log("TEST 0,1: " + mLinksDico[new Tuple<int, int>(0, 1)]);
                Debug.Log("TEST 0,2: " + mLinksDico[new Tuple<int, int>(0, 2)]);
                Debug.Log("TEST 5,5: " + mLinksDico[new Tuple<int, int>(5, 5)]);

                // erreur
                Debug.Log("TEST 0,3: " + mLinksDico[new Tuple<int, int>(0, 3)]);
                Debug.Log("TEST 4,0: " + mLinksDico[new Tuple<int, int>(4, 0)]);
                Debug.Log("TEST -1,0: " + mLinksDico[new Tuple<int, int>(-1, 0)]);
                Debug.Log("TEST 10,10: " + mLinksDico[new Tuple<int, int>(10, 10)]);
            }
            catch (KeyNotFoundException e)
            {
                Debug.Log("KEY NOT FOUND: " + e.Message);
            }

            if (HumanCounterData.Instance.DetectionOption == DetectionOption.HUMAN_DETECT)
                mNameDetectOption = Buddy.Resources.GetString("humandetect");
            else if (HumanCounterData.Instance.DetectionOption == DetectionOption.FACE_DETECT)
                mNameDetectOption = Buddy.Resources.GetString("facedetect");
            else
                mNameDetectOption = Buddy.Resources.GetString("skeletondetect");
            Buddy.Behaviour.SetMood(Mood.THINKING, true);

            // Set Title with a custom font
            Font lHeaderFont = Buddy.Resources.Get<Font>("os_awesome");
            lHeaderFont.material.color = Color.white;
            Buddy.GUI.Header.SetCustomLightTitle(lHeaderFont); 
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("timertitle"));

            // Create the top left button to go back to Head settings
            FButton lBackButton = Buddy.GUI.Footer.CreateOnLeft<FButton>();
            lBackButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_arrow_left"));
            lBackButton.OnClick.Add(() => { Trigger("BackToHeadSettings"); });

            // Default observation time
            HumanCounterData.Instance.ObservationTime = DEFAULT_OBSERVATION_TIME;

            Buddy.GUI.Toaster.Display<ParameterToast>().With((iOnBuild) =>
            {
                // Create a button to increment the time.
                TButton lIncrementTime = iOnBuild.CreateWidget<TButton>();
                lIncrementTime.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_plus"));
                lIncrementTime.SetLabel(Buddy.Resources.GetString("incrementtimer"));
                // On click TIME_INCREMENT is add to the timer and the text is updated.
                lIncrementTime.OnClick.Add(() => 
                {
                    HumanCounterData.Instance.ObservationTime += TIME_INCREMENT;
                    UpdateTimeInfo();
                });

                // Create the text to inform the user about the timer value.
                mTimeInfo = (HumanCounterData.Instance.ObservationTime / 60).ToString();
                mTimeInfo += "m:" + (HumanCounterData.Instance.ObservationTime % 60) + "s";
                mSettingMessage = iOnBuild.CreateWidget<TText>();
                mSettingMessage.SetLabel(Buddy.Resources.GetString("timerinfo") + mTimeInfo);

                // Create a button to decrement the time.
                TButton lDecrementTime = iOnBuild.CreateWidget<TButton>();
                lDecrementTime.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_minus"));
                lDecrementTime.SetLabel(Buddy.Resources.GetString("decrementtimer"));
                // On click TIME_INCREMENT is substract to the timer and the text is updated.
                lDecrementTime.OnClick.Add(() => 
                {
                    HumanCounterData.Instance.ObservationTime -= TIME_INCREMENT;
                    UpdateTimeInfo();
                });

                // Create a button to reset to default observation time.
                TButton lResetTime = iOnBuild.CreateWidget<TButton>();
                lResetTime.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_spinning"));
                lResetTime.SetLabel(Buddy.Resources.GetString("resettimer"));
                // On click reset the timer to DEFAULT_OBSERVATION_TIME and the text is updated.
                lResetTime.OnClick.Add(() => 
                {
                    HumanCounterData.Instance.ObservationTime = DEFAULT_OBSERVATION_TIME;
                    UpdateTimeInfo();
                });

                // Create the button to switch between detection mode
                mButtonEnum = iOnBuild.CreateWidget<TButton>();
                mButtonEnum.SetLabel(mNameDetectOption);
                mButtonEnum.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_emoji"));
                mButtonEnum.OnClick.Add(() =>
                {
                    DialogerDropDown();
                });
            },
            // Click left.
            () => { /* Back to next settings when available. */ },
            // Left label
            Buddy.Resources.GetString("cancel"),
            // Click right.
            () => { Trigger("ObservationView"); }
            // Right Label.
            , Buddy.Resources.GetString("next"));
        }

        private void DialogerDropDown()
        {
            Buddy.GUI.Dialoger.Display<VerticalListToast>().With((iOnBuilder) =>
            {
                // Human detect button 
                TVerticalListBox lBoxFirst = iOnBuilder.CreateBox();
                lBoxFirst.OnClick.Add(() => { HumanCounterData.Instance.DetectionOption = DetectionOption.HUMAN_DETECT; UpdateOptionDetectText(); Buddy.GUI.Dialoger.Hide(); });
                lBoxFirst.SetLabel(Buddy.Resources.GetString("humandetect"));

                // Face detect button
                TVerticalListBox lBoxSecond = iOnBuilder.CreateBox();
                lBoxSecond.OnClick.Add(() => { HumanCounterData.Instance.DetectionOption = DetectionOption.FACE_DETECT; UpdateOptionDetectText(); Buddy.GUI.Dialoger.Hide(); });
                lBoxSecond.SetLabel(Buddy.Resources.GetString("facedetect"));
                
                // Skeleton detect button
                TVerticalListBox lBoxThird = iOnBuilder.CreateBox();
                lBoxThird.OnClick.Add(() => { HumanCounterData.Instance.DetectionOption = DetectionOption.SKELETON_DETECT; UpdateOptionDetectText(); Buddy.GUI.Dialoger.Hide(); });
                lBoxThird.SetLabel(Buddy.Resources.GetString("skeletondetect"));

            });
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            mLinksDico.Clear();
        }

        private void UpdateOptionDetectText()
        {
            if (HumanCounterData.Instance.DetectionOption == DetectionOption.HUMAN_DETECT)
                mNameDetectOption = Buddy.Resources.GetString("humandetect");
            else if (HumanCounterData.Instance.DetectionOption == DetectionOption.FACE_DETECT)
                mNameDetectOption = Buddy.Resources.GetString("facedetect");
            else
                mNameDetectOption = Buddy.Resources.GetString("skeletondetect");

            mButtonEnum.SetLabel(mNameDetectOption);
        }

        private void UpdateTimeInfo()
        {
            // Check time consistency.
            if (HumanCounterData.Instance.ObservationTime < MINIMUM_TIME)
                HumanCounterData.Instance.ObservationTime = MINIMUM_TIME;
            if (HumanCounterData.Instance.ObservationTime > MAXIMUM_TIME)
                HumanCounterData.Instance.ObservationTime = MAXIMUM_TIME;

            // Update label TText.
            mTimeInfo = (HumanCounterData.Instance.ObservationTime / 60).ToString();
            mTimeInfo += "m:" + (HumanCounterData.Instance.ObservationTime % 60) + "s";
            mSettingMessage.SetLabel(Buddy.Resources.GetString("timerinfo") + mTimeInfo);
        }
    }
}
