using UnityEngine.UI;

using UnityEngine;

using BlueQuark;
using System.Collections.Generic;

namespace BuddyApp.Diagnostic
{
	public enum WindowType : int
	{
		VOCAL,
		LED,
		MOTORS,
		FACE,
		CAMERAS,
		THERMAL,
		SENSORS,
        CONNEXION
    }

	/* A basic monobehaviour as "AI" behaviour for your app */
	public sealed class DiagnosticBehaviour : MonoBehaviour
    {

		[SerializeField]
		private GameObject vocalRoot;
		[SerializeField]
		private GameObject ledRoot;
		[SerializeField]
		private GameObject motorsRoot;
		[SerializeField]
		private GameObject faceRoot;
		[SerializeField]
		private GameObject camerasRoot;
		[SerializeField]
		private GameObject thermalRoot;
		[SerializeField]
		private GameObject sensorsRoot;
        [SerializeField]
        private GameObject connexionRoot;

       /*[SerializeField]
        private Button motorsBT;*/

        [SerializeField]
        private Image vocalBT;
        [SerializeField]
        private Image ledBT;
        [SerializeField]
        private Image motorsBT;
        [SerializeField]
        private Image faceBT;
        [SerializeField]
        private Image camerasBT;
        [SerializeField]
        private Image thermalBT;
        [SerializeField]
        private Image sensorsBT;
        [SerializeField]
        private Image connexionBT;

        private List<GameObject> mRoots;
        private List<Image> mBTs;

        private WindowType mCurrentWindow;
        private Color BuddyBlue = new Color(0.0f, 0.831f, 0.819f);
        private Color White = new Color(1f, 1f, 1f);
        /*
         * Init refs to API and your app data
         */
        void Start()
        {
			mRoots = new List<GameObject>() { vocalRoot,
				ledRoot, motorsRoot, faceRoot,
				camerasRoot, thermalRoot, sensorsRoot, connexionRoot
            };
            mBTs = new List<Image>() { vocalBT,
                ledBT, motorsBT, faceBT,
                camerasBT, thermalBT, sensorsBT, connexionBT
            };
            
            mCurrentWindow = WindowType.CAMERAS;
            Debug.Log("AVANT SET WINDOW");
            SetWindow(0);
            Debug.Log("APRES SET WINDOW");
        }

		public void SetWindow(int iIndex)
		{
			WindowType lType = (WindowType)iIndex;
			if (mCurrentWindow != lType) {
				mCurrentWindow = lType;
                Rect lrect_Vocal = vocalBT.GetComponent<RectTransform>().rect;
                lrect_Vocal.height = 100;
                Rect lrect_Led = ledBT.GetComponent<RectTransform>().rect;
                lrect_Led.height = 100;
                Rect lrect_Motors = motorsBT.GetComponent<RectTransform>().rect;
                lrect_Motors.height = 100;
                Rect lrect_Face = faceBT.GetComponent<RectTransform>().rect;
                lrect_Face.height = 100;
                Rect lrect_Cameras = camerasBT.GetComponent<RectTransform>().rect;
                lrect_Cameras.height = 100;
                Rect lrect_Thermal = thermalBT.GetComponent<RectTransform>().rect;
                lrect_Thermal.height = 100;
                Rect lrect_Sensors = sensorsBT.GetComponent<RectTransform>().rect;
                lrect_Sensors.height = 100;
                Rect lrect_Connexion = connexionBT.GetComponent<RectTransform>().rect;
                lrect_Connexion.height = 100;
                Debug.Log("Vocal rect Before: " + lrect_Vocal);
                switch (mCurrentWindow) {
					case WindowType.VOCAL:
						DisableAllExcept(vocalRoot);
                        vocalBT.color = BuddyBlue;
                        lrect_Vocal.height = 120;
                        Debug.Log("Vocal rect After: " + lrect_Vocal);
                        break;
					case WindowType.LED:
						DisableAllExcept(ledRoot);
                        ledBT.color = BuddyBlue;
                        lrect_Led.height = 120;
                        break;
					case WindowType.MOTORS:
						DisableAllExcept(motorsRoot);
                        motorsBT.color = BuddyBlue;
                        lrect_Motors.height = 120;
                        break;
					case WindowType.FACE:
						DisableAllExcept(faceRoot);
                        faceBT.color = BuddyBlue;
                        lrect_Face.height = 120;
                        break;
					case WindowType.CAMERAS:
						DisableAllExcept(camerasRoot);
                        camerasBT.color = BuddyBlue;
                        lrect_Cameras.height = 120;
                        break;
					case WindowType.THERMAL:
						DisableAllExcept(thermalRoot);
                        thermalBT.color = BuddyBlue;
                        lrect_Thermal.height = 120;
                        break;
					case WindowType.SENSORS:
						DisableAllExcept(sensorsRoot);
                        sensorsBT.color = BuddyBlue;
                        lrect_Sensors.height = 120;
                        break;
                    case WindowType.CONNEXION:
                        DisableAllExcept(connexionRoot);
                        connexionBT.color = BuddyBlue;
                        lrect_Connexion.height = 120;
                        break;
                }
			}
		}

		private void DisableAllExcept(GameObject iGOToKeep)
		{
			foreach (GameObject lRoot in mRoots)
				lRoot.SetActive(lRoot == iGOToKeep);
            foreach (Image lBT in mBTs)
                lBT.color = White;
        }

		public void Quit()
		{
			//BYOS.Instance.AppManager.Quit();
			//new Buddy.Command.HomeCmd().Execute();
		}
	}
}