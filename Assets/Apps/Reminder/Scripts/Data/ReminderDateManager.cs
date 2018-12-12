using BlueQuark;

using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Reminder
{
    public sealed class ReminderDateManager
    {
        public enum E_REMINDER_STATE
        {
            E_DATE_CHOICE = 0,
            E_HOUR_CHOICE = 1,
            E_MESSAGE_CHOICE = 2
        }
        
        public const string STR_BYE = "bye";
        public const string STR_QUIT = "quit";
        public const string STR_SORRY = "srynotunderstand";
        public const string STR_WHEN = "when";
        public const string STR_WHOURS = "whours";
        public const string STR_STEPS = "steps";
        public const string STR_SETUPDATE = "setupdate";
        public const string STR_REMINDER_OK = "reminderok";

        public E_REMINDER_STATE AppState { get; set; }
        public DateTime ReminderDate { get; set; }
        public string ReminderMsg { get; set; }

        // Singleton design pattern
        private static readonly ReminderDateManager mInstance = new ReminderDateManager();

        // Singleton design pattern
        static ReminderDateManager()
        {
        }

        // Singleton design pattern
        private ReminderDateManager()
        {
        }

        // Singleton design pattern
        public static ReminderDateManager GetInstance()
        {
            return mInstance;
        }

        public void Initialize()
        {
            AppState = 0;
            ReminderDate = DateTime.Today;
            ReminderMsg = null;
        }
    }
}