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
            Debug.Log("pas de lol " + ReminderManager.Command.Content + " : " + ReminderManager.Command.Receiver);
            if (ReminderManager.Command.Intent == Intent.NONE ||
                (ReminderManager.Command.Content == null && ReminderManager.Command.Receiver == null)) {
                Debug.Log("pas de truc");
                Interaction.TextToSpeech.Say("oui tu peux me dire quoi, quand et pour qui");
                while (Interaction.TextToSpeech.IsSpeaking)
                    yield return null;
                Trigger("ProcessVocal");
            } else {
                if (ReminderManager.Command.Content == null || ReminderManager.Command.Content == "") {
                    Interaction.TextToSpeech.Say("quelle est le contenu");
                    while (Interaction.TextToSpeech.IsSpeaking)
                        yield return null;
                    Interaction.SpeechToText.OnBestRecognition.Add(GetContent);
                    Interaction.SpeechToText.Request();
                    yield return new WaitForSeconds(4F);
                    Interaction.SpeechToText.OnBestRecognition.Remove(GetContent);
                }

                if (ReminderManager.Command.RemindDate.Equals(DateTime.MinValue)) {
                    Interaction.TextToSpeech.Say("a quelle date");
                    while (Interaction.TextToSpeech.IsSpeaking)
                        yield return null;
                    Interaction.SpeechToText.OnBestRecognition.Add(GetDate);
                    Interaction.SpeechToText.Request();
                    yield return new WaitForSeconds(4F);
                    Interaction.SpeechToText.OnBestRecognition.Remove(GetDate);
                }

                if (ReminderManager.Command.Receiver == null || ReminderManager.Command.Receiver == "") {
                    Interaction.TextToSpeech.Say("qui est le destinataire");
                    while (Interaction.TextToSpeech.IsSpeaking)
                        yield return null;
                    Interaction.SpeechToText.OnBestRecognition.Add(GetReceiver);
                    Interaction.SpeechToText.Request();
                    yield return new WaitForSeconds(4F);
                    Interaction.SpeechToText.OnBestRecognition.Remove(GetReceiver);
                }

                if (ReminderManager.Command.Intent == Intent.ADD) {
                    Interaction.TextToSpeech.Say("D'accord je l'ajoute");
                    Reminder lReminder = new Reminder();
                    lReminder.RemindDate = ReminderManager.Command.RemindDate;
                    lReminder.Content = ReminderManager.Command.Content;
                    lReminder.Receiver = ReminderManager.Command.Receiver;
                    lReminder.Title = ReminderManager.Command.Title;
                    ReminderManager.ReminderContent.reminderList.Add(lReminder);
                    //lAdded = true;
                }
            }
            Trigger("ProcessVocal");
            Debug.Log("pas de chips");
        }

        private void GetContent(string iAnswer)
        {
            ReminderManager.Command.Content = iAnswer;
        }

        private void GetReceiver(string iAnswer)
        {
            ReminderManager.Command.Receiver = iAnswer;
        }

        private void GetDate(string iAnswer)
        {
            ProcessVocalWitAI lProcess = new ProcessVocalWitAI();
            Command lCommand = lProcess.ExtractParameters(iAnswer);
            ReminderManager.Command.RemindDate = lCommand.RemindDate;
        }
    }

}
