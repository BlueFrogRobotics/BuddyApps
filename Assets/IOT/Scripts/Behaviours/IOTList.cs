using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.IOT
{
    public class IOTList : MonoBehaviour
    {
        [SerializeField]
        private Transform content;

        [SerializeField]
        private IOTLinkManager IOTLinkIA;

        private List<IOTObjects> mObjects = new List<IOTObjects>();
        public List<IOTObjects> Objects { get { return mObjects; } }

        private void PopulateButtonClick()
        {
            for(int i = 0; i < content.childCount-1; ++i)
                content.GetChild(i).GetChild(1).GetComponent<Button>().onClick.AddListener(() => IOTLinkIA.setTriggerChoice(i));
        }

        private void DestroyObjects()
        {
            for (int i = 0; i < content.childCount - 1; ++i)
                GameObject.Destroy(content.GetChild(i).gameObject);
        }

        void OnEnable()
        {
            DestroyObjects();
            for (int i = 0; i < mObjects.Count; ++i)
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
                lButton.transform.SetParent(content, false);
                lButton.transform.SetSiblingIndex(content.childCount - 2);
            }
            PopulateButtonClick();
        }
    }
}