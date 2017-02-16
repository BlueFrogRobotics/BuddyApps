using UnityEngine;
using System.Collections;

namespace BuddyApp.Guardian
{
    public class DetectionGuardianState : AStateGuardian
    {

        //parameters that can be chosen at the parameters menu
        [SerializeField]
        private bool canDetectFire = true;

        [SerializeField]
        private bool canDetectSound = true;

        [SerializeField]
        private bool canDetectMovement = true;

        [SerializeField]
        private bool canDetectKidnapping = true;

        //boolean that can be used to desactivate temporaly the detection
        [SerializeField]
        private bool isDetectingFire = true;

        [SerializeField]
        private bool isDetectingSound = true;

        [SerializeField]
        private bool isDetectingMovement = true;

        [SerializeField]
        private bool isDetectingKidnapping = true;

        //parameters that can be chosen at the parameters menu
        public bool CanDetectFire { get { return canDetectFire; } set { canDetectFire = value; } }
        public bool CanDetectSound { get { return canDetectSound; } set { canDetectSound = value; } }
        public bool CanDetectMovement { get { return canDetectMovement; } set { canDetectMovement = value; } }
        public bool CanDetectKidnapping { get { return canDetectKidnapping; } set { canDetectKidnapping = value; } }

        //boolean that can be used to desactivate temporaly the detection
        public bool IsDetectingFire { get { return isDetectingFire; } set { isDetectingFire = value; } }
        public bool IsDetectingSound { get { return isDetectingSound; } set { isDetectingSound = value; } }
        public bool IsDetectingMovement { get { return isDetectingMovement; } set { isDetectingMovement = value; } }
        public bool IsDetectingKidnapping { get { return isDetectingKidnapping; } set { isDetectingKidnapping = value; } }

        private Detectors mDetectorManager;
        private Animator mAnimator;


        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            SetWindowAppOverBuddyColor(0);
            SetParamDetection();
            //Debug.Log("peut detecter son: " + canDetectSound + " " + isDetectingSound);
            //mDetectorManager = StateManager.DetectorManager;
            mDetectorManager = StateManager.Detectors;
            animator.SetBool("IsDetecting", true);
            animator.SetBool("ChangeState", false);
            mAnimator = animator;

            if (canDetectSound && !mDetectorManager.SoundDetector.HasStarted)
                mDetectorManager.SoundDetector.StartMic();

            mDetectorManager.SoundDetector.OnDetection += OnSoundDetected;
            mDetectorManager.MovementDetector.OnDetection += OnMovementDetected;
            mDetectorManager.KidnappingDetector.OnDetection += OnKidnappingDetected;
            mDetectorManager.FireDetector.OnDetection += OnFireDetected;


        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mDetectorManager.SoundDetector.OnDetection -= OnSoundDetected;
            mDetectorManager.MovementDetector.OnDetection -= OnMovementDetected;
            mDetectorManager.KidnappingDetector.OnDetection -= OnKidnappingDetected;
            mDetectorManager.FireDetector.OnDetection -= OnFireDetected;
            animator.SetBool("ChangeState", false);
        }

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        private void SetParamDetection()
        {
            canDetectFire = GuardianData.Instance.FireDetectionIsActive;
            canDetectMovement = GuardianData.Instance.MovementDetectionIsActive;
            canDetectKidnapping = GuardianData.Instance.KidnappingDetectionIsActive;
            canDetectSound = GuardianData.Instance.SoundDetectionIsActive;
            //if (GuardianData.Instance.Recever == GuardianData.Contact.NOBODY)
            //    Debug.Log("vraiment lol");
            //else if (GuardianData.Instance.Recever == GuardianData.Contact.WALID)
            //    Debug.Log("plutot lol");
        }

        private void OnSoundDetected()
        {
            if (canDetectSound && isDetectingSound)
            {
                int lAlerte = (int)DetectionManager.Alert.SOUND;
                mAnimator.SetBool("ChangeState", true);
                mAnimator.SetBool("HasAlerted", true);
                mAnimator.SetInteger("Alerte", lAlerte);
                Debug.Log("alerte!!!: " + lAlerte);
            }
        }

        private void OnMovementDetected()
        {
            if (canDetectMovement && isDetectingMovement)
            {
                int lAlerte = (int)DetectionManager.Alert.MOVEMENT;
                mAnimator.SetBool("ChangeState", true);
                mAnimator.SetBool("HasAlerted", true);
                mAnimator.SetInteger("Alerte", lAlerte);
                Debug.Log("alerte!!!: " + lAlerte);
            }
        }

        private void OnFireDetected()
        {
            if (canDetectFire && isDetectingFire)
            {
                int lAlerte = (int)DetectionManager.Alert.FIRE;
                mAnimator.SetBool("ChangeState", true);
                mAnimator.SetBool("HasAlerted", true);
                mAnimator.SetInteger("Alerte", lAlerte);
                Debug.Log("alerte!!!: " + lAlerte);
            }
        }

        private void OnKidnappingDetected()
        {
            if (canDetectKidnapping && isDetectingKidnapping)
            {
                int lAlerte = (int)DetectionManager.Alert.KIDNAPPING;
                mAnimator.SetBool("ChangeState", true);
                mAnimator.SetBool("HasAlerted", true);
                mAnimator.SetInteger("Alerte", lAlerte);
                Debug.Log("alerte!!!: " + lAlerte);
            }
        }

        private int CheckAlarm()
        {
            if (mDetectorManager.FireDetector.IsFireDetected && canDetectFire && isDetectingFire)
            {
                return (int)DetectionManager.Alert.FIRE;
            }
            else if (mDetectorManager.SoundDetector.IsASoundDetected && canDetectSound && isDetectingSound)
            {
                return (int)DetectionManager.Alert.SOUND;
            }
            else if (mDetectorManager.KidnappingDetector.IsBeingKidnapped && canDetectKidnapping && isDetectingKidnapping)
            {
                return (int)DetectionManager.Alert.KIDNAPPING;
            }
            else if (mDetectorManager.MovementDetector.IsMovementDetected && canDetectMovement && isDetectingMovement)
            {
                return (int)DetectionManager.Alert.MOVEMENT;
            }

            else
            {
                return (int)DetectionManager.Alert.NONE;
            }
        }
    }
}