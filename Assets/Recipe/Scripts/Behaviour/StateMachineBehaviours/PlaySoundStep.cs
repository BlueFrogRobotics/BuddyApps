using UnityEngine;
using BuddyOS.App;
using BuddyOS;

namespace BuddyApp.Recipe
{
    public class PlaySoundStep : AStateMachineBehaviour
    {
        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            switch (UnityEngine.Random.Range(0, 10))
            {
                case 0:
                    BYOS.Instance.Speaker.Voice.Play(VoiceSound.LAUGH_1);
                    break;
                case 1:
                    BYOS.Instance.Speaker.Voice.Play(VoiceSound.LAUGH_2);
                    break;
                case 2:
                    BYOS.Instance.Speaker.Voice.Play(VoiceSound.LAUGH_3);
                    break;
                case 3:
                    BYOS.Instance.Speaker.Voice.Play(VoiceSound.LAUGH_4);
                    break;
                case 4:
                    BYOS.Instance.Speaker.Voice.Play(VoiceSound.LAUGH_5);
                    break;
                case 5:
                    BYOS.Instance.Speaker.Voice.Play(VoiceSound.SURPRISED_1);
                    break;
                case 6:
                    BYOS.Instance.Speaker.Voice.Play(VoiceSound.SURPRISED_2);
                    break;
                case 7:
                    BYOS.Instance.Speaker.Voice.Play(VoiceSound.SURPRISED_3);
                    break;
                case 8:
                    BYOS.Instance.Speaker.Voice.Play(VoiceSound.SURPRISED_4);
                    break;
                case 9:
                    BYOS.Instance.Speaker.Voice.Play(VoiceSound.SURPRISED_5);
                    break;
                case 10:
                    BYOS.Instance.Speaker.Voice.Play(VoiceSound.SURPRISED_6);
                    break;
                default:
                    BYOS.Instance.Speaker.Voice.Play(VoiceSound.LAUGH_1);
                    break;
            }
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mSpeaker.Voice.Status == SoundChannelStatus.FINISH)
                GetComponent<Animator>().SetTrigger("DisplayStep");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }
    }
}