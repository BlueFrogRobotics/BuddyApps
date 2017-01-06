﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BuddyOS;
using BuddyOS.Command;

namespace BuddyApp.HideAndSeek
{
    public class WindowLinker : MonoBehaviour
    {
        [SerializeField]
        private GameObject ObjAppOverBuddy;

        [SerializeField]
        private GameObject ObjAppOverBuddyWhite;

        private Dictionary mDictionnary;

        private bool mWillQuit = false;

        //private List<AWindow> mWindows;

        // Use this for initialization
        void Start()
        {
            mDictionnary = BYOS.Instance.Dictionary;

            AWindow[] lWindows = GetComponentsInChildren<AWindow>();
            foreach(AWindow lWindow in lWindows)
            {
                lWindow.Dictionary = mDictionnary;
                lWindow.Init();
            }

            mWillQuit = false;
            SetAppBlack();
        }

        // Update is called once per frame
        void Update()
        {
            if (mWillQuit)
            {
                new HomeCmd().Execute();
                mWillQuit = false;
            }
        }

        public void SetAppBlack()
        {
            ObjAppOverBuddy.SetActive(true);
            ObjAppOverBuddyWhite.SetActive(false);
        }

        public void SetAppWhite()
        {
            ObjAppOverBuddy.SetActive(false);
            ObjAppOverBuddyWhite.SetActive(true);
        }

        public void QuitApplication()
        {
            mWillQuit = true;
        }
    }
}