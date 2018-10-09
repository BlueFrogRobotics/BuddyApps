﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.BuddyLab
{
    public sealed class BLScene : AStateMachineBehaviour
    {
        private LabUIEditorManager mUIManager;
        private ItemControlUnit mItemControl;
        private BuddyLabBehaviour mBLBehaviour;


        public override void Start()
        {
            mUIManager = GetComponent<LabUIEditorManager>();
            mItemControl = GetComponentInGameObject<ItemControlUnit>(4);
            mBLBehaviour = GetComponentInGameObject<BuddyLabBehaviour>(3);
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("on enter de blscene");
            StartCoroutine(InitScene());
            //Interaction.Mood.Set(Buddy.MoodType.NEUTRAL);
            //Debug.Log("name project: " + mBLBehaviour.NameOpenProject);
            //mItemControl.CleanSequence();
            //mItemControl.ShowSequence(mBLBehaviour.NameOpenProject + ".xml");
            ////mUIManager.SetBackground(true);
            //mUIManager.OpenBottomUI();
            //mUIManager.OpenLineProgram();
            //mUIManager.OpenTrashArea();
            
            //mUIManager.PlayButton.onClick.AddListener(PlaySequence);
            //mUIManager.SaveButton.onClick.AddListener(SaveSequence);
            //mUIManager.FolderButton.onClick.AddListener(OpenFolder);
            //mUIManager.BackButton.onClick.AddListener(GoToMenu);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //mUIManager.SetBackground(false);
            
            mUIManager.CloseBottomUI();
            mUIManager.CloseLineProgram();
            mUIManager.CloseTrashArea();
            mUIManager.PlayButton.onClick.RemoveListener(PlaySequence);
            mUIManager.SaveButton.onClick.RemoveListener(SaveSequence);
            mUIManager.FolderButton.onClick.RemoveListener(OpenFolder);
            mUIManager.BackButton.onClick.RemoveListener(GoToMenu);
            mUIManager.UndoButton.onClick.RemoveListener(Undo);
            mUIManager.RedoButton.onClick.RemoveListener(Redo);
            GetGameObject(6).GetComponent<Animator>().SetTrigger("close");

        }

        private void PlaySequence()
        {
            mUIManager.CloseWindows();
            mItemControl.SaveSequence();
            mItemControl.FillBehaviourAlgorithm(mBLBehaviour.NameOpenProject + ".xml");
            Trigger("Play");
        }

        private void SaveSequence()
        {
            mItemControl.SaveSequence();
        }

        private void OpenFolder()
        {
            mUIManager.CloseWindows();
            mItemControl.SaveSequence();
            Trigger("StartOpen");
        }

        private void GoToMenu()
        {
            mUIManager.CloseWindows();
            mItemControl.SaveSequence();
            Trigger("ProjectToMenu");
        }

        private void Undo()
        {
            mItemControl.Undo();
        }

        private void Redo()
        {
            mItemControl.Redo();
        }

        private IEnumerator InitScene()
        {
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
            Debug.Log("name project: " + mBLBehaviour.NameOpenProject);
            mItemControl.CleanSequence();
            yield return null;
            Debug.Log("avant algo");
            mItemControl.ShowAlgo(mBLBehaviour.NameOpenProject + ".xml");
            Debug.Log("apres algo");
            //mItemControl.ShowSequence(mBLBehaviour.NameOpenProject + ".xml");
            //mUIManager.SetBackground(true);
            mUIManager.OpenBottomUI();
            mUIManager.OpenLineProgram();
            mUIManager.OpenTrashArea();
            mUIManager.PlayButton.onClick.AddListener(PlaySequence);
            mUIManager.SaveButton.onClick.AddListener(SaveSequence);
            mUIManager.FolderButton.onClick.AddListener(OpenFolder);
            mUIManager.BackButton.onClick.AddListener(GoToMenu);
            mUIManager.UndoButton.onClick.AddListener(Undo);
            mUIManager.RedoButton.onClick.AddListener(Redo);
        }

    }
}

