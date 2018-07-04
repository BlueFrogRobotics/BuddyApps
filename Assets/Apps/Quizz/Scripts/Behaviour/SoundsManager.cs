using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.Quizz
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundsManager : MonoBehaviour
    {

        public enum Sound : int
        {
            QUIZZ_BUZZER = 0,
            GOOD_ANSWER = 1,
            BAD_ANSWER = 2,
            END_GAME = 3,
            COMPUTING = 4
        }

        public bool IsPlaying { get { return mAudioSource.isPlaying; } }

        [SerializeField]
        private AudioClip quizzBuzzerSound;

        [SerializeField]
        private AudioClip goodAnswerSound;

        [SerializeField]
        private AudioClip badAnswerSound;

        [SerializeField]
        private AudioClip endGameSound;

        [SerializeField]
        private AudioClip computingSound;

        private AudioSource mAudioSource;

        // Use this for initialization
        void Start()
        {
            mAudioSource = GetComponent<AudioSource>();
        }

        public void PlaySound(Sound iSound)
        {
            switch(iSound)
            {
                case Sound.QUIZZ_BUZZER:
                    mAudioSource.clip = quizzBuzzerSound;
                    mAudioSource.Play();
                    break;
                case Sound.GOOD_ANSWER:
                    mAudioSource.clip = goodAnswerSound;
                    mAudioSource.Play();
                    break;
                case Sound.BAD_ANSWER:
                    mAudioSource.clip = badAnswerSound;
                    mAudioSource.Play();
                    break;
                case Sound.END_GAME:
                    mAudioSource.clip = endGameSound;
                    mAudioSource.Play();
                    break;
                case Sound.COMPUTING:
                    mAudioSource.clip = computingSound;
                    mAudioSource.Play();
                    break;
                default:
                    break;
            }
        }

        public void Stop()
        {
            mAudioSource.Stop();
        }
    }
}