using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Buddy;
using Buddy.UI;

namespace BuddyApp.Timer
{

    public class Countdown : AStateMachineBehaviour {

        int finalcountdown;
        private bool mIsDone;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mIsDone = false;

            finalcountdown = CommonIntegers["finalcountdown"];
            //Interaction.Mood.Set(MoodType.THINKING);
			if (finalcountdown > 60) {
				DateTime lDateTimer = DateTime.Now.AddSeconds(finalcountdown);
				BYOS.Instance.DataBase.Memory.Procedural.AddReminder(lDateTimer, "Timer ended!! (" + lDateTimer.Hour + ":" + lDateTimer.Minute + ")", 0, "", ReminderType.ALARM, true);
                mIsDone = true;
                
                
            } else
				BYOS.Instance.Toaster.Display<CountdownToast>().With(finalcountdown, EndTimer);

			TimerData.Instance.VocalRequest = "";

		}

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(mIsDone)
            {
                BYOS.Instance.Interaction.TextToSpeech.Say(Dictionary.GetString("whenalarm").Replace("[nbSecond]", finalcountdown.ToString()));
                mIsDone = false;
            }
            if (BYOS.Instance.Interaction.TextToSpeech.HasFinishedTalking && finalcountdown > 60)
                QuitApp();
        }

        private IEnumerator DisplayAlert(String bipbip)
        {
            Debug.Log("display Alert");
            BYOS.Instance.Toaster.Display<TextToast>().With(bipbip);
            yield return new WaitForSeconds(5F);
            BYOS.Instance.Toaster.Hide();
            CommonIntegers["finalcountdown"] = 0;
            finalcountdown = 0;
        }
        
        private void EndTimer()
        {
            Trigger("Bipbip");
            //StartCoroutine(DisplayAlert("Bipbip je sonne"));
        }
        
    }
}