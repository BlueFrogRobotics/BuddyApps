using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BuddyApp.Guardian
{
    public class TimerPatrolState : AStateGuardian
    {

        [SerializeField]
        private float Timer = 5.0f;

        private float mTimer = 5.0f;
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
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            InitLink();
            SetWindowAppOverBuddyColor(1);
            animator.SetBool("ChangeState", false);
            mTimer = Timer;
            //mTTS = new BuddyFeature.Vocal.TextToSpeech();
            Debug.Log("debut timer state");
            mCounterTime.SetActive(true);

            //mBackgroundPrefab.GetComponent<Canvas>().enabled = false;
            mBackgroundPrefab.SetActive(true);
            mQuestionPrefab.SetActive(true);
            mHaloPrefab.SetActive(true);
            Debug.Log("meuh");
            mBackgroundAnimator.SetTrigger("Open_BG");// SetBool("Open", true);
            mQuestionAnimator.SetTrigger("Open_WQuestion");//.SetBool("Open", true);
            mHaloAnimator.SetTrigger("Open_WTimer");//.SetBool("Open", true);


            mAnimator = animator;
            mCancelButton.onClick.AddListener(Cancel);
            mValidateButton.onClick.AddListener(Validate);
            mIcoMessage.enabled = false;
            for (int i = 0; i < mHaloImages.Length; i++)
                mHaloImages[i].color = new Color(0F, 212f / 255f, 209f / 255f, 1F);
            mMessage.text = "JE LANCE LA SURVEILLANCE DANS";
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            int lTime = Mathf.CeilToInt(mTimer);
            //mBackgroundPrefab.GetComponent<Canvas>().enabled = true;
            mTimer -= Time.deltaTime;
            mText.text = "" + lTime;

            if (mTimer < 4.5f && mTimer >= 0.0f)
            {
                //mBackgroundPrefab.GetComponent<Canvas>().enabled = true;
                mQuestionPrefab.GetComponent<Canvas>().enabled = true;
                mHaloPrefab.GetComponent<Canvas>().enabled = true;
            }

            else if (mTimer < 0.0f)
            {
                Validate();
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("fin timer");
            //animator.SetBool("ChangeState", false);
            mTimer = Timer;
            mCancelButton.onClick.RemoveAllListeners();
            mValidateButton.onClick.RemoveAllListeners();
        }

        private void Cancel()
        {
            mAnimator.SetBool("ChangeState", true);
            mAnimator.SetBool("Cancelled", true);
            mBackgroundAnimator.SetTrigger("Close_BG");//.SetBool("Open", false);
            mQuestionAnimator.SetTrigger("Close_WQuestion");//.SetBool("Open", false);
            mHaloAnimator.SetTrigger("Close_WTimer");//.SetBool("Open", false);
        }

        private void Validate()
        {
            mAnimator.SetBool("ChangeState", true);
            mBackgroundAnimator.SetTrigger("Close_BG"); //SetBool("Open", false);
            mQuestionAnimator.SetTrigger("Close_WQuestion");//.SetBool("Open", false);
            mHaloAnimator.SetTrigger("Close_WTimer");//.SetBool("Open", false);
        }

        private void InitLink()
        {
            mText = StateManager.TextCounter;
            mBackgroundPrefab = StateManager.BackgroundPrefab;
            mQuestionPrefab = StateManager.QuestionPrefab;
            mHaloPrefab = StateManager.HaloPrefab;
            mBackgroundAnimator = StateManager.BackgroundAnimator;
            mQuestionAnimator = StateManager.QuestionAnimator;
            mHaloAnimator = StateManager.HaloAnimator;
            mCancelButton = StateManager.CancelButton;
            mValidateButton = StateManager.ValidateButton;
            mHaloImages = StateManager.HaloImages;
            mIcoMessage = StateManager.IcoMessage;
            mMessage = StateManager.MessageText;
            mCounterTime = StateManager.CounterTime;
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
}