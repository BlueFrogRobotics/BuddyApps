using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS;

namespace BuddyApp.Guardian
{
    //[RequireComponent(typeof(CanvasGroup))]
    public class NotifMail : MonoBehaviour
    {

        int mCounter = 0;
        public Text[] mTexts;
        public GameObject[] mNotifObjects;
        bool mMustIncrement = false;

        // Use this for initialization
        void Start()
        {
            for(int i=0; i<mTexts.Length; i++)
                mTexts[i].text = "0";
            for (int i = 0; i < mNotifObjects.Length; i++)
                mNotifObjects[i].SetActive(false);
            //GetComponent<CanvasGroup>().alpha = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if (mMustIncrement)
            {
                //GetComponent<CanvasGroup>().alpha = 1;
                mCounter++;
                for (int i = 0; i < mTexts.Length; i++)
                    mTexts[i].text = "" + mCounter;
                for (int i = 0; i < mNotifObjects.Length; i++)
                    mNotifObjects[i].SetActive(true);
                //mText.text = "" + mCounter;
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
            for (int i = 0; i < mTexts.Length; i++)
                mTexts[i].text = "0";
            for (int i = 0; i < mNotifObjects.Length; i++)
                mNotifObjects[i].SetActive(false);
            //mText.text = "0";
            //GetComponent<CanvasGroup>().alpha = 0;
        }
    }
}