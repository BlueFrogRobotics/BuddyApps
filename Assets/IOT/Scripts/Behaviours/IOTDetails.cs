using UnityEngine;
using UnityEngine.UI;
using BuddyOS;

namespace BuddyApp.IOT {
    public class IOTDetails : MonoBehaviour {
        [SerializeField]
        private Transform content;

        [SerializeField]
        private ParametersGameObjectContainer param;

        private IOTObjects mObject;
        public IOTObjects Object { get { return mObject; } set { mObject = value; } }

        private SpriteManager mSpriteManager;

        private float mTime = 0f;

        private Transform ParamContainer(GameObject iGO)
        {
            return iGO.transform.GetChild(3);
        }

        private void SetSystemLogo(Transform iDevicePanel, IOTSystems iObject)
        {
            Transform lLogo = iDevicePanel.transform.GetChild(0);
            lLogo.gameObject.SetActive(true);
            lLogo.GetChild(1).GetChild(2).GetComponent<Text>().text = iObject.Name;
            lLogo.GetChild(1).GetChild(1).GetComponent<Image>().sprite = mSpriteManager.GetSprite(iObject.SpriteName, "AtlasIOT");

        }

        private void SetDeviceLogo(Transform iDevicePanel, IOTDevices iObject)
        {
            Transform lLogo = iDevicePanel.transform.GetChild(1);
            lLogo.gameObject.SetActive(true);
            lLogo.GetChild(1).GetChild(2).GetComponent<Text>().text = iObject.Name;
            lLogo.GetChild(1).GetChild(1).GetComponent<Image>().sprite  = mSpriteManager.GetSprite(iObject.SpriteName, "AtlasIOT");
        }

        private void CleanParams()
        {
            for (int i = 0; i < content.childCount; ++i)
                GameObject.Destroy(content.GetChild(i).gameObject);
        }

        void OnEnable()
        {
            mSpriteManager = BYOS.Instance.SpriteManager;
            mTime = 0f;

            CleanParams();
            GameObject lFirst = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/DevicePanel"));
            lFirst.transform.SetParent(content, false);
            lFirst.transform.GetChild(0).gameObject.SetActive(true);
            SetSystemLogo(lFirst.transform, (IOTSystems)mObject);

            Transform lParams = ParamContainer(lFirst);
            mObject.InitializeParams();
            mObject.PlaceParams(lParams);

            transform.GetChild(1).GetComponent<Text>().text = "YOU HAVE OPENED " + mObject.Name.ToUpper() + ", WHAT DO YOU WANT TO DO?";

            if (mObject is IOTSystems)
            {
                for (int i = 0; i < ((IOTSystems)mObject).Devices.Count; i++)
                {
                    IOTDevices lDevice = ((IOTSystems)mObject).Devices[i];
                    GameObject lPanel = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/DevicePanel"));
                    lPanel.transform.SetParent(content, false);
                    lPanel.transform.GetChild(1).gameObject.SetActive(true);
                    SetDeviceLogo(lPanel.transform, (IOTDevices)lDevice);

                    lDevice.ParamGO = mObject.ParamGO;
                    Transform lParamsChild = ParamContainer(lPanel);
                    lDevice.InitializeParams();
                    lDevice.PlaceParams(lParamsChild);
                }
            }
        }

        void Update()
        {
            mTime += Time.deltaTime;
            if(mTime > 1F)
            {
                if (mObject is IOTSystems)
                {
                    IOTSystems mSystem = (IOTSystems)mObject;
                    for (int i = 0; i < mSystem.Devices.Count; ++i)
                    {
                        mSystem.Devices[i].UpdateSlow();
                    }
                }
                else if (mObject is IOTDevices)
                    ((IOTDevices)mObject).UpdateSlow();

                mTime = 0F;
            }
        }
    }
}
