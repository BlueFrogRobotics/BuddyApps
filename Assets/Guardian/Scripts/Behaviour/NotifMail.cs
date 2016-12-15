using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS;

namespace BuddyApp.Guardian
{
    public class NotifMail : MonoBehaviour
    {

        int mCounter = 0;
        public Text mText;
        bool mMustIncrement = false;

        // Use this for initialization
        void Start()
        {
            mText.text = "";
        }

        // Update is called once per frame
        void Update()
        {
            if (mMustIncrement)
            {
                mCounter++;
                mText.text = "" + mCounter;
                Debug.Log("incremente");
                mMustIncrement = false;
                BYOS.Instance.NotManager.Display<SimpleNot>().With("Mail envoyé", null);
            }
        }

        public void IncrementNumber()
        {
            mMustIncrement = true;
        }

        void Clear()
        {
            mCounter = 0;
            mText.text = "";
        }
    }
}