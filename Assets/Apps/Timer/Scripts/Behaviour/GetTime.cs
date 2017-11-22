using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Buddy;
using Buddy.UI;

namespace BuddyApp.Timer
{
    public class GetTime : AStateMachineBehaviour
    {
        private int finalsec;
        private int sec;
        private int min;
        private int hour;

        private string Voice;
        // Use this for initialization
        public override void Start()
        {
            
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            finalsec = 0;
            if (TimerData.Instance.VocalRequest != "")
            {
                Voice = TimerData.Instance.VocalRequest.ToLower();
                ParseTime(Voice);
            }
        }

        private void ParseTime (string iVoice)
        {
            double num;
            string lCnumber;
            string[] lWord = iVoice.Split(' ');
            lCnumber = null;

            for (int i = 0; i <= lWord.Length; i++)
            {
                if (double.TryParse(lWord[i], out num))
                {
                    lCnumber = lWord[i];
                    i++;
                }


                //if ((lWord[i].Equals(Dictionary.GetPhoneticStrings("sec")) || lWord[i].Equals(Dictionary.GetPhoneticStrings("secs"))))
                if ((lWord[i].Equals("seconde") || lWord[i].Equals("secondes")) && lCnumber != null)
                {
                    finalsec += Int32.Parse(lCnumber);
                    lCnumber = null;
                    i++;
                }
                else if ((lWord[i].Equals("minutes") || lWord[i].Equals("minute")) && lCnumber != null)
                {
                    finalsec += Int32.Parse(lCnumber) * 60;
                    lCnumber = null;
                    i++;
                }
                else if ((lWord[i].Equals("heure") || lWord[i].Equals("heures")) && lCnumber != null)
                {
                    finalsec += Int32.Parse(lCnumber) * 60 * 60;
                    lCnumber = null;
                    i++;
                }
                //else if (lWord[i].Equals(Dictionary.GetPhoneticStrings("hour")) || lWord[i].Equals(Dictionary.GetPhoneticStrings("hours")))
            }

            CommonIntegers["finalcountdown"] = finalsec;

            Trigger("Countdown");

        }


        // Update is called once per frame
        void Update()
        {

        }
    }
}
