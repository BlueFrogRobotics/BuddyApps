using UnityEngine;
using System.Collections;

namespace BuddyApp.Guardian
{
    public class CloseWindowToAnimator : MonoBehaviour
    {

        public GameObject mWindow;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void CloseWindow()
        {
            mWindow.SetActive(false);
        }
    }
}