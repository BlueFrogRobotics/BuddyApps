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

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            finalcountdown = CommonIntegers["finalcountdown"];
            Interaction.Mood.Set(MoodType.THINKING);
			if (finalcountdown > 60) {
				BYOS.Instance.DataBase.Memory.Procedural.AddReminder(DateTime.Now.AddSeconds(-finalcountdown), "", 0, ReminderType.ALARM);
				QuitApp();
			} else
				BYOS.Instance.Toaster.Display<CountdownToast>().With(finalcountdown, EndTimer);

			TimerData.Instance.VocalRequest = "";

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