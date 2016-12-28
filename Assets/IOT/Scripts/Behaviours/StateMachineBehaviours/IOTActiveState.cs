using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace BuddyApp.IOT
{
    public class IOTActiveState : AIOTStateMachineBehaviours
    {
        public enum EnterOrExit { ONENTER, ONEXIT};

        [SerializeField]
        private EnterOrExit when = EnterOrExit.ONENTER;
        [SerializeField]
        private bool clearOnBestReco = false;
        [SerializeField]
        private bool setActive = false;
        [SerializeField]
        private List<int> gameobject = new List<int>();

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
            if(when == EnterOrExit.ONENTER)
            {
                for(int i = 0; i < gameobject.Count; ++i)
                {
                    GameObject lGO = GetGameObject(gameobject[i]);
                    lGO.SetActive(setActive);
                }
                StartCoroutine(OpenObject());
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
                        if (clearOnBestReco)
                            mSTT.OnBestRecognition.Clear();
                        string[] lName = lGO.name.Split('_');
                        string lWinName = lName[lName.Length - 1];
                        lAnim.SetTrigger("Close_W" + lWinName);
                    }
                }
                StartCoroutine(CloseObject());
            }
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
        }

        IEnumerator CloseObject()
        {
            yield return new WaitForSeconds(0.8F);

            for (int i = 0; i < gameobject.Count; ++i)
            {
                GetGameObject(gameobject[i]).SetActive(setActive);
            }
        }

        IEnumerator OpenObject()
        {
            yield return new WaitForSeconds(0.8F);

            for (int i = 0; i < gameobject.Count; ++i)
            {
                GameObject lGO = GetGameObject(gameobject[i]);
                Animator lAnim = lGO.GetComponent<Animator>();
                if (lAnim != null)
                {
                    string[] lName = lGO.name.Split('_');
                    string lWinName = lName[lName.Length - 1];
                    lAnim.SetTrigger("Open_W" + lWinName);
                }
            }
        }
    }
}
