using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BlueQuark;

namespace BuddyApp.FreezeDance
{
    public class FDEndGame : AStateMachineBehaviour
    {
        private ScoreManager mScoreManager;
        private Ranking mRanking;        
        // Credentials for google freespeech
        private string mGoogleCredentials;
        private const int NBLISTENMAX = 3;
        private int mNbListen;
        private int mScore;
        public PlayerNamesData mPlayerNamesData;

        public override void Start()
        {
            mScoreManager = GetComponent<ScoreManager>();
            mRanking = GetComponent<Ranking>();
            StartCoroutine(RetrieveCredentialsAsync());

            string lang = Buddy.Platform.Language.OutputLanguage.ISO6391Code.ToString().ToLower();
            string playerFileName = Buddy.Resources.AppRawDataPath + "player_names_" + lang + ".xml";
            try
            {
                mPlayerNamesData = Utils.UnserializeXML<PlayerNamesData>(playerFileName);
                Debug.Log("FreezeDance: success parsing file " + playerFileName);
            }
            catch (NullReferenceException e)
            {
                Debug.LogError("FreezeDance: error parsing file " + playerFileName + " : " + e.Message);
            }
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mNbListen = 0;
            mScore = (int)mScoreManager.Score;
            string txt = string.Format(Buddy.Resources.GetRandomString("gameisover"), mScore);

            if (Buddy.WebServices.HasInternetAccess)
            {
                Buddy.Vocal.StopAndClear();
                Buddy.Vocal.OnEndListening.Clear();
                Buddy.Vocal.OnEndListening.Add(RegisterPlayerName);
                Buddy.Vocal.DefaultInputParameters = new SpeechInputParameters()
                {
                    Credentials = mGoogleCredentials,
                    RecognitionMode = SpeechRecognitionMode.FREESPEECH_ONLY
                };
                Buddy.Behaviour.SetMood(Mood.THINKING);
                txt += "[200]" + Buddy.Resources.GetRandomString("whatsyourname");
            }
            Buddy.Vocal.Say(txt, (iOutput) =>
            {
                if (Buddy.WebServices.HasInternetAccess)
                {
                    Buddy.Vocal.Listen();
                }
                else
                {
                    AddAnonymousPlayer();
                }
            });
        }

        private void AddAnonymousPlayer()
        {
            // Pick a rando name
            System.Random random = new System.Random();
            int lId = random.Next(mPlayerNamesData.Names.Count);
            string name = mPlayerNamesData.Names[lId];

            string txt = string.Format(Buddy.Resources.GetRandomString("givename"), name);
            Buddy.Vocal.Say(txt, (iOutput) =>
            {
                AddPlayer(name, mScore);
            });
        }

        private void AddPlayer(string name, int score)
        {
            mRanking.AddPlayer(mScore, name);

            Buddy.Behaviour.SetMood(Mood.HAPPY);
            string txt = string.Format(Buddy.Resources.GetRandomString("givescore"), name, score);
            Buddy.Vocal.Say(txt, NextStep);
        }

        private void NextStep(SpeechOutput iSpeech)
        {
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
            Trigger("Ranking");
        }

        private void RegisterPlayerName(SpeechInput iSpeech)
        {
            if (iSpeech.IsInterrupted || string.IsNullOrEmpty(iSpeech.Utterance))
            {
                if (mNbListen < NBLISTENMAX)
                {
                    mNbListen++;
                    Buddy.Vocal.Listen();
                }
                else
                {
                    AddAnonymousPlayer();
                }

                return;
            }
            Buddy.Vocal.StopAndClear();
            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
            string name = iSpeech.Utterance;
            AddPlayer(name, mScore);
        }

        /// <summary>
        /// To collect freespeech credentials
        /// </summary>
        /// <returns></returns>
        private IEnumerator RetrieveCredentialsAsync()
        {
            //OLD cred : http://bfr-dev.azurewebsites.net/dev/BuddyDev-mplqc5fk128f1.txt
            using (WWW lQuery = new WWW("http://bfr-dev.azurewebsites.net/dev/BuddyDev-cmfc3b05c071.txt"))
            {
                yield return lQuery;
                mGoogleCredentials = lQuery.text;
                Debug.Log("<color=red>CREDENTIAL : </color>" + mGoogleCredentials);
            }
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.Vocal.StopAndClear();
            Buddy.Vocal.OnEndListening.Clear();
            ResetTrigger("Ranking");
        }

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}
    }
}