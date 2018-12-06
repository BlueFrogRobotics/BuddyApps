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

            mCurrentWindow = WindowType.FACE;
            //SetWindow((int)WindowType.VOCAL);
            SetWindow((int)WindowType.MOTORS);
            motorsBT.color = BuddyBlue;         
        }

		public void SetWindow(int iIndex)
		{
			WindowType lType = (WindowType)iIndex;
			if (mCurrentWindow != lType) {
				mCurrentWindow = lType;
				switch (mCurrentWindow) {
					case WindowType.VOCAL:
						DisableAllExcept(vocalRoot);
                        vocalBT.color = BuddyBlue;
						break;
					case WindowType.LED:
						DisableAllExcept(ledRoot);
                        ledBT.color = BuddyBlue;
                        break;
					case WindowType.MOTORS:
						DisableAllExcept(motorsRoot);
                        motorsBT.color = BuddyBlue;
                        break;
					case WindowType.FACE:
						DisableAllExcept(faceRoot);
                        faceBT.color = BuddyBlue;
                        break;
					case WindowType.CAMERAS:
						DisableAllExcept(camerasRoot);
                        camerasBT.color = BuddyBlue;
                        break;
					case WindowType.THERMAL:
						DisableAllExcept(thermalRoot);
                        thermalBT.color = BuddyBlue;
                        break;
					case WindowType.SENSORS:
						DisableAllExcept(sensorsRoot);
                        sensorsBT.color = BuddyBlue;
                        break;
                    case WindowType.CONNEXION:
                        DisableAllExcept(connexionRoot);
                        connexionBT.color = BuddyBlue;
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