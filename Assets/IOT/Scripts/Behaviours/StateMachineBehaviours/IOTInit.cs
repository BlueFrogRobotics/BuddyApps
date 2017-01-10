using UnityEngine;
using System.Collections;

namespace BuddyApp.IOT
{
    public class IOTInit : AIOTStateMachineBehaviours
    {

        public override void Init()
        {
            BuddyOS.BYOS.Instance.VocalManager.EnableTrigger = false;

            if (PlayerPrefs.GetString("somfy_user") != "") {
                IOTSomfy lSomfy = new IOTSomfy();
                lSomfy.Credentials[1] = PlayerPrefs.GetString("somfy_user");
                lSomfy.Credentials[2] = PlayerPrefs.GetString("somfy_password");

                lSomfy.ParamGO = GetGameObject(0).GetComponent<ParametersGameObjectContainer>();
                lSomfy.InitializeParams();

                lSomfy.Connect();

                GetGameObject(2).GetComponent<IOTList>().Objects.Add(lSomfy);
            }
            if (PlayerPrefs.GetString("philips_ip") != "") {
                IOTPhilipsHue lPhilips = new IOTPhilipsHue();
                lPhilips.Credentials[0] = PlayerPrefs.GetString("philips_ip");
                lPhilips.Credentials[1] = PlayerPrefs.GetString("philips_user");

                lPhilips.ParamGO = GetGameObject(0).GetComponent<ParametersGameObjectContainer>();
                lPhilips.InitializeParams();

                lPhilips.Connect();

                GetGameObject(2).GetComponent<IOTList>().Objects.Add(lPhilips);
            }
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {

        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
        }
    }
}
