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

        private List<GameObject> mRoots;

		private WindowType mCurrentWindow;

		/*
         * Init refs to API and your app data
         */
		void Start()
        {

			mRoots = new List<GameObject>() { vocalRoot,
				ledRoot, motorsRoot, faceRoot,
				camerasRoot, thermalRoot, sensorsRoot, connexionRoot
            };

			

			mCurrentWindow = WindowType.FACE;
            //SetWindow((int)WindowType.VOCAL);
            SetWindow((int)WindowType.MOTORS);
        }

		public void SetWindow(int iIndex)
		{
			WindowType lType = (WindowType)iIndex;
			if (mCurrentWindow != lType) {
				mCurrentWindow = lType;
				switch (mCurrentWindow) {
					case WindowType.VOCAL:
						DisableAllExcept(vocalRoot);
						break;
					case WindowType.LED:
						DisableAllExcept(ledRoot);
						break;
					case WindowType.MOTORS:
						DisableAllExcept(motorsRoot);
						break;
					case WindowType.FACE:
						DisableAllExcept(faceRoot);
						break;
					case WindowType.CAMERAS:
						DisableAllExcept(camerasRoot);
						break;
					case WindowType.THERMAL:
						DisableAllExcept(thermalRoot);
						break;
					case WindowType.SENSORS:
						DisableAllExcept(sensorsRoot);
						break;
                    case WindowType.CONNEXION:
                        DisableAllExcept(connexionRoot);
                        break;
                }
			}
		}

		private void DisableAllExcept(GameObject iGOToKeep)
		{
			foreach (GameObject lRoot in mRoots)
				lRoot.SetActive(lRoot == iGOToKeep);
		}

		public void Quit()
		{
			//BYOS.Instance.AppManager.Quit();
			//new Buddy.Command.HomeCmd().Execute();
		}
	}
}