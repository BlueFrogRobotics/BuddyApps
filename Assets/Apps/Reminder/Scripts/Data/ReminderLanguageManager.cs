using BlueQuark;

using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using BuddyApp.Shared;

namespace BuddyApp.Reminder
{
    public sealed class ReminderLanguageManager
    {
        SharedLanguageManager<DateInterpreter> mDateLanguage;
        SharedLanguageManager<HourInterpreter> mHourLanguage;
        SharedLanguageManager<RecurrenceInterpreter> mRecurrenceLanguage;

        // Singleton design pattern
        private static readonly ReminderLanguageManager mInstance = new ReminderLanguageManager();

        // Singleton design pattern
        static ReminderLanguageManager()
        {
        }

        // Singleton design pattern
        private ReminderLanguageManager()
        {
        }

        // Singleton design pattern
        public static ReminderLanguageManager GetInstance()
        {
            return mInstance;
        }
        
        public void Initialize (Dictionary<ISO6391Code, DateInterpreter> iDictionaryDate,
                                Dictionary<ISO6391Code, HourInterpreter> iDictionaryHour,
                                Dictionary<ISO6391Code, RecurrenceInterpreter> iDictionaryRecurrence)
        {
            mDateLanguage = new SharedLanguageManager<DateInterpreter>(iDictionaryDate);
            mHourLanguage = new SharedLanguageManager<HourInterpreter>(iDictionaryHour);
            mRecurrenceLanguage = new SharedLanguageManager<RecurrenceInterpreter>(iDictionaryRecurrence);
        }

        public DateInterpreter GetDateLanguage()
        {
            return mDateLanguage.GetLanguage();
        }

        public HourInterpreter GetHourLanguage()
        {
            return mHourLanguage.GetLanguage();
        }

        public RecurrenceInterpreter GetRecurrenceLanguage()
        {
            return mRecurrenceLanguage.GetLanguage();
        }

        public bool SetLanguage(ISO6391Code iInput)
        {
            return (mDateLanguage.SetLanguage(iInput)
                    & mHourLanguage.SetLanguage(iInput)
                    & mRecurrenceLanguage.SetLanguage(iInput));
        }

        public bool SetDateLanguage(ISO6391Code iInput)
        {
            return mDateLanguage.SetLanguage(iInput);
        }

        public bool SetHourLanguage(ISO6391Code iInput)
        {
            return mHourLanguage.SetLanguage(iInput);
        }

        public bool SetRecurrenceLanguage(ISO6391Code iInput)
        {
            return mRecurrenceLanguage.SetLanguage(iInput);
        }
    }
}