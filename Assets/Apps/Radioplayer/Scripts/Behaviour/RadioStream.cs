﻿using System.Collections;
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
using Buddy;

namespace BuddyApp.Radioplayer
{
    /// <summary>
    /// This class uses the radioline rest api to retreive informations on radio and get a working stream url
    /// It also play the radio stream using the android plugin (with exoplayer)
    /// link to the radioline doc: https://developers.radioline.co/client.php
    /// </summary>
    public class RadioStream : MonoBehaviour
    {

        [SerializeField]
        private RadioUIManager mRadioUI;

        /// <summary>
        /// Client id to use the radioline api 
        /// Used to get get a token (the radioline api is protected by a OAuth2 authetification)
        /// </summary>
        private const string CLIENT_ID = "N2amcZh_-9LNt?X2=R;XWOfsZ8B@.;PwxozfLqoE";

        private AndroidJavaObject currentActivity;

        /// <summary>
        /// Current streaming radio url
        /// </summary>
        private string mUrl = "";

        /// <summary>
        /// Token retreived
        /// </summary>
        private string mToken = "";

        /// <summary>
        /// Current radio logo url
        /// </summary>
        private string mLogoUrl = "";

        private void Awake()
        {
            RadioplayerActivity.Init(GetComponent<Animator>(), this);
        }

        // Use this for initialization
        void Start()
        {
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");    

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
        public void GetToken()
        {
            string poststring = String.Format("client_id={0}&device_serial={1}&grant_type={2}", System.Uri.EscapeDataString(CLIENT_ID), "test", "password");
            string lPostResult = PostData("https://test.auth.radioline.fr/auth/token", poststring);
            mToken = GetToken(lPostResult);
        }

        /// <summary>
        /// Play given radio name
        /// </summary>
        /// <param name="iName">radio name</param>
        public void Play(string iName)
        {
            //Stop();

            StartCoroutine(GetRadioInformations(iName));
            StartCoroutine(GetLiveInformations(iName));
            StartCoroutine(PlayRadio(iName));
        }

        /// <summary>
        /// Play the radio entered in the inputfield
        /// </summary>
        public void Play()
        {
            //Stop();
            string lName = mRadioUI.RadioNameSearched;
            RadioplayerData.Instance.DefaultRadio = lName;
            Play(lName);

        }

        /// <summary>
        /// Stop playing stream audio
        /// </summary>
        public void Stop()
        {
            currentActivity.Call("stopStreamWithExo");
        }

        /// <summary>
        /// Get and save a streaming radio url
        /// </summary>
        /// <param name="iRadioName">name of the radio</param>
        /// <returns></returns>
        private IEnumerator GetRadios(string iRadioName)
        {

            string lRadioName = iRadioName.Trim().ToLower().Replace(" ", "_");
            UnityWebRequest lWww = UnityWebRequest.Get("http://service.buddy.api.radioline.fr/Pillow/radios/"+ lRadioName +"/play");
            //Send request
            lWww.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            lWww.SetRequestHeader("Authorization", "Bearer " + mToken);
            yield return lWww.Send();

            if (!lWww.isError)
            {
                string resultContent = lWww.downloadHandler.text;
                Debug.Log("le token: " + resultContent);
                mUrl= GetLink(resultContent);

            }
            else
            {

            }
        }

        /// <summary>
        /// Get and transmit the official name and logo radio url to RadioUIManager
        /// </summary>
        /// <param name="iRadioName">name of the radio</param>
        /// <returns></returns>
        private IEnumerator GetRadioInformations(string iRadioName)
        {

            string lRadioName = iRadioName.Trim().ToLower().Replace(" ", "_");
            UnityWebRequest www = UnityWebRequest.Get("http://service.buddy.api.radioline.fr/Pillow/radios/" + lRadioName);
            //Send request
            www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            www.SetRequestHeader("Authorization", "Bearer " + mToken);
            yield return www.Send();

            if (!www.isError)
            {
                string resultContent = www.downloadHandler.text;
                Debug.Log("le token: " + resultContent);
                mLogoUrl = GetInformations(resultContent);
                mRadioUI.SetPictureFromUrl(mLogoUrl);
                mRadioUI.SetRadioName(GetRadioName(resultContent));
                //StartCoroutine(LoadPicture(mLogoUrl));

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
        private IEnumerator GetLiveInformations(string iRadioName)
        {
            string lRadioName = iRadioName.Trim().ToLower().Replace(" ", "_");
            UnityWebRequest www = UnityWebRequest.Get("http://service.buddy.api.radioline.fr/Pillow/radios/" + lRadioName+"/live");
            //Send request
            www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            www.SetRequestHeader("Authorization", "Bearer " + mToken);
            yield return www.Send();

            if (!www.isError)
            {
                string resultContent = www.downloadHandler.text;
                Debug.Log("le live info: " + resultContent);
                mRadioUI.SetShowDescription(GetShowDescription(resultContent));
                //showDescription.text = GetShowDescription(resultContent);


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
            String result = "";

            Debug.Log("poststring: \n" + iPostString);
            Stream myWriter = null;

            byte[] bytedata = System.Text.Encoding.UTF8.GetBytes(iPostString);

            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(iURL);
            objRequest.Method = "POST";
            objRequest.ContentLength = bytedata.Length;
            objRequest.ContentType = "application/x-www-form-urlencoded";
            objRequest.ProtocolVersion = HttpVersion.Version11;

            objRequest.UseDefaultCredentials = true;
            objRequest.Credentials = CredentialCache.DefaultCredentials;
            objRequest.ClientCertificates.Add(new X509Certificate2(BYOS.Instance.Resources.GetPathToRaw("service.buddy.79.0.p12"), "@7Cs=\\ze?plT"));
           
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
                myWriter = objRequest.GetRequestStream();
                myWriter.Write(bytedata, 0, bytedata.Length);
                myWriter.Flush();
                myWriter.Close();
                HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
                using (StreamReader sr =
                new StreamReader(objResponse.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                    
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
                    result = sr.ReadToEnd();

                    // Close and clean up the StreamReader
                    sr.Close();
                }
            }
            finally
            {
                
            }
            
            Debug.Log("le result: " + result);
            
            return result;
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
                    X509Certificate2 signerCert2 = new X509Certificate2((BYOS.Instance.Resources.GetPathToRaw("ca.sha2.pem")));

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

        /// <summary>
        /// Extract the token from given json
        /// </summary>
        /// <param name="iJson">json retreived by the post request</param>
        /// <returns></returns>
        private string GetToken(string iJson)
        {
            string lToken = "";

            JSONNode lJsonNode = Buddy.JSON.Parse(iJson);
            
            lToken = lJsonNode["access_token"].Value;
            return lToken;
            
        }

        /// <summary>
        /// Extract the streaming url from given json
        /// </summary>
        /// <param name="iJson">json retreived from get request</param>
        /// <returns></returns>
        private string GetLink(string iJson)
        {
            string lLink = "";

            JSONNode lJsonNode = Buddy.JSON.Parse(iJson);
            if (lJsonNode["body"]["type"] == "error")
            {
                return "error";
            }
            else
            {
                lLink = lJsonNode["body"]["content"]["streams"][0]["url"].Value;
                return lLink;
            }
        }

        /// <summary>
        /// Extract the radio logo url from given json
        /// </summary>
        /// <param name="iJson"></param>
        /// <returns>json retreived from get request</returns>
        private string GetInformations(string iJson)
        {
            string lLink = "";

            JSONNode lJsonNode = Buddy.JSON.Parse(iJson);
            if (lJsonNode["body"]["type"] == "error")
            {
                return "error";
            }
            else
            {
                lLink = lJsonNode["body"]["content"]["logo"].Value;
                return lLink;
            }
        }

        /// <summary>
        /// Extract the radio name from given json
        /// </summary>
        /// <param name="iJson">json retreived from get request</param>
        /// <returns></returns>
        private string GetRadioName(string iJson)
        {
            string lName = "";

            JSONNode lJsonNode = Buddy.JSON.Parse(iJson);
            if (lJsonNode["body"]["type"] == "error")
            {
                return "error";
            }
            else
            {
                lName = lJsonNode["body"]["content"]["name"].Value;
                return lName;
            }
        }

        /// <summary>
        /// Extract the actual radio show playing from given json
        /// </summary>
        /// <param name="iJson"></param>
        /// <returns>json retreived from get request</returns>
        private string GetShowDescription(string iJson)
        {
            string lDescription = "";

            JSONNode lJsonNode = Buddy.JSON.Parse(iJson);
            if (lJsonNode["body"]["type"] == "error")
            {
                return "error";
            }
            else
            {
                lDescription = lJsonNode["body"]["content"]["show"]["name"].Value;
                return lDescription;
            }
        }

        

        

        /// <summary>
        /// Start playing given radio 
        /// </summary>
        /// <param name="iRadioName">radio name</param>
        /// <returns></returns>
        private IEnumerator PlayRadio(string iRadioName)
        {
            yield return StartCoroutine(GetRadios(iRadioName));
            if(mUrl!="error")
                currentActivity.Call("playStreamWithExo", mUrl);
        }



    }
}