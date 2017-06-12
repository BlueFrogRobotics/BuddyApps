using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BuddyApp.Reminder
{
    public class AddState : AStateMachineBehaviour
    {
        override public void Start()
        {
   
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            StartCoroutine(AddReminder());
            
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        private IEnumerator AddReminder()
        {
            bool lAdded = false;
            Debug.Log("pas de lol "+ mReminderManager.Command.Content+" : "+ mReminderManager.Command.Receiver);
            if (mReminderManager.Command.Intent == Intent.NONE || (mReminderManager.Command.Content == null && mReminderManager.Command.Receiver == null ))
            {
                Debug.Log("pas de truc");
                mTTS.Say("oui tu peux me dire quoi, quand et pour qui");
                while (mTTS.IsSpeaking)
                    yield return null;
                Trigger("ProcessVocal");
            }

            else
            {
                if(mReminderManager.Command.Content == null || mReminderManager.Command.Content == "")
                {
                    mTTS.Say("quelle est le contenu");
                    while (mTTS.IsSpeaking)
                        yield return null;
                    mSTT.OnBestRecognition.Add(GetContent);
                    mSTT.Request();
                    yield return new WaitForSeconds(4F);
                    mSTT.OnBestRecognition.Remove(GetContent);
                }

                if (mReminderManager.Command.RemindDate.Equals(DateTime.MinValue))
                {
                    mTTS.Say("a quelle date");
                    while (mTTS.IsSpeaking)
                        yield return null;
                    mSTT.OnBestRecognition.Add(GetDate);
                    mSTT.Request();
                    yield return new WaitForSeconds(4F);
                    mSTT.OnBestRecognition.Remove(GetDate);
                }

                if (mReminderManager.Command.Receiver == null || mReminderManager.Command.Receiver == "")
                {
                    mTTS.Say("qui est le destinataire");
                    while (mTTS.IsSpeaking)
                        yield return null;
                    mSTT.OnBestRecognition.Add(GetReceiver);
                    mSTT.Request();
                    yield return new WaitForSeconds(4F);
                    mSTT.OnBestRecognition.Remove(GetReceiver);
                }

                if (mReminderManager.Command.Intent == Intent.ADD)
                {
                    mTTS.Say("D'accord je l'ajoute");
                    Reminder lReminder = new Reminder();
                    lReminder.RemindDate = mReminderManager.Command.RemindDate;
                    lReminder.Content = mReminderManager.Command.Content;
                    lReminder.Receiver = mReminderManager.Command.Receiver;
                    lReminder.Title = mReminderManager.Command.Title;
                    mReminderManager.ReminderContent.reminderList.Add(lReminder);
                    //lAdded = true;
                }
            }
            Trigger("ProcessVocal");
            Debug.Log("pas de chips");
        }

        private void GetContent(string iAnswer)
        {
            mReminderManager.Command.Content = iAnswer;
        }

        private void GetReceiver(string iAnswer)
        {
            mReminderManager.Command.Receiver = iAnswer;
        }

        private void GetDate(string iAnswer)
        {
            ProcessVocalWitAI lProcess = new ProcessVocalWitAI();
            Command lCommand = lProcess.ExtractParameters(iAnswer);

            mReminderManager.Command.RemindDate = lCommand.RemindDate;
        }
    }

}
