using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Buddy;

namespace BuddyApp.Reminder
{
    public class ProcessVocalState : AStateMachineBehaviour
    {
        private string mCommandText = "";
        private Command mCommand;
        private IProcessVocal mProcessVocal;
        private float mTimer = 0.0f;

        override public void Start()
        {
            //Debug.Log("YOUPI TRALALA!!!!!!!!!!!!!!!!!!!!!!!!!!");
            mProcessVocal = new ProcessVocalManual();
            //mProcessVocal = new ProcessVocalWitAI();
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTimer = 0.0f;
            mCommand = null;
            mCommandText = "";
            Interaction.SpeechToText.OnBestRecognition.Add(OnBestReco);
            if (ReminderManager.CommandText == "") {
                Interaction.SpeechToText.Request();
            } else
                mCommandText = ReminderManager.CommandText;

        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTimer += Time.deltaTime;

            if (mCommandText != "" && mCommand == null) {
                mCommand = ReminderManager.ProcessVocal.ExtractParameters(mCommandText);
                Debug.Log("1");
                switch (mCommand.Intent) {
                    case Intent.ADD:
                        Trigger("Add");
                        Debug.Log("2");
                        break;
                    case Intent.PRINT:
                        Trigger("Print");
                        break;
                    default:
                        break;
                }
                ReminderManager.Command = mCommand;
            }

            if (mTimer > 5.0f && mCommand == null) {
                QuitApp();
                mTimer = 0.0f;
            }

        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ReminderManager.CommandText = "";
            Interaction.SpeechToText.OnBestRecognition.Remove(OnBestReco);
        }

        private void OnBestReco(string iText)
        {
            Debug.Log("on best lol");
            mCommandText = iText;
        }


    }
}