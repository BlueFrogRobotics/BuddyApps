using UnityEngine;
using System.Collections;

namespace BuddyApp.IOT
{
    public class IOTLinkManager : MonoBehaviour
    {
        private Animator mAnimator;
        // Use this for initialization
        void Awake()
        {
            mAnimator = GetComponent<Animator>();

            foreach (AIOTStateMachineBehaviours lBehaviours in mAnimator.GetBehaviours<AIOTStateMachineBehaviours>())
            {
                for(int i = 0; i < mAnimator.parameterCount; ++i)
                    lBehaviours.HashList.Add(mAnimator.GetParameter(i).nameHash);
            }
        }

        public void setTriggerChoice(int iChoice)
        {
            mAnimator.SetInteger("Choice", iChoice);
        }

        public void Quit()
        {
            BuddyOS.BYOS.Instance.AppManager.Quit();
        }

        public void OpenMenu()
        {
            BuddyOS.BYOS.Instance.MenuPanel.LaunchMenu();
        }
    }
}
