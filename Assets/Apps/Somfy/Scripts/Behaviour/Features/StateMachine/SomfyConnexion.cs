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
            StartCoroutine(Login());
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
      
        }

        private IEnumerator Login()
        {
            GetComponent<SomfyBehaviour>().Login();

            yield return new WaitForSeconds(1);
            yield return GetComponent<SomfyBehaviour>().ConnectTheDevices();
            yield return new WaitForSeconds(1);
            GetComponent<SomfyBehaviour>().GetDevices();
            Trigger("NextStep");
        }
    }
}