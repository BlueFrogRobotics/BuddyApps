using Buddy;
using Buddy.UI;

using System;
using System.IO;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

using Buddy.Command;
using System.Globalization;

namespace BuddyApp.Reminder
{

    public class SaveState : AStateMachineBehaviour
    {

        private ReminderManager mReminderManager;
        private RemindersData mReminders;

        private bool mOk;

        public override void Start()
        {

            mReminderManager = GetComponent<ReminderManager>();
            mReminders = mReminderManager.RemindersData;

        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mOk = false;
            mVocal = GetComponent<ReminderBehaviour>();

            string Wow = Dictionary.GetRandomString("finalreminder");

            Wow = Wow.Replace("[user]", mVocal.AllParam[0]);
            Wow = Wow.Replace("[date]", mVocal.AllParam[1]);
            Wow = Wow.Replace("[hour]", mVocal.AllParam[2]);

            BYOS.Instance.Interaction.TextToSpeech.Say(Dictionary.GetRandomString("isok"));
            Toaster.Display<BinaryQuestionToast>().With(Wow, IsOk, ByeBye);
        }

        private void IsOk()
        {
            string lRemVoc = mVocal.AllParam[0] + mVocal.AllParam[1] + mVocal.AllParam[2];
            Reminderkey lRk = new Reminderkey();

            var charsToRemove = new string[] {":", "/"};

            foreach (var c in charsToRemove)
            {
                lRemVoc = lRemVoc.Replace(c, string.Empty);
            }

            //Get the xml reminder : 
            lRk.Key = lRemVoc;
            lRk.Name = mVocal.AllParam[0];
            lRk.Date = mVocal.AllParam[1];
            lRk.Hour = mVocal.AllParam[2];

            

            
            DateTime lDate = DateTime.ParseExact(lRk.Date, "dd/MM/yyyy",
                                       System.Globalization.CultureInfo.InvariantCulture);
            double lHour = 0;
            double.TryParse(lRk.Hour, out lHour);

            int lReccurence = 0;
            int.TryParse(mVocal.AllParam[3], out lReccurence);
            RemindType lRemindType = RemindType.ONE_TIME;
            RemindRecurrence lRemindRecurrence = RemindRecurrence.NONE;

            switch (lReccurence)
            {
                case 0:
                    lRemindType = RemindType.ONE_TIME;
                    lRemindRecurrence = RemindRecurrence.NONE;
                    break;
                case 1:
                    lRemindType = RemindType.RECURRENT;
                    lRemindRecurrence = RemindRecurrence.EVERY_DAY;
                    break;
                case 2:
                    lRemindType = RemindType.RECURRENT;
                    lRemindRecurrence = RemindRecurrence.EVERY_WEEK;
                    break;
                case 3:
                    lRemindType = RemindType.RECURRENT;
                    lRemindRecurrence = RemindRecurrence.EVERY_MONTH;
                    break;
                default:
                    lRemindType = RemindType.ONE_TIME;
                    lRemindRecurrence = RemindRecurrence.NONE;
                    break;
            }

            lDate.AddHours(lHour);
            Debug.Log("date saved: " + lDate.ToLongDateString());
            //DateTime.Today.AddMinutes(5);
            //BYOS.Instance.DataBase.Memory.Procedural.AddReminder("content", 0, "adresse");
            lRk.ID=BYOS.Instance.DataBase.Memory.Procedural.AddReminder(lDate, RemindPrecision.MINUTE, "message", ReminderData.Instance.SenderID, mVocal.Name[ReminderData.Instance.SenderID], lRemindType, lRemindRecurrence);
            Debug.Log("2");
            Debug.Log("id: "+ lRk.ID);
            mReminders.Reminders.Add(lRk);
            Debug.Log("3");
            Utils.Save(BYOS.Instance.Resources.GetPathToRaw(""+ lRk.ID + ".wav"), mVocal.RemindMe);
            Debug.Log("4");
            string[] lReminderfile = Directory.GetFiles(BYOS.Instance.Resources.GetPathToRaw("Reminders"));
            Debug.Log("5");
            Utils.SerializeXML<RemindersData>(mReminders, lReminderfile[0]);
            Debug.Log("6");
            BYOS.Instance.Interaction.TextToSpeech.Say(Dictionary.GetRandomString("reminderok"));
            mOk = true;
        }

        private void ByeBye()
        {
            BYOS.Instance.Interaction.TextToSpeech.Say(Dictionary.GetRandomString("remindernotok"));
            mOk = true;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (mOk)
            {
                if (BYOS.Instance.Interaction.TextToSpeech.HasFinishedTalking)
                {

                    QuitApp();

                }
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }
    }
}