using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PasswordState : AStateGuardian {

    private PasswordWriter mPasswordWriter;
    private ParametersGuardian mParameters;
    private GameObject mObjectPasswordWriter;
    private GameObject mBackgroundPrefab;
    private GameObject mHaloPrefab;
    private Animator mHaloAnimator;
    private Animator mBackgroundAnimator;
    private Button mButtonValidate;
    private Button mButtonCancel;
    private Image mIcoMessage;
    private Text mMessage;
    private Image[] mHaloImages;

    private string mPassword;
    private bool mHasShownGrid;
    private float mTimer;
    private Animator mAnimator;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        InitLink();

        //ParametersGuardian lParamGuardian = mParameterObject.GetComponent<ParametersGuardian>();
        mPassword = mParameters.Password.text;
        mPasswordWriter = mObjectPasswordWriter.GetComponent<PasswordWriter>();
        mPasswordWriter.Clear();
        mButtonValidate.onClick.AddListener(Validate);
        mButtonCancel.onClick.AddListener(Cancel);
        mIcoMessage.enabled = false;
        mMessage.text = "5 SEC POUR ENTRER VOTRE MOT DE PASSE!";
        for (int i = 0; i < mHaloImages.Length; i++)
            mHaloImages[i].color = new Color(0F, 212f / 255f, 209f / 255f, 1F);
        //mBackgroundPrefab.SetActive(true);

        mHasShownGrid = false;
        mTimer = 2.0f;
        mAnimator = animator;
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        mTimer -= Time.deltaTime;
        if (!mHasShownGrid && mTimer<0.0f)
        {
            
            //mQuestionPrefab.SetActive(true);
            mObjectPasswordWriter.SetActive(true);
            mBackgroundPrefab.SetActive(true);
            mHaloPrefab.SetActive(true);
            mBackgroundAnimator.SetBool("Open", true);
            mHaloAnimator.SetBool("Open", true);
            mHasShownGrid = true;
            mTimer = 6.0f;
        }

        if(mHasShownGrid && mTimer < 0.0f)
        {
            Cancel();
        }
    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        mAnimator.SetBool("PasswordTrue", false);
        mAnimator.SetBool("ChangeState", false);
        mObjectPasswordWriter.SetActive(false);
        mBackgroundPrefab.SetActive(false);
        mHaloPrefab.SetActive(false);
        mButtonValidate.onClick.RemoveAllListeners();
        mButtonCancel.onClick.RemoveAllListeners();
    }

    private void Validate()
    {
        if (mPassword == mPasswordWriter.Password)
        {
            Debug.Log("valide");
            mAnimator.SetBool("PasswordTrue", true);
            mAnimator.SetBool("ChangeState", true);
        }
        else
        {
            Debug.Log("erreur");
            mAnimator.SetBool("PasswordTrue", false);
            mAnimator.SetBool("ChangeState", true);
        }
        mBackgroundAnimator.SetBool("Open", false);
        mHaloAnimator.SetBool("Open", false);
    }

    private void Cancel()
    {
        Debug.Log("cancel");
        mAnimator.SetBool("PasswordTrue", false);
        mAnimator.SetBool("ChangeState", true);
        mBackgroundAnimator.SetBool("Open", false);
        mHaloAnimator.SetBool("Open", false);
    }

    private void InitLink()
    {
        mObjectPasswordWriter = mStatePatrolManager.ObjectPasswordWriter;
        mBackgroundPrefab = mStatePatrolManager.BackgroundPrefab;
        mParameters = mStatePatrolManager.Parameters;
        mBackgroundAnimator = mStatePatrolManager.BackgroundAnimator;
        mButtonValidate = mStatePatrolManager.ButtonValidatePassword;
        mButtonCancel = mStatePatrolManager.ButtonCancelPassword;
        mHaloPrefab = mStatePatrolManager.HaloPrefab;
        mHaloAnimator = mStatePatrolManager.HaloAnimator;
        mIcoMessage = mStatePatrolManager.IcoMessage;
        mMessage = mStatePatrolManager.MessageText;
        mHaloImages = mStatePatrolManager.HaloImages;
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
