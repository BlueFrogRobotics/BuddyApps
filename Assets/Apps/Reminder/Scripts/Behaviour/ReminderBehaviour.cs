using Buddy;
using Buddy.UI;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Reminder
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class ReminderBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private ReminderData mAppData;

        public List<string> Name = new List<string>();

        internal TextToSpeech mTTS = BYOS.Instance.Interaction.TextToSpeech;
        internal bool IsVocalGet = false;
        internal List<string> AllParam= new List<string>();
        internal AudioClip RemindMe;

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			ReminderActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = ReminderData.Instance;

            UserAccount[] lAccounts = BYOS.Instance.DataBase.GetUsers();
                Debug.Log(lAccounts + " Poney");
            foreach (UserAccount lUser in lAccounts)
            {
                if (lUser == null)
                    Debug.Log(lUser + "Lol");

                //if (Buddy.WebRTCListener.RemoteID.Trim() == lUser.Email)
                //{
                 Name.Add(lUser.FirstName);
                //}
            }
            //Name.Add("Billy");
            //Name.Add("Jack");
            //Name.Add("Bob");
            //Name.Add(null);
        }


        public void QuestionTime(string iQuestion)
        {
            IsVocalGet = false;
            mTTS.Say(iQuestion);
            StartCoroutine(WaitTTSLoading());
        }

        private IEnumerator WaitTTSLoading()
        {
            yield return new WaitForSeconds(1.0f);
            while (mTTS.IsSpeaking)
                yield return null;
            Debug.Log("Vocal");
            BYOS.Instance.Interaction.VocalManager.OnEndReco = GetAnswer;
            BYOS.Instance.Interaction.VocalManager.OnError = NoAnswer;
            BYOS.Instance.Interaction.VocalManager.StartInstantReco();
        }

        private void GetAnswer(string iAnswer)
        {

            Utils.LogI(LogContext.APP, "GOT AN ANSWER: " + iAnswer);
            ReminderData.Instance.VocalRequest = iAnswer.ToLower();
            IsVocalGet = true;
        }

        private void NoAnswer(STTError iError)
        {
            Utils.LogI(LogContext.APP, "VM error");
            Debug.Log("GOT NO ANSWER");
        }

    }
}