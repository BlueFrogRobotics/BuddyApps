﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.IOT
{
    public class IOTTriggerFromGameObject : AIOTStateMachineBehaviours
    {
        public enum EnterOrExit { ONENTER, ONEXIT };

        [SerializeField]
        private EnterOrExit when = EnterOrExit.ONENTER;
        [SerializeField]
        private string trigger;
        [SerializeField]
        private List<int> gameobject = new List<int>();

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
            if (when == EnterOrExit.ONENTER)
            {
                for (int i = 0; i < gameobject.Count; ++i)
                {
                    GameObject lGO = GetGameObject(gameobject[i]);
                    Animator lAnim = lGO.GetComponent<Animator>();
                    if (lAnim != null)
                    {
                        if (trigger == "")
                        {
                            string[] lName = lGO.name.Split('_');
                            string lWinName = lName[lName.Length - 1];
                            lAnim.SetTrigger("Open_W" + lWinName);
                        }
                        else
                            lAnim.SetTrigger(trigger);
                    }
                }
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
            if (when == EnterOrExit.ONEXIT)
            {
                for (int i = 0; i < gameobject.Count; ++i)
                {
                    GameObject lGO = GetGameObject(gameobject[i]);
                    Animator lAnim = lGO.GetComponent<Animator>();
                    if (lAnim != null)
                    {
                        if (trigger == "")
                        {
                            string[] lName = lGO.name.Split('_');
                            string lWinName = lName[lName.Length - 1];
                            lAnim.SetTrigger("Close_W" + lWinName);
                        }
                        else
                            lAnim.SetTrigger(trigger);
                    }
                }
            }
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
        }
    }
}
