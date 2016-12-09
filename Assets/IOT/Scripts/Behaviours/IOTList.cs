using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.IOT
{
    public class IOTList : MonoBehaviour
    {
        [SerializeField]
        private Transform content;

        private List<IOTObjects> mObjects = new List<IOTObjects>();
        public List<IOTObjects> Objects { get { return mObjects; } }

        void OnEnable()
        {
            for(int i = 0; i < mObjects.Count; ++i)
            {
                GameObject lButton;
                if (mObjects[i] is IOTSystems)
                {
                    lButton = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/System_Button"));
                    lButton.GetComponent<IOTObjectContainer>().Object = mObjects[i];

                }else
                {
                    lButton = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Device_Button"));
                    lButton.GetComponent<IOTObjectContainer>().Object = mObjects[i];
                }
                lButton.transform.SetParent(content);
                lButton.transform.SetSiblingIndex(content.childCount - 2);
            }
        }
    }
}