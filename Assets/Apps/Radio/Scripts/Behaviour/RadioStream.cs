using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Web;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using BlueQuark;

namespace BuddyApp.Radio
{
    /// <summary>
    /// This class uses the radioline rest api to retreive informations on radio and get a working stream url
    /// It also play the radio stream using the android plugin (with exoplayer)
    /// link to the radioline doc: https://developers.radioline.co/client.php
    /// </summary>
    public sealed class RadioStream : MonoBehaviour
    {

        public enum ErrorState: int
        {
            NONE,
            INVALID_CLIENT,
            INVALID_OATH_REQUEST,
            RADIO_NOT_FOUND
        }

        //[SerializeField]
        //private RadioUIManager mRadioUI;

        /// <summary> 
        /// Client id to use the radioline api 
        /// Used to get get a token (the radioline api is protected by a OAuth2 authetification)
        /// </summary>
        private const string CLIENT_ID = "N2amcZh_-9LNt?X2=R;XWOfsZ8B@.;PwxozfLqoElol";

        public bool IsUpdatingLiveInfos { get; private set; }

        public ErrorState Error { get; private set; }

        private AndroidJavaObject currentActivity;

        /// <summary>
        /// Current streaming radio url
        /// </summary>
        private string mUrl = ""; 

        /// <summary>
        /// Token retreived
        /// </summary>
        //private string mToken = "";

        /// <summary>
        /// Current radio logo url
        /// </summary>
        private string mLogoUrl = "";

        private string mPermaLink = "";

        private string mRadioName = "";
        private string mShowDescription = "";

        private void Awake()
        {
            //RadioplayerActivity.Init(GetComponent<Animator>(), this);
        }

        // Use this for initialization
        void Start()
        {
            Error = ErrorState.NONE;
            //AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            //currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Retreive a token using OAuth authentification
        /// Needed to make get requests (with bearer authentification)
        /// Tokens expire after one day 
        /// </summary>
        public string GetToken()
        {
            string poststring = String.Format("client_id={0}&device_serial={1}&grant_type={2}", System.Uri.EscapeDataString(CLIENT_ID), "test", "password");
            string lPostResult = PostData("https://test.auth.radioline.fr/auth/token", poststring);
            if(lPostResult.Contains("invalid_client"))
                Error = ErrorState.INVALID_CLIENT;
            else
                Error = ErrorState.NONE;
            JSONNode lJsonNode = JSON.Parse(lPostResult);

            string lToken = lJsonNode["access_token"].Value;
            return lToken;
        }

        /// <summary>
        /// Play given radio name
        /// </summary>
        /// <param name="iName">radio name</param>
        public void Play(string iName, string lToken)
        {
            //Stop();
            
            //StartCoroutine(GetLiveInformations(iName, lToken));
            //StartCoroutine(PlayRadio(iName, lToken));

            //StartCoroutine(SearchRadioName(iName));
        }

        /// <summary>
        /// Stop playing stream audio
        /// </summary>
        public void Stop()
        {
            currentActivity.Call("stopStreamWithExo");
        }

        public void UpdateLiveInformations(string iToken)
        {
            //StartCoroutine(GetLiveInformations(mPermaLink, iToken));
        }

        public IEnumerator GetRadioInformations(string iRadioName, RadioInfos iInfos, string iToken)
        {
            string lRadioName = iRadioName.Trim().ToLower().Replace(" ", "_");
            RadioInfos lInfos = new RadioInfos();
            UnityWebRequest lWww = UnityWebRequest.Get("http://service.buddy.api.radioline.fr/Pillow/radios/" + lRadioName);
            //Send request
            lWww.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            lWww.SetRequestHeader("Authorization", "Bearer " + iToken);
            yield return lWww.SendWebRequest();

            if (!lWww.isNetworkError)
            {
                string lResultContent = lWww.downloadHandler.text;
                Debug.Log("le result des infos: " + lResultContent);
                if (lResultContent.Contains("Invalid OAuth request"))
                    Error = ErrorState.INVALID_OATH_REQUEST;
                else {
                    JSONNode lJsonNode = JSON.Parse(lResultContent);

                    if (lJsonNode["body"]["type"].Value == "error") {
                        Error = ErrorState.RADIO_NOT_FOUND;
                    } else {
                        Error = ErrorState.NONE;
                        string lPermalink = lJsonNode["body"]["content"]["permalink"].Value;
                        lPermalink = lPermalink.Replace("radios/", "");
                        iInfos.Permalink = lPermalink;
                        iInfos.Name = lJsonNode["body"]["content"]["name"].Value;
                        iInfos.LogoURL = lJsonNode["body"]["content"]["logo"].Value;
                        iInfos.Description = lJsonNode["body"]["content"]["description"].Value;
                        iInfos.Language = lJsonNode["body"]["content"]["language"].Value;
                        iInfos.Country = lJsonNode["body"]["content"]["country"].Value;
                    }
                }
            }
            else
            {

            }
            
        }

        /// <summary>
        /// Get and save a streaming radio url
        /// </summary>
        /// <param name="iRadioName">name of the radio</param>
        /// <returns></returns>
        public IEnumerator GetRadiosStreams(string iRadioName, string iToken, StreamList iStreams)
        {

            string lRadioName = iRadioName.Trim().ToLower().Replace(" ", "_");
            UnityWebRequest lWww = UnityWebRequest.Get("http://service.buddy.api.radioline.fr/Pillow/radios/" + lRadioName + "/play");
            //Send request
            lWww.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            lWww.SetRequestHeader("Authorization", "Bearer " + iToken);
            yield return lWww.SendWebRequest();
            iStreams.StreamLinks = new List<StreamLink>();

            if (!lWww.isNetworkError)
            {
                string lResultContent = lWww.downloadHandler.text;
                Debug.Log("le token: " + lResultContent);
                if (lResultContent.Contains("Invalid OAuth request"))
                    Error = ErrorState.INVALID_OATH_REQUEST;
                else {
                    JSONNode lJsonNode = JSON.Parse(lResultContent);
                    if (lJsonNode["body"]["type"].Value == "error") {
                        Error = ErrorState.RADIO_NOT_FOUND;
                    } else {
                        Error = ErrorState.NONE;
                        for (int i = 0; i < lJsonNode["body"]["content"]["streams"].Count; i++) {
                            StreamLink lLink = new StreamLink();
                            lLink.Url = lJsonNode["body"]["content"]["streams"][i]["url"].Value;
                            lLink.Protocol = lJsonNode["body"]["content"]["streams"][i]["protocol"].Value;
                            lLink.Format = lJsonNode["body"]["content"]["streams"][i]["format"].Value;
                            lLink.Codec = lJsonNode["body"]["content"]["streams"][i]["codec"].Value;
                            lLink.Port = lJsonNode["body"]["content"]["streams"][i]["port"].AsInt;
                            lLink.Frequency = lJsonNode["body"]["content"]["streams"][i]["frequency"].AsInt;
                            lLink.Bitrate = lJsonNode["body"]["content"]["streams"][i]["bitrate"].AsInt;
                            lLink.Channels = lJsonNode["body"]["content"]["streams"][i]["channels"].AsInt;
                            iStreams.StreamLinks.Add(lLink);
                        }


                    }
                }
            }
            else
            {

            }
        }

        /// <summary>
        /// Get the show name playing at the radio
        /// </summary>
        /// <param name="iRadioName">radio name</param>
        /// <returns></returns>
        public IEnumerator GetLiveInformations(string iRadioName, string iToken, ShowInfos iInfos)
        {
            IsUpdatingLiveInfos = true;
            string lRadioName = iRadioName.Trim().ToLower().Replace(" ", "_");
            UnityWebRequest lWww = UnityWebRequest.Get("http://service.buddy.api.radioline.fr/Pillow/radios/" + lRadioName + "/live");
            //Send request
            lWww.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            lWww.SetRequestHeader("Authorization", "Bearer " + iToken);
            yield return lWww.SendWebRequest();

            if (!lWww.isNetworkError)
            {
                string lResultContent = lWww.downloadHandler.text;
                Debug.Log("le live info: " + lResultContent);
                if (lResultContent.Contains("Invalid OAuth request"))
                    Error = ErrorState.INVALID_OATH_REQUEST;
                else {
                    JSONNode lJsonNode = JSON.Parse(lResultContent);
                    if (lJsonNode["body"]["type"] == "error") {
                        Error = ErrorState.RADIO_NOT_FOUND;
                    } else {
                        Error = ErrorState.NONE;
                        iInfos.Baseline = lJsonNode["body"]["content"]["show"]["name"].Value;
                        iInfos.Start = lJsonNode["body"]["content"]["show"]["start"].Value;
                        iInfos.End = lJsonNode["body"]["content"]["show"]["end"].Value;
                        iInfos.Logo = lJsonNode["body"]["content"]["show"]["logo"].Value;
                        if (lJsonNode["body"]["content"]["track"] != null)
                            iInfos.Song = lJsonNode["body"]["content"]["track"]["name"].Value;
                        if (lJsonNode["body"]["content"]["track"] != null)
                            iInfos.Singer = lJsonNode["body"]["content"]["track"]["artist"]["name"].Value;
                    }

                }
            }
            else
            {

            }
            IsUpdatingLiveInfos = false;
        }

        public IEnumerator SearchRadioName(string iRadioName, string iToken, RadioList iInfos)
        {
            string lRadioName = iRadioName.Trim().ToLower().Replace(" ", "_");
            UnityWebRequest lWww = UnityWebRequest.Get("http://service.buddy.api.radioline.fr/Pillow/search?query=" + System.Uri.EscapeDataString(iRadioName) + "&type=radio");
            //Send request
            lWww.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            lWww.SetRequestHeader("Authorization", "Bearer " + iToken);
            yield return lWww.SendWebRequest();
            iInfos.Radios = new List<RadioInfos>();
            if (!lWww.isNetworkError)
            {
                string lResultContent = lWww.downloadHandler.text;
                if (lResultContent.Contains("Invalid OAuth request"))
                    Error = ErrorState.INVALID_OATH_REQUEST;
                else {
                    JSONNode lJsonNode = JSON.Parse(lResultContent);
                    if (lJsonNode["body"]["type"] == "error") {
                        Error = ErrorState.RADIO_NOT_FOUND;
                    } else {
                        Error = ErrorState.NONE;
                        for (int i = 0; i < lJsonNode["body"]["content"].Count; i++) {
                            RadioInfos lInfo = new RadioInfos();
                            string lPermalink = lJsonNode["body"]["content"][i]["permalink"].Value;
                            lPermalink = lPermalink.Replace("radios/", "");
                            lInfo.Permalink = lPermalink;
                            lInfo.Name = lJsonNode["body"]["content"][i]["name"].Value;
                            lInfo.LogoURL = lJsonNode["body"]["content"][i]["logo"].Value;
                            lInfo.Description = lJsonNode["body"]["content"][i]["description"].Value;
                            lInfo.Language = lJsonNode["body"]["content"][i]["language"].Value;
                            lInfo.Country = lJsonNode["body"]["content"][i]["country"].Value;
                            iInfos.Radios.Add(lInfo);
                        }
                    }
                }
                
            }
            else
            {

            }
        }

        /// <summary>
        /// Make a post request to given url with the given parameter
        /// Uses the radioline certificate (in p12 format)
        /// </summary>
        /// <param name="iURL">url to make the post request</param>
        /// <param name="iPostString">parameters in the format of key-value</param>
        /// <returns></returns>
        private string PostData(string iURL, string iPostString)
        {
            String lResult = "";

            Debug.Log("poststring: \n" + iPostString);
            Stream lWriter = null;

            byte[] lBytedata = System.Text.Encoding.UTF8.GetBytes(iPostString);

            HttpWebRequest lObjRequest = (HttpWebRequest)WebRequest.Create(iURL);
            lObjRequest.Method = "POST";
            lObjRequest.ContentLength = lBytedata.Length;
            lObjRequest.ContentType = "application/x-www-form-urlencoded";
            lObjRequest.ProtocolVersion = HttpVersion.Version11;

            lObjRequest.UseDefaultCredentials = true;
            lObjRequest.Credentials = CredentialCache.DefaultCredentials;
            lObjRequest.ClientCertificates.Add(new X509Certificate2(Buddy.Resources.GetRawFullPath("service.buddy.79.0.p12"), "@7Cs=\\ze?plT"));

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
                lWriter = lObjRequest.GetRequestStream();
                lWriter.Write(lBytedata, 0, lBytedata.Length);
                lWriter.Flush();
                lWriter.Close();
                HttpWebResponse objResponse = (HttpWebResponse)lObjRequest.GetResponse();
                using (StreamReader sr =
                new StreamReader(objResponse.GetResponseStream()))
                {
                    lResult = sr.ReadToEnd();

                    // Close and clean up the StreamReader
                    sr.Close();
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse objResponse = (HttpWebResponse)ex.Response;
                using (StreamReader sr =
               new StreamReader(objResponse.GetResponseStream()))
                {
                    lResult = sr.ReadToEnd();

                    // Close and clean up the StreamReader
                    sr.Close();
                }
            }
            finally
            {

            }

            Debug.Log("le result: " + lResult);

            return lResult;
        }



        private bool MyRemoteCertificateValidationCallback(System.Object sender,
    X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;
            // If there are errors in the certificate chain,
            // look at each error to determine the cause.
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                for (int i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status == X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        continue;
                    }
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                    X509Certificate2 signerCert2 = new X509Certificate2((Buddy.Resources.GetRawFullPath("ca.sha2.pem")));

                    chain.ChainPolicy.ExtraStore.Add(signerCert2);
                    bool chainIsValid = chain.Build((X509Certificate2)certificate);
                    if (!chainIsValid)
                    {
                        isOk = false;
                        break;
                    }
                }
            }
            return isOk;
        }


        ///// <summary>
        ///// Start playing given radio 
        ///// </summary>
        ///// <param name="iRadioName">radio name</param>
        ///// <returns></returns>
        //private IEnumerator PlayRadio(string iRadioName, string lToken)
        //{
        //    yield return StartCoroutine(GetRadios(iRadioName, lToken));
        //    if (mUrl != "error")
        //        currentActivity.Call("playStreamWithExo", mUrl);
        //}



    }
}