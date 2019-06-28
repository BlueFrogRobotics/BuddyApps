using BlueQuark;

using UnityEngine;

using System;
using System.Collections;
using UnityEngine.Networking;

namespace BuddyApp.TestHybride
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class TestHybride : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private string mFreeSpeechCredentials;
        private bool mPendingRequest;
        private const string CREDENTIAL_DEFAULT_URL = "http://bfr-dev.azurewebsites.net/dev/BuddyDev-cmfc3b05c071.txt";

        /// <summary>
        /// When starting the app, Buddy says something then listen
        /// to the user
        /// </summary>
        /// <returns></returns>
        public IEnumerator Start()
        {

            Debug.LogWarning("Start testhynride");
            mPendingRequest = true;
            /*
			* You can setup your App activity here.
			*/
            TestHybrideActivity.Init(null);

            /*
			* Init your app data
			*/
            Buddy.Vocal.EnableTrigger = false;
            //BYOS.Instance.Interaction.VocalManager.OnError = OnError;
            //BYOS.Instance.Interaction.VocalManager.OnEndReco = OnSpeechRecognition;


            System.IO.File.WriteAllText(@"C:\Users\Public\test.txt", "test й а л л");


            Buddy.GUI.Toaster.Display<VerticalListToast>().With((iBuilder) => {
                TVerticalListBox lBox2 = iBuilder.CreateBox();
                lBox2.SetLabel("й о п ай и");
            });


            yield return StartCoroutine(RetrieveCredentialsAsync());
            Debug.Log("pre Say and listen");
            SayAndListen("ilisten");
            mPendingRequest = false;
        }

        /// <summary>
		/// To collect freespeech credentials
		/// </summary>
		/// <returns></returns>
		private IEnumerator RetrieveCredentialsAsync()
        {
            //OLD cred : http://bfr-dev.azurewebsites.net/dev/BuddyDev-mplqc5fk128f1.txt
            using (WWW lQuery = new WWW(CREDENTIAL_DEFAULT_URL)) {
                yield return lQuery;
                mFreeSpeechCredentials = lQuery.text;
            }
        }


        /// <summary>
        /// To simplify the call with credential
        /// </summary>
        /// <param name="iToSay"></param>
        /// <param name="iIsKey"></param>
        private void SayAndListen(string iToSay, bool iIsKey = true)
        {
            // if string is a key, get the value
            if (iIsKey)
                iToSay = Buddy.Resources.GetRandomString("ilisten");

            //Call the say and listen function with correct params
            Debug.Log("Say and Listen " + iToSay);
            Buddy.Vocal.SayAndListen(new SpeechOutput(iToSay), null, FreeSpeechResult, OnListeningStatus, new SpeechInputParameters() {
                Credentials = mFreeSpeechCredentials, // here your credentials
                Grammars = new string[] { "common" },
                RecognitionMode = SpeechRecognitionMode.GRAMMAR_THEN_FREESPEECH // Default mode is Grammar, you have to specify the free speech mode
            });
        }

        /// <summary>
        /// When we got a listening status, we can collect error of STT
        /// </summary>
        /// <param name="iSpeechInputStatus"></param>
        private void OnListeningStatus(SpeechInputStatus iSpeechInputStatus)
        {
            Debug.Log("Listening status");

            //if (iSpeechInputStatus.IsError) {
            //    Debug.Log("Listening status : " + iSpeechInputStatus.Type.ToString());
            //    // An error occured, probably network or credentials.
            //    // We need to signal it and then we will quit the app.
            //    Buddy.Behaviour.SetMood(Mood.SAD);
            //    Buddy.Vocal.SayKey("freespeecherror",
            //        // Quit app
            //        (iSpeechOutput) => { Buddy.Platform.Application.Stop(); });
            //} else
            // Something else happened, just print it...
            Debug.Log("Status: " + iSpeechInputStatus.Type.ToString());
        }

        private void FreeSpeechResult(SpeechInput iSpeechInput)
        {
            // Stop Coroutine if the vocal has stop because of end of users's speech
            //StopAllCoroutines();

            // set active Answer in Dialog
            Debug.LogWarning("Freespeech result " + iSpeechInput.Utterance);
            Debug.LogWarning("nb listeners " + Buddy.Vocal.OnEndListening.Count);

            string lRule = "";
            if (!string.IsNullOrEmpty(iSpeechInput.Rule))
                lRule = Utils.GetRealStartRule(iSpeechInput.Rule);
            SayAndListen("J'ai entendu " + iSpeechInput.Utterance + "avec la rиgle " + lRule, false);


        }


        //private void OnError(STTError iError)
        //{
        //    pendingRequest = false;
        //}

        //private void Update()
        //{
        //    if (!Buddy.Vocal.IsBusy && !string.IsNullOrEmpty(mFreeSpeechCredentials)) {
        //        SayAndListen("ilisten");
        //    }
        //}



        private void OnApplicationQuit()
        {
            // Should be done by default
            Buddy.Vocal.OnEndListening.Clear();
        }



    }
}