using BlueQuark;

using UnityEngine;

using System;
using System.Collections;
using UnityEngine.Networking;

namespace BuddyApp.WolframAlpha
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class WolframAlphaBehaviour : MonoBehaviour
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
            Debug.LogWarning("Start Wolfram");
            mPendingRequest = true;
            /*
			* You can setup your App activity here.
			*/
            WolframAlphaActivity.Init(null);

            /*
			* Init your app data
			*/
            Buddy.Vocal.EnableTrigger = false;
            //BYOS.Instance.Interaction.VocalManager.OnError = OnError;
            //BYOS.Instance.Interaction.VocalManager.OnEndReco = OnSpeechRecognition;

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
                RecognitionMode = SpeechRecognitionMode.FREESPEECH_ONLY // Default mode is Grammar, you have to specify the free speech mode
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

            if (iSpeechInput.Utterance.ToLower() == "quit") {
                Buddy.Behaviour.SetMood(Mood.NEUTRAL);
                Buddy.Platform.Application.Stop();
            } else if (!string.IsNullOrEmpty(iSpeechInput.Utterance)) {
                Buddy.Behaviour.SetMood(Mood.THINKING);
                mPendingRequest = true;
                StartCoroutine(RequestWolfram(iSpeechInput.Utterance));
            } else if (!Buddy.Vocal.IsBusy) {
                SayAndListen("ilisten");
            } else {
                Debug.Log("Vocal still busy... " + Buddy.Vocal.OnEndListening.Count);
            }

        }


        //private void OnError(STTError iError)
        //{
        //    pendingRequest = false;
        //}

        private void Update()
        {
            if (!Buddy.Vocal.IsBusy && !mPendingRequest && !string.IsNullOrEmpty(mFreeSpeechCredentials)) {
                SayAndListen("ilisten");
            }

        }


        IEnumerator RequestWolfram(String iQuestion)
        {
            if (!string.IsNullOrEmpty(iQuestion)) {

                //var requestUriString = string.Format("https://api.wolframalpha.com/v1/result?i={0}%3F&appid=Y5P9Q6-84A9H28PKT", Uri.EscapeUriString(iQuestion));
                var requestUriString = string.Format("http://api.wolframalpha.com/v1/conversation.jsp?appid=XJHHH8-TT59GH9JX8&i={0}", Uri.EscapeUriString(iQuestion));
                Debug.Log(requestUriString);


                using (UnityWebRequest request = UnityWebRequest.Get(requestUriString)) {
                    yield return request.SendWebRequest();

                    if (request.isNetworkError) // Error
                    {
                        Buddy.Behaviour.SetMood(Mood.SAD);
                        Debug.LogWarning(request.error);
                        SayAndListen("ilisten");
                    } else // Success
                      {
                        Buddy.Behaviour.SetMood(Mood.NEUTRAL);
                        Debug.LogWarning(iQuestion + " GOT ANSWER: " + request.downloadHandler.text);

                        string lRes = "";
                        string[] lFragmant = request.downloadHandler.text.Split('"');

                        for (int i = 0; i < lFragmant.Length; ++i) {
                            if (lFragmant[i] == "result") {
                                lRes = lFragmant[i + 2];
                                Debug.Log(lRes);
                                break;
                            }
                        }

                        if (!string.IsNullOrEmpty(lRes)) {
                            lRes = lRes.Replace(@"\", "");
                            lRes = lRes.Replace("Wolfram|Alpha did not understand your input", "Sorry, I didn't find an answer...");

                            lRes = lRes.Replace("an empty list", "Sorry, I have no answer to your question...");
                            lRes = lRes.Replace("Wolfram|Alpha", "I");
                            lRes = lRes.Replace("Wolfram Alpha", "Buddy");
                            lRes = lRes.Replace("No short answer available", "I didn't find any answer...");
                        } else
                            lRes = "I didn't find any answer...";

                        SayAndListen(lRes, false);
                    }

                    //ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;


                }
            } else
                SayAndListen("ilisten");

            mPendingRequest = false;
        }

        private void OnApplicationQuit()
        {
            // Should be done by default
            Buddy.Vocal.OnEndListening.Clear();
        }



    }
}