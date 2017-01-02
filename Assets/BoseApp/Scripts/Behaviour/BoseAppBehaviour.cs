using UnityEngine;
using BuddyOS;
using UnityEngine.UI;

namespace BuddyApp.BoseApp
{
    public class BoseAppBehaviour : MonoBehaviour
    {
        private TextToSpeech mTextToSpeech;
        private SpeechToText sTT;

        private BoseAppData mAppData;

        [SerializeField]
        private Recast recast;

        [SerializeField]
        private Bose bose;

        [SerializeField]
        private BoseRecast boseRecast;

        private VocalActivation mVocalActivation;
        private bool startSpeech = false;

        void Start()
        {
            mTextToSpeech = BYOS.Instance.TextToSpeech;
            sTT = BYOS.Instance.SpeechToText;
            mVocalActivation = BYOS.Instance.VocalActivation;
            mVocalActivation.VocalProcessing = AnswerTextEvent;

            mAppData = BoseAppData.Instance;

            sTT.OnError.Add(ErrorSpeech);

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
            if (!this.startSpeech && !this.mTextToSpeech.IsSpeaking)
            {
                this.startSpeech = true;
                mVocalActivation.StartInstantReco();
            }
        }

        public void recastLoop()
        {
            string slug;

            if (recast.hasAnswered())
            {
                slug = recast.getSlug();
                if (slug != null)
                {
                    boseRecast.execute(slug, recast.getEntities());
                }
                this.startSpeech = false;
            }
        }

        public void AnswerTextEvent(string iMsg)
        {
            if (iMsg != null && !iMsg.Equals(""))
                this.recast.send(iMsg);
            else
                this.startSpeech = false;
        }

        public void ErrorSpeech(string error)
        {
            this.startSpeech = false;
        }

    }
}