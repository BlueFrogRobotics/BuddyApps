using BlueQuark;

using System;
using System.Collections;
using Random = System.Random;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.HelloWorld
{
    public static class HelloWorldUtils
    {
        // Actuators Velocity
        public const float NO_VELOCITY = 70F;
        public const float YES_VELOCITY = 45F;
        public const float LINEAR_VELOCITY = 70F;
        public const float ANGULAR_VELOCITY = 70F;

        public static Random mRandom;
        private static Array mValue;
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
            mValue = Enum.GetValues(typeof(Mood));
        }

        // Temporary waiting for real BI
        public static IEnumerator PlayBIAsync(Action iOnEnd = null, string iBML = null, bool iMood = false)
        {
            // Just play random mood
            if (iMood) {
                Mood lMood = (Mood)mValue.GetValue(mRandom.Next(mValue.Length));
                if (lMood == Mood.NEUTRAL || lMood == Mood.HEARING || lMood == Mood.LISTENING || lMood == Mood.SURPRISED)
                    lMood = Mood.LOVE;
                Debug.LogError("--- MOOD:" + lMood + " .. SET MOOD ...");
                Buddy.Behaviour.SetMood(lMood, false);
                yield return new WaitForSeconds(1F);
                Buddy.Behaviour.SetMood(Mood.NEUTRAL, false);
            }
            // play a random BML or specific one if iBML != null
            else {
                bool lBMLIsEnding = false;

                if (string.IsNullOrEmpty(iBML)) {
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
            }

            yield return new WaitForSeconds(0.200F);

            if (iOnEnd != null)
                iOnEnd();
        }

        public static IEnumerator WaitTimeAsync(float iWaitingTime, Action iOnEndWaiting)
        {
            yield return new WaitForSeconds(iWaitingTime);
            if (iOnEndWaiting != null)
                iOnEndWaiting();
        }

        public static float WrapAngle(double iAngle)
        {
            iAngle = (double)Math.IEEERemainder((double)iAngle, 360);
            if (iAngle <= -180) {
                iAngle += 360;
            }
            else if (iAngle > 180) {
                iAngle -= 360;
            }

            return (float)iAngle;
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
