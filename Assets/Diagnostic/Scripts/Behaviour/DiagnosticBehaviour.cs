﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BuddyOS;

namespace BuddyApp.Diagnostic
{
    public enum WindowType : int
    {
        VOCAL,
        LED,
        MOTORS,
        FACE,
        CAMERAS,
        THERMAL,
        SENSORS
    }

    public class DiagnosticBehaviour : MonoBehaviour
    {
        [SerializeField]
        private GameObject vocalRoot;

        [SerializeField]
        private GameObject ledRoot;

        [SerializeField]
        private GameObject motorsRoot;

        [SerializeField]
        private GameObject faceRoot;

        [SerializeField]
        private GameObject camerasRoot;

        [SerializeField]
        private GameObject thermalRoot;

        [SerializeField]
        private GameObject sensorsRoot;

        private List<GameObject> mRoots;

        private WindowType mCurrentWindow;

        // Use this for initialization
        void Start()
        {            
            mRoots = new List<GameObject>() { vocalRoot,
                ledRoot, motorsRoot, faceRoot,
                camerasRoot, thermalRoot, sensorsRoot
            };

            mCurrentWindow = WindowType.FACE;
            SetWindow((int)WindowType.VOCAL);
        }

        public void SetWindow(int iIndex)
        {
            WindowType lType = (WindowType)iIndex;
            if (mCurrentWindow != lType) {
                mCurrentWindow = lType;
                switch (mCurrentWindow) {
                    case WindowType.VOCAL:
                        DisableAllExcept(vocalRoot);
                        break;
                    case WindowType.LED:
                        DisableAllExcept(ledRoot);
                        break;
                    case WindowType.MOTORS:
                        DisableAllExcept(motorsRoot);
                        break;
                    case WindowType.FACE:
                        DisableAllExcept(faceRoot);
                        break;
                    case WindowType.CAMERAS:
                        DisableAllExcept(camerasRoot);
                        break;
                    case WindowType.THERMAL:
                        DisableAllExcept(thermalRoot);
                        break;
                    case WindowType.SENSORS:
                        DisableAllExcept(sensorsRoot);
                        break;
                }
            }
        }

        private void DisableAllExcept(GameObject iGOToKeep)
        {
            foreach (GameObject lRoot in mRoots)
                lRoot.SetActive(lRoot == iGOToKeep);
        }
    }
}