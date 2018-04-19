using Buddy;
using Buddy.UI;

using System;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;


namespace BuddyApp.Reminder
{

    public class UserState : AStateMachineBehaviour
    {
        private List<string> name = new List<string>();
        private string Voice;

        // Use this for initialization
        public override void Start()
        {
            
            //name.Add("Billy");
            //name.Add("Jack");
            //name.Add("Bob");
            //name.Add(null);
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            UserAccount[] lAccounts = BYOS.Instance.DataBase.GetUsers();
            foreach (UserAccount lUser in lAccounts)
            {
                //if (Buddy.WebRTCListener.RemoteID.Trim() == lUser.Email)
                //{
                name.Add(lUser.FirstName);
                //}
            }
            //name.Add(null);
            mReminderBehaviour = GetComponent <ReminderBehaviour>();
            mReminderBehaviour.QuestionTime(Dictionary.GetRandomString("wyname"));
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (ReminderData.Instance.VocalRequest != "" && mReminderBehaviour.IsVocalGet)
            {
                mReminderBehaviour.IsVocalGet = false;
                Voice = ReminderData.Instance.VocalRequest.ToLower();
                Debug.Log("Get this NAME !");
                for (int i = 0; i < name.Count; i++)
                {
                    Debug.Log("NUMERO = " + i);
                    Debug.Log("WORD = " + name[i]);
                    if (ContainsOneOf(Voice, name[i]))
                    {
                        Debug.Log("SALUT " + name[i]);
                        mReminderBehaviour.AllParam.Add(name[i]);
                        ReminderData.Instance.VocalRequest = "";
                        ReminderData.Instance.SenderID = i;
                        Trigger("Date");
                    }

                }
                if(Dictionary.ContainsPhonetic(Voice, "everybody"))
                {
                    mReminderBehaviour.AllParam.Add(Dictionary.GetString("everybody"));
                    ReminderData.Instance.VocalRequest = "";
                    ReminderData.Instance.SenderID = -1;
                    Trigger("Date");
                }
                if (mReminderBehaviour.AllParam.Count != 1)
                {
                    ReminderData.Instance.VocalRequest = "";
                    mReminderBehaviour.QuestionTime(Dictionary.GetRandomString("what") + ", " +Dictionary.GetRandomString("wyname"));
                }
                // ADD CAROUSSEL WITH NAMES
            }
        }

        private bool ContainsOneOf(string iSpeech, string iListSpeech)
        {
            iSpeech = iSpeech.ToLower();
                string[] words = iListSpeech.Split(' ');
                if (words.Length < 2)
                {
                    words = iSpeech.Split(' ');
                    foreach (string word in words)
                    {
                        if (word == iListSpeech.ToLower())
                        {
                            return true;
                        }
                    }
                }
                else if (iSpeech.ToLower().Contains(iListSpeech.ToLower()))
                    return true;
            return false;
        }



    }

}
