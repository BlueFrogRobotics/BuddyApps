using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.ExperienceCenter
{
    public class ECInitState : StateMachineBehaviour
    {

        private TcpServer mTcpServer;

        private static bool FirstRun = true;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("[EXCENTER] on state ECInitState");
            if (FirstRun)
            {
                ExperienceCenterData.Instance.StatusTcp = "Offline";
                ExperienceCenterData.Instance.IPAddress = "-";
                ExperienceCenterData.Instance.StopDistance = 0.4F;
                ExperienceCenterData.Instance.NoiseTime = 0.2F;
                ExperienceCenterData.Instance.TableDistance = 0.4F;
                ExperienceCenterData.Instance.IOTDistance = 1.5F;
                ExperienceCenterData.Instance.NoHingeAngle = 60F;
                ExperienceCenterData.Instance.NoHingeSpeed = 120F;
                ExperienceCenterData.Instance.HeadPoseTolerance = 0.2F;
                ExperienceCenterData.Instance.WelcomeTimeOut = 2F;
                ExperienceCenterData.Instance.MoveTimeOut = 5F;
                ExperienceCenterData.Instance.MaxDistance = 1F;
                ExperienceCenterData.Instance.MinDistance = 0.7F;
                ExperienceCenterData.Instance.MaxSpeed = 200F;
                ExperienceCenterData.Instance.MinSpeed = -200F;
                ExperienceCenterData.Instance.EnableHeadMovement = true;
                ExperienceCenterData.Instance.EnableBaseMovement = true;
                ExperienceCenterData.Instance.VoiceTrigger = true;
                ExperienceCenterData.Instance.EnableBML = true;
                ExperienceCenterData.Instance.CollisionDebug = false;
                ExperienceCenterData.Instance.DanceDuration = 15.0f;
                FirstRun = false;
            }

            mTcpServer = GameObject.Find("AIBehaviour").GetComponent<TcpServer>();
            mTcpServer.Init();

            Buddy.Behaviour.Mood = Mood.NEUTRAL;

            Buddy.GUI.Header.DisplayParametersButton(false);
        }
    }
}
