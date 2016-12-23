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
            {
                int blah = i;
                content.GetChild(i).GetChild(1).GetComponent<Button>().onClick.AddListener(() => { IOTLinkIA.setTriggerChoice(blah+1); });
            }
        }

        private void DestroyObjects()
        {
            for (int i = 0; i < content.childCount - 1; ++i)
                GameObject.Destroy(content.GetChild(i).gameObject);
        }
        void OnDisable()
        {
            DestroyObjects();
        }

        void OnEnable()
        {
            for (int i = 0; i < mObjects.Count; ++i)
            {
                GameObject lButton;
                BuddyOS.SpriteManager lSpriteManager = BuddyOS.BYOS.Instance.SpriteManager;
                if (mObjects[i] is IOTSystems)
                {
                    lButton = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/System_Button"));
                    lButton.transform.GetChild(1).GetChild(1).GetComponent<Image>().sprite = lSpriteManager.GetSprite(mObjects[i].SpriteName, "AtlasIOT");
                    lButton.GetComponent<IOTObjectContainer>().Object = mObjects[i];
                    lButton.transform.GetChild(1).GetChild(2).GetComponent<Text>().text = mObjects[i].Name;

                }else
                {
                    lButton = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Device_Button"));
                    lButton.GetComponent<IOTObjectContainer>().Object = mObjects[i];
                    lButton.transform.GetChild(1).GetChild(2).GetComponent<Text>().text = mObjects[i].Name;
                }
                lButton.transform.SetParent(content, false);
                lButton.transform.SetSiblingIndex(content.childCount - 2);
            }
            PopulateButtonClick();
        }
    }
}