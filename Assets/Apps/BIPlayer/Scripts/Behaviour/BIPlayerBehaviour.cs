using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.BIPlayer
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class BIPlayerBehaviour : MonoBehaviour
	{
		[SerializeField]
		private InputField bmlName;

		[SerializeField]
		private UnityEngine.UI.Button launchButton;



		public void Callback(string iInput)
		{
			if (string.IsNullOrEmpty(iInput)) {
				Buddy.Vocal.Say("Veuillez donner un nom de BML à exécuter s'il-vous-plaît.");
				return;
			}

			if (!Buddy.Behaviour.Interpreter.Run("BML/" + iInput))
				Buddy.Vocal.Say("Je n'ai pas  su trouvé le BML demandé.");
		}

		public void LaunchBML()
		{
			Callback(bmlName.text);
		}

		public void ToggleUI()
		{
			bmlName.gameObject.SetActive(!bmlName.isActiveAndEnabled);
			launchButton.gameObject.SetActive(!launchButton.isActiveAndEnabled);
		}
	
	/*
	 * Data of the application. Save on disc when app is quitted
	 */
	private BIPlayerData mAppData;

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			BIPlayerActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = BIPlayerData.Instance;
        }
    }
}