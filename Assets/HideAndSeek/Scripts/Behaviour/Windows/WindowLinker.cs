using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BuddyOS;

namespace BuddyApp.HideAndSeek
{
    public class WindowLinker : MonoBehaviour
    {
        private Dictionary mDictionnary;
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
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}