using Buddy;
using Buddy.UI;
using Buddy.Command;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
	public class ProposeGame : AStateMachineBehaviour
	{
		private bool mNoGame;

		private float mTime;

		private List<string> mKeyOptions;

        private string mProposal = "";

        private TextToSpeech mTTS = BYOS.Instance.Interaction.TextToSpeech;


        public override void Start()
		{
			mState = GetComponentInGameObject<Text>(0);
			mDetectionManager = GetComponent<DetectionManager>();
			mActionManager = GetComponent<ActionManager>();
			mKeyOptions = new List<string>();
			mKeyOptions.Add("memory");
			mKeyOptions.Add("playmath");
			mKeyOptions.Add("freezedance");
			mKeyOptions.Add("rlgl");

		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
            mProposal = RandomProposal(mKeyOptions);

            mDetectionManager.mDetectedElement = Detected.NONE;
            mState.text = "Propose Game";
            mTime = 0F;
			mNoGame = false;
            Interaction.Mood.Set(MoodType.HAPPY);

            mTTS.Say(Dictionary.GetRandomString("attention") + " "+ Dictionary.GetRandomString(mProposal + "propose"));

            Toaster.Display<BinaryQuestionToast>().With(Dictionary.GetRandomString(mProposal), YesAnswer, NoAnswer);
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mTime += Time.deltaTime;

			if (mTime > 60F || mNoGame) {
				iAnimator.SetTrigger("ASKNEWRQ");
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mDetectionManager.mDetectedElement = Detected.NONE;
		}

        private void YesAnswer ()
        {
            mTTS.Say(Dictionary.GetRandomString("herewego"));
            OnAnswer(mProposal);
        }

        private void NoAnswer()
        {
            mTTS.Say(Dictionary.GetRandomString("nopb"));
            OnAnswer("nogame");
        }

        private string RandomProposal(List<string> iProp)
        {
            string  lProp = "";
            int lRdmOne = UnityEngine.Random.Range(1, 4);

            switch (lRdmOne) {
                case 1:
                    lProp = "joke";
                    break;
                case 2:
                    lProp = "dance";
                    break;
                case 3:
                    lProp = iProp[UnityEngine.Random.Range(1, iProp.Count)] ;
                    break;
            }
            return lProp;
        }



		void OnAnswer(string iAnswer)
		{
            switch (iAnswer) {
				case "playmath":
					CompanionData.Instance.InteractDesire -= 50;
					new StartAppCmd("PlayMath").Execute();
					break;

				case "freezedance":
					CompanionData.Instance.InteractDesire -= 50;
					new StartAppCmd("FreezeDanceApp").Execute();
					break;

				case "rlgl":
					CompanionData.Instance.InteractDesire -= 50;
					new StartAppCmd("RLGLApp").Execute();
					break;

				case "memory":
					CompanionData.Instance.InteractDesire -= 50;
					new StartAppCmd("MemoryGameApp").Execute();
					break;
                case "jokes":
                    CompanionData.Instance.InteractDesire -= 30;
                    Interaction.BMLManager.LaunchRandom("joke");
                    break;
                case "dance":
                    CompanionData.Instance.InteractDesire -= 30;
                    Interaction.BMLManager.LaunchRandom("dance");
                    break;
                case "nogame":
					CompanionData.Instance.InteractDesire += 10;
					mNoGame = true;
					break;
			}
		}
	}
}