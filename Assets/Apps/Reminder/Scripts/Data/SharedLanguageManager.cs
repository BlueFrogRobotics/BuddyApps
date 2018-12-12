using BlueQuark;

using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Reminder
{
    public sealed class SharedLanguageManager<T> where T : new()
    {
        private T mCurrentLanguage = new T();
        Dictionary<ISO6391Code, T> mLanguages;
        
        // Singleton design pattern
        private static SharedLanguageManager<T> mInstance = new SharedLanguageManager<T>();

        // Singleton design pattern
        static SharedLanguageManager()
        {
        }

        // Singleton design pattern
        private SharedLanguageManager()
        {
        }

        // Singleton design pattern
        public static SharedLanguageManager<T> GetInstance()
        {
            return mInstance;
        }
        
        public void Initialize (Dictionary<ISO6391Code, T> iDictionary)
        {
            mLanguages = iDictionary;
            SetLanguage(Buddy.Platform.Language.SystemInputLanguage.ISO6391Code);
        }

        public T GetLanguage()
        {
            return mCurrentLanguage;
        }

        public bool SetLanguage(ISO6391Code iInput)
        {

            if (mLanguages.ContainsKey(Buddy.Platform.Language.SystemInputLanguage.ISO6391Code))
            {
                mCurrentLanguage = mLanguages[Buddy.Platform.Language.SystemInputLanguage.ISO6391Code];
                return true;
            }

            Debug.LogWarning("Language Manager - cannot find required language.");
            SetDefaultLanguage();

            return false;
        }

        private void SetDefaultLanguage()
        {
            mCurrentLanguage = new T();
        }
    }
}