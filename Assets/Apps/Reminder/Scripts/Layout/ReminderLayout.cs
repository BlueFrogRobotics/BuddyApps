using Buddy.UI;
using Buddy.Command;
using Buddy;
using System;
using UnityEngine;

namespace BuddyApp.Reminder
{
    public class ReminderLayout : AWindowLayout
    {
        public Dropdown mContacts;

        private Button mHearButton;

        public Action HearCallback { get; set; }

        public RemindersData Users { get; set; }

        public DropdownCallback UserSelectCallback { get; set; }
       
        //public Button Hear { get; set; }
        //public Button mDelete { get; set; }


        public override void Build()
        {

            Title = "Current Rappel";
            //HearCallback = null;

            CreateWidgets();

            mHearButton.OnClickEvent(() => HearCallback());

            //////////////////////////
            mContacts.OnSelectEvent((iLabel, iObj, iIdx) =>
            {
                UserSelectCallback(iLabel, iObj, iIdx);
            });

            foreach (Reminderkey lUser in Users.Reminders)
            {
                //Debug.Log("C CA : " + lUser.FirstName);
                Debug.Log("Name " + lUser.Name + " KEY " + lUser.Key);
                mContacts.AddOption(lUser.Name, lUser.Key);
            }

            mContacts.SetDefault("Bob" + " " + "Bob170120180200");
        }

        private void CreateWidgets()
        {
            mContacts = CreateWidget<Dropdown>();
            mHearButton = CreateWidget<Button>();

        }

        public override void LabelizeWidgets()
        {
            mContacts.Label = "Contacts";
            mHearButton.Label = "Lire";
        }
    }
}