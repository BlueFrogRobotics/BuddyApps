using UnityEngine;
using System.Collections;

namespace BuddyApp.Call
{
    public class HideUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject feedBack;
        [SerializeField]
        private GameObject callerVideo;
        [SerializeField]
        private GameObject buddyFace;

        private bool mUIEnabled;

        void Start()
        {
            mUIEnabled = true;
        }
        
        void Update()
        {

        }

        public void SwitchUI()
        {
            if(mUIEnabled) {
                callerVideo.SetActive(false);
                feedBack.SetActive(false);
                buddyFace.SetActive(false);
                mUIEnabled = false;
            } else {
                callerVideo.SetActive(true);
                feedBack.SetActive(true);
                //buddyFace.SetActive(false);
                mUIEnabled = true;
            }
        }
    }
}