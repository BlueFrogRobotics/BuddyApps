using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.BuddyLab
{
    public sealed class SpecialItemManager : MonoBehaviour
    {
        private string mNameSItem;
        public string NameSItem { get { return mNameSItem; } set { mNameSItem = value; } }

        private bool mIsDone;

        void Start()
        {
            mIsDone = false;
        }

        void Update()
        {
            //LoadSpecialItem();
            //if(mNameSItem == "GOTO" && !mIsDone)
            //{
            //    Debug.Log("GOTO MAGGLE");
            //    mIsDone = true;
            //}
        }

        private void LoadSpecialItem()
        {

        }
    }
}
