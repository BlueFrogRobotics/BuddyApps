using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using BuddyApp.Shared;

namespace BuddyApp.Scheduler
{
    public sealed class InitScheduler : AStateMachineBehaviour
    {
        private const int RECOGNITION_SENSIBILITY = 5000;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            // ReminderDateManager was initialized in the OnLoading function.
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

            // Define reminder date languages
            Dictionary<ISO6391Code, DateInterpreter> lDictionaryDate = new Dictionary<ISO6391Code, DateInterpreter>
                {
                    { ISO6391Code.EN, new DateInterpreterEnglish() },
                    { ISO6391Code.FR, new DateInterpreterFrench() },
                    { ISO6391Code.NL, new DateInterpreterDutch() }
                };

            // Define reminder hour languages
            Dictionary<ISO6391Code, HourInterpreter> lDictionaryHour = new Dictionary<ISO6391Code, HourInterpreter>
                {
                    { ISO6391Code.EN, new HourInterpreterEnglish() },
                    { ISO6391Code.FR, new HourInterpreterFrench() },
                    { ISO6391Code.NL, new HourInterpreterDutch() }
                };

            // Define reminder recurrence languages
            Dictionary<ISO6391Code, RecurrenceInterpreter> lDictionaryRecurrence = new Dictionary<ISO6391Code, RecurrenceInterpreter>
                {
                    { ISO6391Code.EN, new RecurrenceInterpreterEnglish() },
                    { ISO6391Code.FR, new RecurrenceInterpreterFrench() }
                };

			SchedulerLanguageManager.GetInstance().Initialize(lDictionaryDate, lDictionaryHour, lDictionaryRecurrence);
            
            Debug.Log("----- REMINDER WILL START -----");
            if (SchedulerDateManager.GetInstance().CompanionInput != null)
                Trigger("Start");   // Replace by "PreProcessing" when tag are available
            else
                Trigger("Start");
        }
    }
}
