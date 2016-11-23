using UnityEngine;
using System;
using System.Collections;
using BuddyOS;

public class MenuPatrolState : AStateGuardian {

    private bool mHasTalked = false;
    private int mMode;
    private GameObject mMenu;
    private TextToSpeech mTTS;
    private SpeechToText mSTT;
    private Action<string> mActionSetMode;
    private float mTimer = 2.2f;
    private bool mHasAskedOnce = false;

    public int Mode { get { return mMode; } set { mMode = value; } }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        mMenu = mStatePatrolManager.Menu;

        animator.SetBool("ChangeState", false);
        mTTS = BYOS.Instance.TextToSpeech;
        mSTT = BYOS.Instance.SpeechToText;
        //mActionSetMode = new Action<string>(SetMode);
        mSTT.OnBestRecognition.Add(SetMode);
        mTimer = 2.2f;
        animator.SetInteger("Mode", 0);
        mMode = 0;
        mHasAskedOnce = false;
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (mMode != 0)
        {
            animator.SetBool("ChangeState", true);
            mSTT.OnBestRecognition.Remove(SetMode);
        }
        /*if (!mHasTalked)
        {
            mTTS.Say("Quel mode souhaite tu. Fixe ou mobile");
            mHasTalked = true;
        }*/

        mTimer -= Time.deltaTime;
        if (mTimer<0.0f && (!mHasAskedOnce || mSTT.HasFinished))
        {
            Debug.Log("ask");
            mHasAskedOnce = true;
            mMenu.SetActive(true);
            mSTT.Request();
            mTimer = 4.2f;
        }

        
    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        mSTT.OnBestRecognition.Clear();
        animator.SetInteger("Mode", mMode);
        mMenu.SetActive(false);
        animator.SetBool("ChangeState", false);
    }

    public void SetMode(string iAnswer)
    {
        if (mMode == 0)
        {
            Debug.Log(iAnswer);

            if (iAnswer.Contains("mobile") && !iAnswer.Contains("immobile"))//(iAnswer == "mobile")
            {
                mMode = 2;
            }
            else if (iAnswer.Contains("immobile") || iAnswer.Contains("fixe"))//(iAnswer == "immobile" || iAnswer=="fixe" || iAnswer=="six")
            {
                mMode = 1;
            }
            else
            {
                mTTS.Say("Je n ai pas compris");
            }
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
