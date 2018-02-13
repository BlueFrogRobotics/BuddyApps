﻿using Buddy;
using Buddy.UI;

using System;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

using Buddy.Command;
using System.Globalization;

namespace BuddyApp.Reminder
{
    public class GetReminders : AStateMachineBehaviour
    {

        private ReminderManager mReminderManager;
        private RemindersData mReminders;
        private ReminderLayout mLayout;

        private AudioSource aud;
        private string NewUser;
        private string KeyUser;
        private float mSongLength;
        private float mTime;
        private float mDiff;


        private string url = "file:///" + BYOS.Instance.Resources.GetPathToRaw("[reminder].wav");

        // Use this for initialization
        public override void Start()
        {
            mReminderManager = GetComponent<ReminderManager>();
            mReminders = mReminderManager.RemindersData;

            mLayout = new ReminderLayout()
            {
                HearCallback = PlayReminder,
                UserSelectCallback = GetUserString,
                Users = mReminders
              };
            aud = GetComponent<AudioSource>();


            aud.loop = false;
            //WWW audioLoader = new WWW(url);
            //while (!audioLoader.isDone)
            //{
            //    Debug.Log("uploading");
            //}

            //Debug.Log("1");

            //aud.clip = audioLoader.GetAudioClip(false, false, AudioType.WAV);
            //Essayer AVEC LA TECHNIQUE LOADINGMUSICSTATE pour que sa marche / peut etre///

        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (Reminderkey key in mReminders.Reminders)
            {
                Debug.Log("NAME : " + key.Name);
                Debug.Log("KEY : " + key.Key);
                Debug.Log("DATE : " + key.Date);
                Debug.Log("HOUR : " + key.Hour);
            }

            mTime = 0.0F;
            mSongLength = 0.0F;

            Toaster.Display<ParameterToast>().With(mLayout);
            
        }

        private void PlayReminder()
        {

            url = url.Replace("[reminder]", KeyUser);

            Debug.Log("Coucou coucou : " + KeyUser);
            WWW audioLoader = new WWW(url);

            StartCoroutine(GetMusic(audioLoader));
        }

        private void GetUserString(string User, object Obj, int idx)
        {
            Debug.Log("Name " + User + " NNAME " + NewUser + " KeyUser " + KeyUser + " Obj " + Obj.ToString());
            NewUser = User;
            KeyUser = "" + Obj;
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            Debug.Log("Aloooooor" + KeyUser);
            if (!aud.isPlaying)
            {
                Debug.Log("playing");
                aud.Play();
            }
            if (aud.isPlaying)
            {
                mSongLength = aud.clip.length;
                mTime += Time.deltaTime;
                mDiff = mTime / mSongLength;
                if (mDiff > 1)
                {
                    aud.Stop();
                    aud.clip = null;
                }
            }
        }

        IEnumerator GetMusic(WWW iWWW)
        {
            //while (!audioLoader.isDone)
            //{
            //    Debug.Log("uploading");
            //}

            //Debug.Log("1");

            //aud.clip = audioLoader.GetAudioClip(false, false, AudioType.WAV);

            yield return iWWW;
            if (iWWW.error == null && iWWW != null)
            {
                aud.clip = iWWW.GetAudioClip(false, true, AudioType.WAV);
                Debug.Log("coucou hiboux");
            }
            else
            Debug.Log("WWW ERROR : " + iWWW.error);
        }
    }
}