﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.BuddyLab
{
    public class BLScene : AStateMachineBehaviour
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
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("name project: " + mBLBehaviour.NameOpenProject);
            mItemControl.CleanSequence();
            mItemControl.ShowSequence(mBLBehaviour.NameOpenProject + ".xml");
            //mUIManager.SetBackground(true);
            mUIManager.OpenBottomUI();
            mUIManager.OpenLineProgram();
            mUIManager.OpenTrashArea();
            
            mUIManager.PlayButton.onClick.AddListener(PlaySequence);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //mUIManager.SetBackground(false);
            mUIManager.CloseBottomUI();
            mUIManager.CloseLineProgram();
            mUIManager.CloseTrashArea();
            mUIManager.PlayButton.onClick.RemoveListener(PlaySequence);
            GetGameObject(6).GetComponent<Animator>().SetTrigger("close");

        }

        private void PlaySequence()
        {          
            mItemControl.SaveSequence();
            Trigger("Play");
        }

    }
}

