using UnityEngine;
using System.Collections;

namespace BuddyApp.IOT {
    public class IOTDetails : MonoBehaviour {
        [SerializeField]
        private Transform content;

        [SerializeField]
        private ParametersGameObjectContainer param;

        private IOTObjects mObject;
        public IOTObjects Object { get { return mObject; } set { mObject = value; } }

        private Transform ParamContainer(GameObject iGO)
        {
            return iGO.transform.GetChild(3);
        }

        void OnEnable()
        {
            //FOR TEST
            mObject = new IOTPhilipsHue();
            for (int i = 0; i < param.ParametersList.Count; ++i)
                mObject.ListParam.Add(param.ParametersList[i]);

            GameObject lFirst = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/DevicePanel"));
            lFirst.transform.SetParent(content, false);

            Transform lParams = ParamContainer(lFirst);
            //mObject.InitializeParams();
            //mObject.PlaceParams(lParams);

            if (mObject is IOTSystems)
            {
                for (int i = 0; i < ((IOTSystems)mObject).Devices.Count; i++)
                {
                    IOTDevices lDevice = ((IOTSystems)mObject).Devices[i];
                    GameObject lPanel = Resources.Load<GameObject>("Prefabs/DevicePanel");
                    lFirst.transform.SetParent(content, false);

                    Transform lParamsChild = ParamContainer(lPanel);
                    lDevice.InitializeParams();
                    lDevice.PlaceParams(lParamsChild);
                }
            }
        }
    }
}
