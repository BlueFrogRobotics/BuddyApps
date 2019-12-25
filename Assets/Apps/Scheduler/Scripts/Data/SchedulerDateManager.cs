using BlueQuark;

using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Scheduler
{
    public sealed class SchedulerDateManager
	{
        public enum E_SCHEDULER_STATE
        {
            E_DATE_CHOICE = 0,
            E_HOUR_CHOICE = 1,
            //E_RECURRENCE_CHOICE = 2,
            E_MESSAGE_CHOICE = 2
        }
        
        public const string STR_BYE = "bye";
        public const string STR_QUIT = "quit";
        public const string STR_SORRY = "srynotunderstand";
        public const string STR_WHEN = "when";
        public const string STR_WHOURS = "whours";
        public const string STR_RECC = "howoften";
        public const string STR_STEPS = "steps";
        public const string STR_SETUPDATE = "setupdate";
        public const string STR_TASK_OK = "taskok";

        public SpeechInput CompanionInput { get; set; }
        public E_SCHEDULER_STATE AppState { get; set; }
        public DateTime SchedulerDate { get; set; }
        public string SchedulerMsg { get; set; }
		public string SchedulerRule { get; set; }
		public RepetitionTime RepetitionTime = RepetitionTime.ONCE;
        public List<DayOfWeek> RepetitionDays = new List<DayOfWeek>();

        // Singleton design pattern
        private static readonly SchedulerDateManager mInstance = new SchedulerDateManager();

        // Singleton design pattern
        static SchedulerDateManager()
        {
        }


        // Singleton design pattern
        public static SchedulerDateManager GetInstance()
        {
            return mInstance;
        }

        public void Initialize()
        {
            AppState = 0;
            SchedulerDate = DateTime.Today;
            SchedulerMsg = null;
			SchedulerRule = null;
			CompanionInput = null;
        }
    }
}