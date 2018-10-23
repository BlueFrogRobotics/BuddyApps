using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.Somfy
{
    public class SomfyConnexion : AStateMachineBehaviour
    {
        //private SomfyLayout mLayout;

        public override void Start()
        {
            //mLayout = new SomfyLayout();
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //Toaster.Display<ParameterToast>().With(mLayout, () => { Login(); }, null);
            Login();
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
      
        }

        private void Login()
        {
            GetComponent<SomfyBehaviour>().Login();
            Trigger("NextStep");
        }
    }
}