using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Reminder
{
    public class InitReminder : AStateMachineBehaviour
    {
        private const int RECOGNITION_SENSIBILITY = 5000;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("----- INIT REMINDER DATA -----");
            ReminderDateManager.GetInstance().Initialize();
            
            // Setting of Header
            Buddy.GUI.Header.DisplayParametersButton(false);
            Font lHeaderFont = Buddy.Resources.Get<Font>("os_awesome");
            lHeaderFont.material.color = new Color(0, 0, 0);
            Buddy.GUI.Header.SetCustomLightTitle(lHeaderFont);

            // Setting of Vocon param
            Buddy.Vocal.DefaultInputParameters = new SpeechInputParameters()
            {
                RecognitionThreshold = RECOGNITION_SENSIBILITY
            };

            // Define reminder languages
            Dictionary<ISO6391Code, ReminderLanguage> lDictionary = new Dictionary<ISO6391Code, ReminderLanguage>
                {
                    { ISO6391Code.EN, new ReminderLanguageEnglish() },
                    { ISO6391Code.FR, new ReminderLanguageFrench() }
                };
            SharedLanguageManager<ReminderLanguage>.GetInstance().Initialize(lDictionary);
            
            Debug.Log("----- REMINDER WILL START -----");
            Trigger("StartReminder");
        }
    }
}
