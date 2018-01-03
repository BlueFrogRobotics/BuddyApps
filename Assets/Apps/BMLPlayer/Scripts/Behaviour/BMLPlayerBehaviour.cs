using Buddy;
using Buddy.UI;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.BMLPlayer
{
    public class BMLPlayerBehaviour : MonoBehaviour
    {
        [SerializeField]
        private InputField bmlName;

        [SerializeField]
        private UnityEngine.UI.Button launchButton;
        
        private TextToSpeech mTTS;
        private BMLManager mBMLManager;

        void Start()
        {
            mTTS = BYOS.Instance.Interaction.TextToSpeech;
            mBMLManager = BYOS.Instance.Interaction.BMLManager;
            //mBMLManager.LoadAppBML();
        }

        public void Callback(string iInput)
        {
            if (string.IsNullOrEmpty(iInput)) {
                mTTS.Say("Veuillez donner un nom de BML � ex�cuter s'il-vous-pla�t.");
                return;
            }

            if (!mBMLManager.LaunchByName(iInput))
                mTTS.Say("Je n'ai pas su trouv� le BML demand�.");
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
    }
}