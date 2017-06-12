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

        override public void Start() {
            Debug.Log("YOUPI TRALALA!!!!!!!!!!!!!!!!!!!!!!!!!!");
            //mProcessVocal = new ProcessVocalManual();
            mProcessVocal = new ProcessVocalWitAI();
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTimer = 0.0f;
            mCommand = null;
            mCommandText = "";
            mSTT.OnBestRecognition.Add(OnBestReco);
            if (mReminderManager.CommandText == "")
            {
                mSTT.Request();
            }
            else
                mCommandText = mReminderManager.CommandText;
            
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTimer += Time.deltaTime;

            if(mCommandText!="" && mCommand==null)
            {
                mCommand = mProcessVocal.ExtractParameters(mCommandText);
                Debug.Log("1");
                switch (mCommand.Intent)
                {
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
                mReminderManager.Command = mCommand;
            }

            if(mTimer>5.0f && mCommand == null)
            {
                QuitApp();
                mTimer = 0.0f;
            }

        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mReminderManager.CommandText = "";
            mSTT.OnBestRecognition.Remove(OnBestReco);
        }

        private void OnBestReco(string iText)
        {
            Debug.Log("on best lol");
            mCommandText = iText;
        }


    }
}