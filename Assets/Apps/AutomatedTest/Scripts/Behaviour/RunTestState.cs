using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System;

namespace BuddyApp.AutomatedTest
{
    //IDEE : utiliser les invoke pour chaque fonction a partir de la liste et faire des invoke (nameof(nomMethod), X), apres avoir un bool global pour chaque action si c'est fini et enfin avoir une fonction 
    //globale pour poser une question pour refaire le test. (peut etre le mettre en option)


    public enum MoveTest
    {
        ROTATE,
        ROTATE_YES,
        ROTATE_NO,
        MOVE_FORWARD,
        MOVE_BACKWARD,
        OBSTACLE_STOP,
        OBSTACLE_AVOIDANCE
    }

    public enum CameraTest
    {
        MOTION_DETECT,
        FACE_DETECT,
        SKELETON,
        HUMAN_DETECT,
        TAKEPHOTO
    }

    public class RunTestState : AStateMachineBehaviour
    {
        private Action<MoveTest> DoActionMove;
        private bool mIsFuncRunning;

        public override void Start()
        {
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mIsFuncRunning = false;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(!mIsFuncRunning && AutomatedTestData.Instance.TestOptions.Count != 0)
            {
                mIsFuncRunning = true;
                if (hasOption("Rotate"))
                {
                    DoActionMove = Move;
                    DoActionMove(MoveTest.ROTATE);

                }
            }

                
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        public bool hasOption(string searchItem)
        {
            // Search the list for any element that contains the searchItem
            return AutomatedTestData.Instance.TestOptions.Contains(searchItem);
        }

        private void Move(MoveTest iMovement)
        {
            Debug.Log("MOVE");



            AskRedo(Move);
        }

        private void CameraTest(CameraTest iCameraTest)
        {

        }

        private void AskRedo(Action<MoveTest> iFunc)
        {

            Buddy.Vocal.SayAndListen(
            Buddy.Resources.GetString("quit"),
            null,
            (iInput) => { OnEndListen(iInput); });

            DoActionMove = iFunc;
        }

        private void AskRedo(CameraTest iMoveTest)
        {

        }

        private void OnEndListen(SpeechInput iInput)
        {
            // If we clicked on the button yes we stopListen and it calls OnEndListen and iInput.IsInterrupted 
            // return true if we call Buddy.Vocal.StopListening()
            if (iInput.IsInterrupted)
                return;
            // We collect the human answer in Buddy.Vocal.LastHeardInput.Utterance and check if it
            // is one of the expected sentences in the dico
            if (Utils.ContainsOneOf(Buddy.Vocal.LastHeardInput.Utterance, "yes"))
            {

                // if the user says yes, we hide the toast and quit the app
                Buddy.GUI.Toaster.Hide();
                QuitApp();

            }
            else if (Utils.ContainsOneOf(Buddy.Vocal.LastHeardInput.Utterance, "no"))
            {

                // If the user says no, we hide the toast and get back to the menu
                Buddy.GUI.Toaster.Hide();
                Trigger("MenuTrigger");
            }
            else
            {
                //if (mNumberListen < MaxListenningIter)
                //{
                //    // if the human answer is outside of planned sentences, we increment the
                //    // number of listen and we listen again.
                //    mNumberListen++;
                //    Buddy.Vocal.Listen(
                //        iInputRec => { OnEndListen(iInputRec); }
                //        );
                //}
                //else
                //{
                //    // If we launch the listen too many times, it's like a timeout and
                //    // we get back to the menu
                //    Buddy.GUI.Toaster.Hide();
                //    Trigger("MenuTrigger");
                //}
            }
        }
    }
}

