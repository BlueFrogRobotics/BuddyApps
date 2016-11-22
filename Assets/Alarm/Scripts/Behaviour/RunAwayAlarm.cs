﻿using UnityEngine;
using BuddyOS.Command;
using BuddyFeature.Navigation;

namespace BuddyApp.Alarm
{
    [RequireComponent(typeof(RoombaNavigation))]
    public class RunAwayAlarm : MonoBehaviour
    {
        [SerializeField]
        private AudioSource ringer;

        [SerializeField]
        private GameObject pauseScreen;

        [SerializeField]
        private GameObject pauseButton;

        [SerializeField]
        private GameObject halo1;

        [SerializeField]
        private GameObject halo2;

        [SerializeField]
        private GameObject halo3;

        [SerializeField]
        private GameObject halo4;

        private RoombaNavigation mRandomNavigation;
        private int mCountToChangePauseButtPos;
        private float mOffsetHalo = 99.5f;

        // TODO : 
        // - add a timeout in case the alarm is triggered without any user answering (can be a parameter)
        // - set the flag to private when debug done
        // - remove the public from the stopAlarm();
        // - See TODO 1,2,3

        // How to use 
        // initAlarm() or start() by default
        // startAlarm()
        // stopAlarm()

        public bool IsAlarmStarted { get; set; }
        public bool IsAlarmStopped { get; set; }

        // Use this for initialization
        void Start()
        {

            InitAlarm();
            mCountToChangePauseButtPos = 0;
            mRandomNavigation = GetComponent<RoombaNavigation>();
            // do the requiered call
        }

       
        // Update is called once per frame
        void Update()
        {
            // if the alarm is running 
            if (IsAlarmStarted && !IsAlarmStopped) {
                // check if the song is still running,
                // if not start over the song
                // >> this should be handle by the loop function of the audio source

                // see if there is an obstacle
                // if so, change direction
                // >> this should be handle in the roaming script

                // move the position of the target button
                //positionButton.transform.position = new Vector3(                Random.Range(0, sizeScreenX),                 Random.Range(0, sizeScreenX),                 Random.Range(0, sizeScreenX));

                // TODO 3 : change the position of the button depending of the time 
                if (mCountToChangePauseButtPos > 170) {
                    // TODO 1 : choose the range depending of the screen size
                    Vector3 positionTarget = new Vector3(
                        pauseButton.transform.parent.transform.position.x + Random.Range(-300, 300),
                        pauseButton.transform.parent.transform.position.y + Random.Range(-300, 300),
                        pauseButton.transform.parent.transform.position.z + Random.Range(-300, 300));

                    // TODO 2 : change the position of all the elements at once
                    pauseButton.transform.position = positionTarget;
                    halo1.transform.position = new Vector3(positionTarget.x - mOffsetHalo, positionTarget.y + mOffsetHalo, 0);
                    halo2.transform.position = new Vector3(positionTarget.x + mOffsetHalo, positionTarget.y + mOffsetHalo, 0);
                    halo3.transform.position = new Vector3(positionTarget.x - mOffsetHalo, positionTarget.y - mOffsetHalo, 0);
                    halo4.transform.position = new Vector3(positionTarget.x + mOffsetHalo, positionTarget.y - mOffsetHalo, 0);
                    mCountToChangePauseButtPos = 0;
                } else
                    mCountToChangePauseButtPos++;
            }
        }

        // call when the alarm is triggered
        public void StartAlarm()
        {
            // set a flag
            IsAlarmStarted = true;

            // show a button or equivalent on the screen
            //PauseScreen.enabled = true;
            pauseScreen.SetActive(true);

            // start playing a song
            ringer.Play();

            // start moving
            mRandomNavigation.enabled = true;
        }

        // called when the user push the button
        public void StopAlarm()
        {
            // set flag
            IsAlarmStopped = true;

            // remove the button of the screen
            pauseScreen.SetActive(false);

            // stop the song
            ringer.Stop();

            // stop moving
            mRandomNavigation.enabled = false;

            new HomeCmd().Execute();
        }

        private void InitAlarm()
        {
            IsAlarmStarted = false;
            IsAlarmStopped = false;
        }
    }
}