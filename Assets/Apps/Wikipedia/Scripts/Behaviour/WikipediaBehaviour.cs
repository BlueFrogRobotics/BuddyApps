using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System.Text;
using System.Net;

namespace BuddyApp.Wikipedia
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class WikipediaBehaviour : MonoBehaviour
    {
        //Max listen before quitting the app
        private int mMaxListen;
        private int mNumberOfListen;

        // Credentials for google freespeech
        private string mGoogleCredentials;

        /// <summary>
        /// When starting the app, Buddy says something then listen
        /// to the user
        /// </summary>
        /// <returns></returns>
        public IEnumerator Start()
        {
            mMaxListen = 4;
            mNumberOfListen = 0;
            // We get the credentials for google freespeech before
            // listening.

            if (!string.IsNullOrEmpty(WikipediaData.Instance.Utterance)) {
                string lDefinition = ExtractDefinition(WikipediaData.Instance.Utterance);
                if (string.IsNullOrEmpty(lDefinition)) {
                    Debug.LogWarning("Couldn't find definition to look for");
                    SayAndListen("ilisten");
                } else {
                    // We start a coroutine to make the request
                    Buddy.Behaviour.SetMood(Mood.THINKING);
                    StartCoroutine(BuildDefinitionAnswer(lDefinition));
                }
            } else {
                yield return StartCoroutine(RetrieveCredentialsAsync());
                //Debug.Log("Say and listen");
                SayAndListen("ilisten");
            }
        }


        /******************************************************************/
        // VOCAL FUNCTIONS /////////////////////////////////////////////////
        /******************************************************************/

        /// <summary>
        /// This function is call when STT has finished
        /// </summary>
        /// <param name="iSpeechInput"></param>
        private void OnEndListening(SpeechInput iSpeechInput)
        {
            Debug.Log("OnEndListening");
            if (string.IsNullOrEmpty(iSpeechInput.Utterance)) {
                Debug.Log("ON END LISTENING : ISPEECHINPUT NULL OU EMPTY");
                // We didn't get the answer, we listen again.
                SayAndListen("ilisten");
                return;
            } else {
                string lDefinition = "";
                lDefinition = ExtractDefinition(iSpeechInput.Utterance);
                if (string.IsNullOrEmpty(lDefinition)) {
                    // We didn't get the definition, we listen again.
                    SayAndListen("ilisten");
                } else {
                    // We start a coroutine to make the request
                    Buddy.Behaviour.SetMood(Mood.THINKING);
                    StartCoroutine(BuildDefinitionAnswer(lDefinition));
                }
            }
        }

        /// <summary>
        /// When we got a listening status, we can collect error of STT
        /// </summary>
        /// <param name="iSpeechInputStatus"></param>
        private void OnListeningStatus(SpeechInputStatus iSpeechInputStatus)
        {
            Debug.Log("Listening status");

            if (iSpeechInputStatus.IsError) {
                Debug.Log("Listening status : " + iSpeechInputStatus.Type.ToString());
                // An error occured, probably network or credentials.
                // We need to signal it and then we will quit the app.
                Buddy.Behaviour.SetMood(Mood.SAD);
                Buddy.Vocal.SayKey("freespeecherror",
                    // Quit app
                    (iSpeechOutput) => { Buddy.Platform.Application.Stop(); });
            } else
                // Something else happened, just print it...
                Debug.Log("Status: " + iSpeechInputStatus.Type.ToString());
        }
        /******************************************************************/


        /******************** Hand made simple NLP ************************/
        /// <summary>
        /// This function gets the definition to look for
        /// </summary>
        /// <param name="iUtterance"></param>
        /// <returns></returns>
        private string ExtractDefinition(string iUtterance)
        {
            Debug.Log("ExtractDefinition");

            if (Utils.ContainsOneOf(iUtterance, "definitionof"))
                return GetWordsAfter(iUtterance, "definitionof");

            else if (Utils.ContainsOneOf(iUtterance, "whois"))
                return GetWordsAfter(iUtterance, "whois");

            return "";
        }


        /// <summary>
        /// Extract words after key words iKeyWords
        /// </summary>
        /// <param name="iUtterance"> (Speech) sentence from the user </param>
        /// <param name="iKeyWords"> Key words from which we will extract next words </param>
        /// <returns></returns>
        private string GetWordsAfter(string iUtterance, string iKeyWords)
        {
            string lToBeSearched = "";
            for (int i = 0; i < Buddy.Resources.GetPhoneticStrings(iKeyWords).Length; ++i) {
                lToBeSearched = Buddy.Resources.GetPhoneticStrings(iKeyWords)[i];
                int lIndex = iUtterance.IndexOf(lToBeSearched);

                if (lIndex != -1) {
                    return iUtterance.Substring(lIndex + lToBeSearched.Length);
                }
            }
            return "";
        }


        /********************************************************************************/

        /// <summary>
        /// This function collects the answer from wikipedia, 
        /// verbalise the answer
        /// and listen next question
        /// </summary>
        /// <param name="iDefinition"></param>
        /// <returns></returns>
        private IEnumerator BuildDefinitionAnswer(string iDefinition)
        {
            if (!string.IsNullOrEmpty(iDefinition)) {

                string lRequestUriString = "";
                // check language to request the correct wikipedia webpage
                if (Buddy.Platform.Language.OutputLanguage == Buddy.Platform.Language.GetLanguageFromISOCode(ISO6391Code.FR))
                    lRequestUriString = string.Format("https://fr.wikipedia.org/w/api.php?format=json&action=opensearch&search="
                        + Uri.EscapeUriString(iDefinition) + "&limit=1&profile=fuzzy");
                else
                    lRequestUriString = string.Format("https://en.wikipedia.org/w/api.php?format=json&action=opensearch&search="
                        + Uri.EscapeUriString(iDefinition) + "&limit=1&profile=fuzzy");

                Debug.Log(lRequestUriString);

                using (UnityWebRequest lRequest = UnityWebRequest.Get(lRequestUriString)) {
                    yield return lRequest.SendWebRequest();


                    if (lRequest.isNetworkError) // Error
                    {
                        Debug.Log("We had an error in the request" + lRequest.error);
                        Buddy.Behaviour.SetMood(Mood.SAD);
                    } else // Success
                      {

                        Buddy.Behaviour.SetMood(Mood.NEUTRAL);
                        Debug.LogWarning("ANSWER: " + lRequest.downloadHandler.text);

                        string lRes = "";

                        // To handle French special char
                        //string[] lFragmant = UnescapeHex(lRequest.downloadHandler.text).Split('"');

                        string[] lFragmant = Regex.Unescape(lRequest.downloadHandler.text).Split('"');


                        // Get the correct answer from wikipedia
                        for (int i = 0; i < lFragmant.Length; ++i) {
                            if (lFragmant[i] == "],[") {
                                lRes = lFragmant[i + 1];
                                Debug.Log(lRes);
                            } else if (lFragmant[i].Contains("https://")) {
                                Debug.LogWarning("page url " + lFragmant[i]);
                                StartCoroutine(DisplayImageUrl(lFragmant[i]));
                                break;
                            }
                        }

                        if (string.IsNullOrEmpty(lRes)) {
                            // What to say when we got no answer?
                            lRes = Buddy.Resources.GetRandomString("noanswer").Replace("[definition]", iDefinition);
                            SayAndListen(lRes, false);
                        } else {
                            // Remove phonetics
                            string lRegex = "(\\[.*\\])";
                            lRes = Regex.Replace(lRes, lRegex, "");

                            if (!string.IsNullOrEmpty(WikipediaData.Instance.Utterance))
                                Buddy.Vocal.Say(lRes, (iSpeechOutput) => {
                                    Buddy.GUI.Toaster.Hide();
                                    Buddy.Platform.Application.Stop();
                                });
                            else
                                SayAndListen(lRes, false);
                        }

                    }
                }
            }
        }

        /// <summary>
        /// To handle french caracters
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string UnescapeHex(string data)
        {
            return Encoding.UTF8.GetString(Array.ConvertAll(Regex.Unescape(data).ToCharArray(), c => (byte)c));
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
            Buddy.Vocal.SayAndListen(new SpeechOutput(iToSay), (iOutput) =>
                                    Buddy.GUI.Toaster.Hide(), OnEndListening, OnListeningStatus,
                                    new SpeechInputParameters() {
                                        Credentials = mGoogleCredentials, // here your credentials
                                        RecognitionMode = SpeechRecognitionMode.FREESPEECH_ONLY // Default mode is Grammar, you have to specify the free speech mode
                                    });
        }

        /// <summary>
        /// To collect freespeech credentials
        /// </summary>
        /// <returns></returns>
        private IEnumerator RetrieveCredentialsAsync()
        {
            //OLD cred : http://bfr-dev.azurewebsites.net/dev/BuddyDev-mplqc5fk128f1.txt
            using (WWW lQuery = new WWW("http://bfr-dev.azurewebsites.net/dev/BuddyDev-cmfc3b05c071.txt")) {
                yield return lQuery;
                mGoogleCredentials = lQuery.text;
                Debug.Log("<color=red>CREDENTIAL : </color>" + mGoogleCredentials);
            }
        }

        private IEnumerator DisplayImageUrl(string iUrl)
        {

            Dictionary<string, string> lHeaders = new Dictionary<string, string>();
            lHeaders.Add("Accept", "text/html, application/xhtml+xml, */*");
            lHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko");

            WWW lWww = new WWW(iUrl, null, lHeaders);

            Debug.LogWarning("pre yield ");
            yield return lWww;

            string lOutput = lWww.text;
            string lImgUrl = GetUrlImageFromWikiPage(lOutput);

            Debug.LogWarning("image url " + lImgUrl);

            if (!string.IsNullOrEmpty(lImgUrl))
                StartCoroutine(DisplayImage(lImgUrl));
        }

        private IEnumerator DisplayImage(string iImgUrl)
        {
            // Display random image

            using (WWW www = new WWW(iImgUrl)) {
                // Wait for download to complete
                yield return www;

                // assign texture
                Texture2D lTexture = www.texture;

                //if (www.texture.height < 250 || www.texture.width < 250) {
                //    //GET ANOTHER IMAGE
                //    iImgUrls.RemoveAt(lRand);
                //    StartCoroutine(DisplayRandomImage(iImgUrls, iRequest));
                //} else {
                Debug.LogWarning("size " + lTexture.width + "x" + lTexture.height);
                Sprite lSprite = Sprite.Create(lTexture, new Rect(0.0f, 0.0f, lTexture.width, lTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
                Buddy.GUI.Toaster.Display<PictureToast>().With(lSprite);

                //}
            }
        }


        private string GetUrlImageFromWikiPage(string iUrl)
        {
            int lIndex = iUrl.IndexOf("og:image", StringComparison.Ordinal);
            Debug.LogWarning("index found is " + lIndex);

            if (lIndex >= 0) {
                lIndex = iUrl.IndexOf("\"", lIndex + 12, StringComparison.Ordinal);
                lIndex++;
                int lIndex2 = iUrl.IndexOf("\"", lIndex, StringComparison.Ordinal);
                return iUrl.Substring(lIndex, lIndex2 - lIndex);

            } else
                return "";
        }

    }
}