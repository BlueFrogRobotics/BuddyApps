using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Text.RegularExpressions;
using Buddy;
using System;

namespace BuddyApp.PlayMath
{
    public class PMSelectTableState : AnimatorSyncState
    {

        private Animator mSetTableAnimator;
        private SelectTableBehaviour mSelectTableBehaviour;

        private bool mIsOpen;
        private bool mListening;
        private string mSpeechReco;
        private VocalManager mVocalManager;

        public override void Start()
        {
            mVocalManager = Interaction.VocalManager;
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mSetTableAnimator = GameObject.Find("UI/Set_Table").GetComponent<Animator>();

            mPreviousStateBehaviours.Add(GameObject.Find("UI/Menu").GetComponent<MainMenuBehaviour>());

            mIsOpen = false;
            mListening = false;
            mSpeechReco = string.Empty;

            mSelectTableBehaviour = mSetTableAnimator.gameObject.GetComponent<SelectTableBehaviour>();
            mSelectTableBehaviour.InitState();
            SetTablesStarProgress();

            // Use Vocon
            mVocalManager.UseVocon = true;
            mVocalManager.AddGrammar("timestable", LoadContext.APP);
            mVocalManager.OnVoconBest = VoconBest;
            mVocalManager.OnVoconEvent = EventVocon;
            mVocalManager.EnableDefaultErrorHandling = false;
        }

        private void EventVocon(VoconEvent iEvent)
        {
            Debug.Log(iEvent);
        }

        private void VoconBest(VoconResult iBestResult)
        {
            int lNumberTable = 0;
            mListening = false;
            mSpeechReco = iBestResult.Utterance;
            if (int.TryParse(mSpeechReco, out lNumberTable))
                if (lNumberTable > 0 && lNumberTable <= 9)
                {
                    mSelectTableBehaviour.SetTimeTable(lNumberTable);
                    return;
                }
            mSpeechReco = string.Empty;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (PreviousBehaviourHasEnded() && !mIsOpen)
            {
                mSetTableAnimator.SetTrigger("open");
                mIsOpen = true;
            }
            if (!Interaction.TextToSpeech.HasFinishedTalking || mListening)
                return;

            if (string.IsNullOrEmpty(mSpeechReco))
            {
                mVocalManager.StartInstantReco(false);
                mListening = true;
                return;
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mSetTableAnimator.SetTrigger("close");
            mIsOpen = false;
            mVocalManager.RemoveGrammar("timestable", LoadContext.APP);
            mVocalManager.OnVoconBest = null;
            mVocalManager.OnVoconEvent = null;
            mVocalManager.UseVocon = false;
        }

        private void SetTablesStarProgress()
        {
            Text[] childs = GameObject.Find("UI/Set_Table/Middle_UI").GetComponentsInChildren<Text>();
            foreach (Text child in childs)
            {
                int table = int.Parse(Regex.Match(child.text, @"\d+").Value);
                GameObject StarProgress = child.gameObject.transform.parent.Find("Star_Progress").gameObject;
                StarProgress.transform.Find("Mask_10").gameObject.SetActive(User.Instance.HasTableCertificate(table));
            }
        }
    }
}