using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.SandboxApp
{
    static public class VocalFunctions
    {

        static public bool ContainsOneOf(string iSpeech, List<string> iListSpeech)
        {
            for (int i = 0; i < iListSpeech.Count; ++i)
            {
                string[] words = iListSpeech[i].Split(' ');
                if (words.Length < 2)
                {
                    words = iSpeech.Split(' ');
                    foreach (string word in words)
                    {
                        if (word == iListSpeech[i].ToLower())
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

        static public bool ContainsWhiteSpace(string iString)
        {
            for(int i = 0; i < iString.Length; ++i)
            {
                if (char.IsWhiteSpace(iString[i]))
                    return true;
            }
            return false;
        }
    }
}

