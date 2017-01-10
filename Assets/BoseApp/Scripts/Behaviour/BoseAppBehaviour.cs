using UnityEngine;
using BuddyOS;
using UnityEngine.UI;

namespace BuddyApp.BoseApp
{
    public class BoseAppBehaviour : MonoBehaviour
    {
        [SerializeField]
        private Recast recast;

        [SerializeField]
        private Bose bose;

        [SerializeField]
        private BoseRecast boseRecast;

        private TextToSpeech mTextToSpeech;
        private SpeechToText mSTT;
        private BoseAppData mAppData;
        private VocalManager mVocalManager;
        private bool mStartSpeech = false;

        void Start()
        {
            mTextToSpeech = BYOS.Instance.TextToSpeech;
            mSTT = BYOS.Instance.SpeechToText;
            mVocalManager = BYOS.Instance.VocalManager;
            mVocalManager.OnEndReco = AnswerTextEvent;

            mAppData = BoseAppData.Instance;

            mSTT.OnError.Add(ErrorSpeech);

            this.updateData();
        }

        void Update()
        {
            this.updateData();

            this.recastLoop();
            this.sTTLoop();
        }

        private void updateData()
        {
            recast.setToken(mAppData.recastToken);
            recast.setLangage(mAppData.recastLangage);

            bose.setAddrBose(mAppData.boseAddr);

            bose.setPlaylist1(mAppData.playlistOne);
            bose.setPlaylist2(mAppData.playlistTwo);
            bose.setPlaylist3(mAppData.playlistThree);
            bose.setPlaylist4(mAppData.playlistFour);
            bose.setPlaylist5(mAppData.playlistFive);
            bose.setPlaylist6(mAppData.playlistSix);

        }

        public void sTTLoop()
        {
            //if (this.sTT.HasFinished)
            //    this.startSpeech = false;
            if (!this.mStartSpeech && !this.mTextToSpeech.IsSpeaking)
            {
                this.mStartSpeech = true;
                mVocalManager.StartInstantReco();
            }
        }

        public void recastLoop()
        {
            string lSlug;

            if (recast.hasAnswered())
            {
                lSlug = recast.getSlug();
                if (lSlug != null)
                {
                    boseRecast.execute(lSlug, recast.getEntities());
                }
                this.mStartSpeech = false;
            }
        }

        public void AnswerTextEvent(string iMsg)
        {
            if (iMsg != null && !iMsg.Equals(""))
                this.recast.send(iMsg);
            else
                this.mStartSpeech = false;
        }

        public void ErrorSpeech(string error)
        {
            this.mStartSpeech = false;
        }
    }
}