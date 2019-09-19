using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using Random = System.Random;

namespace BuddyApp.OutOfBoxV3
{
    public class OutOfBoxUtilsVThree
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
            Debug.LogWarning("Waititme ");
            yield return new WaitForSeconds(iWaitingTime);
            if (iOnEndWaiting != null)
                iOnEndWaiting();
            //iOnEndWaiting?.Invoke();
        }

        public static IEnumerator PlayBIAsync(Action iOnEnd = null, string iBML = null, bool iMood = false)
        {
            DebugColor("Play BI!!!!!!!", "red");
            //Debug.LogWarning("Play BI!!!!!!!");
            bool lBMLIsEnding = false;

            DebugColor("--- PlayBIAsync " , "red");

            if (string.IsNullOrEmpty(iBML))
            {
                int lIndex = mRandom.Next(mBML.Length);
                iBML = mBML[lIndex];
            }
            yield return new WaitForSeconds(0.100F);

            if(!string.IsNullOrEmpty(iBML))
            {
                Buddy.Behaviour.Interpreter.Run(iBML, () =>
                {
                    DebugColor("--- ON END RUN " + iBML, "red");
                    lBMLIsEnding = true;
                });
            }


            yield return new WaitUntil(() => { return lBMLIsEnding; });


            yield return new WaitForSeconds(0.200F);


            if (iOnEnd != null) 
                iOnEnd(); 
            //iOnEnd?.Invoke();
        }

        public static float WrapAngle(double iAngle)
        {
            iAngle = (double)Math.IEEERemainder((double)iAngle, 360);
            if (iAngle <= -180)
            {
                iAngle += 360;
            }
            else if (iAngle > 180)
            {
                iAngle -= 360;
            }

            return (float)iAngle;
        }

        public static void DebugColor(string msg, string color = null)
        {
            if (string.IsNullOrEmpty(color))
                Debug.LogWarning(msg);
            else
                Debug.LogWarning("<color=" + color + ">----" + msg + "----</color>");
        }
    }
}

