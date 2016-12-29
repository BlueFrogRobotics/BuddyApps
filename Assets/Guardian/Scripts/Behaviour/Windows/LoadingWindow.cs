using UnityEngine;
using UnityEngine.UI;
using BuddyOS;
using System.Collections;

namespace BuddyApp.Guardian
{
    public class LoadingWindow : MonoBehaviour
    {
        [SerializeField]
        private Text title;

        [SerializeField]
        private Text loadingText;

        private Dictionary mDictionary;

        // Use this for initialization
        void Start()
        {
            mDictionary = BYOS.Instance.Dictionary;
            title.text = mDictionary.GetString("guardian").ToUpper();
            loadingText.text = mDictionary.GetString("loading");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}