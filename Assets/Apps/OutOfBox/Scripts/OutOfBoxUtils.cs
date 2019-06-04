using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using Random = System.Random;

namespace BuddyApp.OutOfBox
{
    public class OutOfBoxUtils
    {
        public static Random mRandom;
        private static string[] mBML;

        public static void Init()
        {
            mBML = new string[]
            {
                "CenterHead01",
                "CenterHeart01",
                "BlinkDouble01",
                "BlinkDouble02",
                "Suspicious01",
                "Suspicious02",
                "Surprised01",
                "Whistle01",
                "Whistle02",
                "Doubtful01",
                "Doubtful02",
            };
            mRandom = new Random();
        }

        public static IEnumerator WaitTimeAsync(float iWaitingTime, Action iOnEndWaiting)
        {
            yield return new WaitForSeconds(iWaitingTime);
            iOnEndWaiting?.Invoke();
        }

        public static IEnumerator PlayBIAsync(Action iOnEnd = null, string iBML = null, bool iMood = false)
        {
            bool lBMLIsEnding = false;

            if (string.IsNullOrEmpty(iBML))
            {
                int lIndex = mRandom.Next(mBML.Length);
                iBML = mBML[lIndex];
            }

            yield return new WaitForSeconds(0.100F);

            Debug.LogError("--- RUN " + iBML);
            Buddy.Behaviour.Interpreter.Run(iBML, () =>
            {
                Debug.LogError("--- ON END RUN " + iBML);
                lBMLIsEnding = true;
            });

            yield return new WaitUntil(() => { return lBMLIsEnding; });


            yield return new WaitForSeconds(0.200F);

            iOnEnd?.Invoke();
        }

        public static void DebugColor(string msg, string color = null)
        {
            if (string.IsNullOrEmpty(color))
                Debug.Log(msg);
            else
                Debug.Log("<color=" + color + ">----" + msg + "----</color>");
        }
    }
}

