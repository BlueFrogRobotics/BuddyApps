using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS;

namespace BuddyApp.Guardian
{
    public class WindowMenu : MonoBehaviour
    {
        [SerializeField]
        private Text mTextFix;

        [SerializeField]
        private Text mTextMobile;

        [SerializeField]
        private Text mTextQuit;

        private Dictionary mDictionnary;

        // Use this for initialization
        void Start()
        {
            mDictionnary = BYOS.Instance.Dictionary;
            mTextFix.text = mDictionnary.GetString("fixMonitoring");
            mTextMobile.text = mDictionnary.GetString("mobileMonitoring");
            mTextQuit.text = mDictionnary.GetString("quit");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}