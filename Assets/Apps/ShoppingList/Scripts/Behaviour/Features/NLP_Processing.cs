using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
//using System.Threading.Tasks;
//using System.Web;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
//using System.Windows;

namespace BuddyApp.ShoppingList
{
    public static class NLP_Processing
    {

        private static string wit_ai_access_token = "JZFCVMIZ3S7YTWCULH3LCQO5AQSJLV6Q";


        public static string ProcessWrittenText(string text)
        {
            return ProcessText(text);
        }

        public static string ProcessSpokenText(string file)
        {

            return ProcessSpeech(file);
        }

        // Send the text to the wit.ai API
        private static string ProcessText(string text)
        {
            // 20140401 tells the wit.ai engine which version to use, in this case the last version
            // before April 1st, 2014
            string url = "https://api.wit.ai/message?v=20170406&q=" + text;

            WebRequest request = WebRequest.Create(url);

            request.Method = "GET";
            request.Headers["Authorization"] = "Bearer " + wit_ai_access_token;
            ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);

            string serverResponse = reader.ReadToEnd();

            return serverResponse;
        }

        // Send the wav file to the wit.ai API
        private static string ProcessSpeech(string file)
        {
            FileStream filestream = new FileStream(file, FileMode.Open, FileAccess.Read);
            BinaryReader filereader = new BinaryReader(filestream);
            byte[] BA_AudioFile = filereader.ReadBytes((Int32)filestream.Length);
            filestream.Close();
            filereader.Close();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.wit.ai/speech");

            request.Method = "POST";
            request.Headers["Authorization"] = "Bearer " + wit_ai_access_token;
            request.ContentType = "audio/wav";
            request.ContentLength = BA_AudioFile.Length;
            request.GetRequestStream().Write(BA_AudioFile, 0, BA_AudioFile.Length);

            // Delete the temp file
            /*try
            {
                File.Delete(file);
            }
            catch
            {
                UnityEngine.Debug.Log("Unable to delete the temp file!" + Environment.NewLine + "Please do so yourself: " + file);
            }*/

            // Process the wit.ai response
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader response_stream = new StreamReader(response.GetResponseStream());
                    return response_stream.ReadToEnd();
                }
                else
                {
                    return "Error: " + response.StatusCode.ToString();
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        public static bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;
            // If there are errors in the certificate chain, look at each error to determine the cause.
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                for (int i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        bool chainIsValid = chain.Build((X509Certificate2)certificate);
                        if (!chainIsValid)
                        {
                            isOk = false;
                        }
                    }
                }
            }
            return isOk;
        }
    }
}

