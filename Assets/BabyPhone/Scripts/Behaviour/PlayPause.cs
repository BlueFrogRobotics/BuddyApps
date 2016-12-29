using UnityEngine;
using System.Collections.Generic;
using BuddyOS;

namespace BuddyApp.BabyPhone
{
    public class PlayPause : MonoBehaviour
    {
        [SerializeField]
        private GameObject stop;

        [SerializeField]
        private GameObject play;

        private Speaker mSpeaker;
        private AudioClip[] mLullabies;
        private List<string> mLullabyName;
        private int mLullabyIndice;
        private bool mIsVolumeOn;

        void OnEnable()
        {
            mSpeaker = BYOS.Instance.Speaker;
            mLullabies = Resources.LoadAll<AudioClip>("Sounds/Lullabies");
            mSpeaker.Media.Load("Sounds/Lullabies");
            mLullabyName = new List<string>();
            FillMusicName(mLullabyName, mLullabies);
            
            stop.SetActive(false);
            play.SetActive(true);
        }


        void Update()
        {
            mLullabyIndice = (int)BabyPhoneData.Instance.LullabyToPlay;
            mSpeaker.Media.Volume = ((BabyPhoneData.Instance.LullabyVolume)/100F);
            mIsVolumeOn = BabyPhoneData.Instance.IsVolumeOn;

            if (! mIsVolumeOn)
            {
                mSpeaker.Media.Stop();
                stop.SetActive(false);
                play.SetActive(true);
            }

        }

        public void PlayLullaby()
        {
            mSpeaker.Media.Loop = true;
            mSpeaker.Media.Play(mLullabies[mLullabyIndice]);

            stop.SetActive(true);
            play.SetActive(false);

        }

        public void StopLullaby()
        {
            mSpeaker.Media.Stop();

            play.SetActive(true);
            stop.SetActive(false);
        }

        private void FillMusicName(List<string> iMusicName, AudioClip[] iAudioCLip)
        {
            for (int i = 0; i < iAudioCLip.Length; ++i)
                iMusicName.Add("Lullabies/" + iAudioCLip[i].name.ToString());
        }
    }
}