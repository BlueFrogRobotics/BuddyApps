using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace BuddyApp.Shared
{
    static public class SharedVocalFunctions
    {

        public static bool ContainsOneOf(string iSpeech, List<string> iListSpeech)
        {
            for (int i = 0; i < iListSpeech.Count; ++i)
            {
                string[] words = iListSpeech[i].Split(' ');
                if (words.Length < 2)
                {
                    words = iSpeech.Split(' ');
                    foreach (string word in words)
                    {
                        if (word.ToLower() == iListSpeech[i].ToLower())
                        {
                            return true;
                        }
                    }
                }
                else if (iSpeech.ToLower().Contains(iListSpeech[i].ToLower()))
                    return true;
            }
            return false;
        }

        public static bool ContainsWhiteSpace(string iString)
        {
            for(int i = 0; i < iString.Length; ++i)
            {
                if (Char.IsWhiteSpace(iString[i]))
                    return true;
            }
            return false;
        }

        public static bool ContainsSpecialChar(string iString)
        {
           return iString.Any(item => !Char.IsLetterOrDigit(item) && !Char.IsWhiteSpace(item));
        }


        /// <summary>
        /// Change format of the StartRule (startrule#yes -> yes)
        /// </summary>
        /// <param name="iStartRuleVocon">Old format</param>
        /// <returns>New format</returns>
        public static string GetRealStartRule(string iStartRuleVocon)
        {
            if (!string.IsNullOrEmpty(iStartRuleVocon) && iStartRuleVocon.Contains("#"))
            {
                string lStartRule = iStartRuleVocon.Substring(iStartRuleVocon.IndexOf("#") + 1);
                return (lStartRule);
            }
            return (string.Empty);
        }
    }
}

