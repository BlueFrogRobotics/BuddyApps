using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System;
using UnityEngine.UI;

namespace BuddyApp.CoursTelepresence
{

    public sealed class CallState : AStateMachineBehaviour
    {

        [SerializeField]
        private Scrollbar VolumeScrollbar;
        [SerializeField]
        private Button Volume;
        [SerializeField]
        private Button Video;
        [SerializeField]
        private Button Micro;
        [SerializeField]
        private Button VideoFeedback;
        [SerializeField]
        private Button Hangup;

        private RTMManager mRTMManager;
        private float mTimeVolume;


        // Use this for initialization
        override public void Start()
        {
            VolumeScrollbar = GetGameObject(4).GetComponentInChildren<Scrollbar>();
            Volume = GetGameObject(5).GetComponentInChildren<Button>();
            Video = GetGameObject(6).GetComponentInChildren<Button>();
            Micro = GetGameObject(7).GetComponentInChildren<Button>();
            VideoFeedback = GetGameObject(8).GetComponentInChildren<Button>();
            Hangup = GetGameObject(9).GetComponentInChildren<Button>();

            VolumeScrollbar.onValueChanged.AddListener(
                (lValue) => {
                    Debug.Log("Volume set to " + lValue);
                    Buddy.Actuators.Speakers.Volume = lValue;
                    mTimeVolume = Time.time;
                });

            Volume.onClick.AddListener(
                () => {
                    VolumeScrollbar.gameObject.SetActive(!VolumeScrollbar.gameObject.activeInHierarchy);
                    mTimeVolume = Time.time;
                    // TODO update button image
                }
                );


            Video.onClick.AddListener(
                () => {
                    // TODO update button image
                    //TODO update video broadcast or not
                }
                );


            Micro.onClick.AddListener(
                () => {
                    // TODO update button image
                    //TODO update micro broadcast or not
                }
                );

            VideoFeedback.onClick.AddListener(
                () => {
                    // TODO update button image
                    //TODO update video feedback or not
                }
                );

            Hangup.onClick.AddListener(
                () => {
                    // Display message are you sure? ...
                }
                );
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("call state");


            Volume.gameObject.SetActive(true);
            Video.gameObject.SetActive(true);
            Micro.gameObject.SetActive(true);
            VideoFeedback.gameObject.SetActive(true);
            Hangup.gameObject.SetActive(true);

        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (VolumeScrollbar.gameObject.activeInHierarchy && Time.time - mTimeVolume > 5.0F)
                VolumeScrollbar.gameObject.SetActive(false);
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("call state exit");
            VolumeScrollbar.gameObject.SetActive(false);
            Volume.gameObject.SetActive(false);
            Video.gameObject.SetActive(false);
            Micro.gameObject.SetActive(false);
            VideoFeedback.gameObject.SetActive(false);
            Hangup.gameObject.SetActive(false);
        }
    }

}