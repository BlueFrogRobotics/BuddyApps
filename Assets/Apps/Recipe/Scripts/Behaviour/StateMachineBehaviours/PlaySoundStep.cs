using UnityEngine;
using Buddy;

namespace BuddyApp.Recipe
{
    public class PlaySoundStep : AStateMachineBehaviour
    {
        public override void Start()
        {
        }

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("play sound step");
            switch (UnityEngine.Random.Range(0, 10))
            {
                case 0:
                    Primitive.Speaker.Voice.Play(VoiceSound.LAUGH_1);
                    break;
                case 1:
                    Primitive.Speaker.Voice.Play(VoiceSound.LAUGH_2);
                    break;
                case 2:
                    Primitive.Speaker.Voice.Play(VoiceSound.LAUGH_3);
                    break;
                case 3:
                    Primitive.Speaker.Voice.Play(VoiceSound.LAUGH_4);
                    break;
                case 4:
                    Primitive.Speaker.Voice.Play(VoiceSound.LAUGH_5);
                    break;
                case 5:
                    Primitive.Speaker.Voice.Play(VoiceSound.SURPRISED_1);
                    break;
                case 6:
                    Primitive.Speaker.Voice.Play(VoiceSound.SURPRISED_2);
                    break;
                case 7:
                    Primitive.Speaker.Voice.Play(VoiceSound.SURPRISED_3);
                    break;
                case 8:
                    Primitive.Speaker.Voice.Play(VoiceSound.SURPRISED_4);
                    break;
                case 9:
                    Primitive.Speaker.Voice.Play(VoiceSound.SURPRISED_5);
                    break;
                case 10:
                    Primitive.Speaker.Voice.Play(VoiceSound.SURPRISED_6);
                    break;
                default:
                    Primitive.Speaker.Voice.Play(VoiceSound.LAUGH_1);
                    break;
            }
        }

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (Primitive.Speaker.Voice.Status == SoundChannelStatus.FINISH)
                GetComponent<Animator>().SetTrigger("DisplayStep");
        }

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }
    }
}