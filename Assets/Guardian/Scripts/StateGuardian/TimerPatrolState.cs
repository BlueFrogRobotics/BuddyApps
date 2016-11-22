﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimerPatrolState : AStateGuardian {

    [SerializeField]
    private float Timer = 5.0f;

    private float mTimer = 5.0f;
    private bool mHasTalked =false;
    private Text mText;
    private GameObject mBackgroundPrefab;
    private GameObject mQuestionPrefab;
    private GameObject mHaloPrefab;
    private Animator mBackgroundAnimator;
    private Animator mQuestionAnimator;
    private Animator mHaloAnimator;
    private Button mValidateButton;
    private Button mCancelButton;
    private Animator mAnimator;
    private Image[] mHaloImages;
    private Image mIcoMessage;
    private Text mMessage;
    private GameObject mCounterTime;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        InitLink();

        animator.SetBool("ChangeState", false);
        mTimer = Timer;
        //mTTS = new BuddyFeature.Vocal.TextToSpeech();
        Debug.Log("debut timer state");
        mCounterTime.SetActive(true);
        mBackgroundPrefab.GetComponent<Canvas>().enabled = false;
        mBackgroundPrefab.SetActive(true);
        mQuestionPrefab.SetActive(true);
        mHaloPrefab.SetActive(true);
        mBackgroundAnimator.SetBool("Open", true);
        mQuestionAnimator.SetBool("Open", true);
        mHaloAnimator.SetBool("Open", true);
        

        mAnimator = animator;
        mCancelButton.onClick.AddListener(Cancel);
        mValidateButton.onClick.AddListener(Validate);
        mIcoMessage.enabled = false;
        for(int i=0; i<mHaloImages.Length; i++)
            mHaloImages[i].color= new Color(0F, 212f/255f, 209f/255f, 1F);
        mMessage.text = "JE LANCE LA SURVEILLANCE DANS";
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        
        int lTime = Mathf.CeilToInt(mTimer);
        
        mTimer -= Time.deltaTime;
        mText.text = "" + lTime;
        
        if (mTimer < 4.5f && mTimer >= 0.0f)
        {
            mBackgroundPrefab.GetComponent<Canvas>().enabled = true;
            mQuestionPrefab.GetComponent<Canvas>().enabled = true;
            mHaloPrefab.GetComponent<Canvas>().enabled = true;
        }

        else if (mTimer < 0.0f)
        {
            Validate();
        }
    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Debug.Log("fin timer");
        animator.SetBool("ChangeState", false);
        mHasTalked = false;
        mTimer = Timer;
        mCancelButton.onClick.RemoveAllListeners();
        mValidateButton.onClick.RemoveAllListeners();
    }

    private void Cancel()
    {
        mAnimator.SetBool("ChangeState", true);
        mAnimator.SetBool("Cancelled", true);
        //mBackgroundAnimator.SetBool("Open", false);
        mQuestionAnimator.SetBool("Open", false);
        mHaloAnimator.SetBool("Open", false);
    }

    private void Validate()
    {
        mAnimator.SetBool("ChangeState", true);
        //mBackgroundAnimator.SetBool("Open", false);
        mQuestionAnimator.SetBool("Open", false);
        mHaloAnimator.SetBool("Open", false);
    }

    private void InitLink()
    {
        mText = mStatePatrolManager.TextCounter;
        mBackgroundPrefab = mStatePatrolManager.BackgroundPrefab;
        mQuestionPrefab = mStatePatrolManager.QuestionPrefab;
        mHaloPrefab = mStatePatrolManager.HaloPrefab;
        mBackgroundAnimator = mStatePatrolManager.BackgroundAnimator;
        mQuestionAnimator = mStatePatrolManager.QuestionAnimator;
        mHaloAnimator = mStatePatrolManager.HaloAnimator;
        mCancelButton = mStatePatrolManager.CancelButton;
        mValidateButton = mStatePatrolManager.ValidateButton;
        mHaloImages = mStatePatrolManager.HaloImages;
        mIcoMessage = mStatePatrolManager.IcoMessage;
        mMessage = mStatePatrolManager.MessageText;
        mCounterTime = mStatePatrolManager.CounterTime;
    }

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}s

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
