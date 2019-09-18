using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

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

        public bool IsPlaying { get { return Buddy.Actuators.Speakers.Media.IsBusy; } }

        private AudioClip mQuizzBuzzerSound;

        private AudioClip mGoodAnswerSound;

        private AudioClip mBadAnswerSound;

        private AudioClip mEndGameSound;

        private AudioClip mComputingSound;

        // Use this for initialization
        void Start()
        {
            mQuizzBuzzerSound = Buddy.Resources.Get<AudioClip>("quizz_buzzer.wav");
            mGoodAnswerSound = Buddy.Resources.Get<AudioClip>("good_answer.wav");
            mBadAnswerSound = Buddy.Resources.Get<AudioClip>("bad_answer.wav");
            mEndGameSound = Buddy.Resources.Get<AudioClip>("end_game.wav");
            mComputingSound = Buddy.Resources.Get<AudioClip>("computing.wav");
        }

        public void PlaySound(Sound iSound)
        {
            switch(iSound)
            {
                case Sound.QUIZZ_BUZZER:
                    Buddy.Actuators.Speakers.Media.Play(mQuizzBuzzerSound);
                    break;
                case Sound.GOOD_ANSWER:
                    Buddy.Actuators.Speakers.Media.Play(mGoodAnswerSound);
                    break;
                case Sound.BAD_ANSWER:
                    Buddy.Actuators.Speakers.Media.Play(mBadAnswerSound);
                    break;
                case Sound.END_GAME:
                    Buddy.Actuators.Speakers.Media.Play(mEndGameSound);
                    break;
                case Sound.COMPUTING:
                    Buddy.Actuators.Speakers.Media.Play(mComputingSound);
                    break;
                default:
                    break;
            }
        }

        public void Stop()
        {
            Buddy.Actuators.Speakers.Media.Stop();
        }
    }
}