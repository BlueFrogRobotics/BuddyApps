using UnityEngine;

namespace BuddyApp.Call
{
    public class WindowsManager : MonoBehaviour
    {
        [SerializeField]
        private Animator receiveCallAnim;

        [SerializeField]
        private Animator receiveCallTimeAnim;

        [SerializeField]
        private Animator backgroundAnim;

        [SerializeField]
        private Animator callAnimator;

        private bool mIncomingCallHandled;

        void Start()
        {
            receiveCallAnim.SetTrigger("Open_WReceiveCall");
            //receiveCallTimeAnim.SetTrigger("Close_WReceiveCallTime");
            //callAnimator.SetTrigger("Close_WCall");
            //backgroundAnim.SetTrigger("Open_BG");
            mIncomingCallHandled = false;
        }
        
        void Update()
        {

        }

        public void GetIncomingCall()
        {
            if (mIncomingCallHandled)
                return;

            mIncomingCallHandled = true;
            receiveCallAnim.SetTrigger("Open_WReceiveCall");
        }

        public void StopCall()
        {
            if (!mIncomingCallHandled)
                return;

            mIncomingCallHandled = false;
            callAnimator.SetTrigger("Close_WCall");
        }
    }
}