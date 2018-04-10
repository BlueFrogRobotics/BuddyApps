using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace BuddyApp.SandboxApp
{
    static public class VocalFunctions
    {

        //public static bool ContainsOneOf(string iSpeech, List<string> iListSpeech)
        //{
        //    for (int i = 0; i < iListSpeech.Count; ++i)
        //    {
        //        string[] words = iListSpeech[i].Split(' ');
        //        if (words.Length < 2)
        //        {
        //            words = iSpeech.Split(' ');
        //            foreach (string word in words)
        //            {
        //                if (word == iListSpeech[i].ToLower())
        //                {
        //                    return true;
        //                }
        //            }
        //        }
        //        else if (iSpeech.ToLower().Contains(iListSpeech[i].ToLower()))
        //            return true;
        //    }
        //    return false;
        //}

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
                if (char.IsWhiteSpace(iString[i]))
                    return true;
            }
            return false;
        }

        public static bool ContainsSpecialChar(string iString)
        {
            for (int i = 0; i < iString.Length; ++i)
            {
                if (iString.Any(item => !Char.IsLetterOrDigit(iString[i]))) return true;
            }
            return false;
        }
    }
}

