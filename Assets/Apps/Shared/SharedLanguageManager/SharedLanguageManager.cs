using BlueQuark;

using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Shared
{
    public sealed class SharedLanguageManager<T> where T : new()
    {
        private T mCurrentLanguage = new T();
        Dictionary<ISO6391Code, T> mLanguages;
        
        public SharedLanguageManager (Dictionary<ISO6391Code, T> iDictionary)
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