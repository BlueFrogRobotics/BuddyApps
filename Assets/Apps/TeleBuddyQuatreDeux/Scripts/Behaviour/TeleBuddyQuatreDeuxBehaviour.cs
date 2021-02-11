using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BuddyApp.TeleBuddyQuatreDeux
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class TeleBuddyQuatreDeuxBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private TeleBuddyQuatreDeuxData mAppData;


        private RTMManager mRTMManager;
        private RTCManager mRTCManager;

        //public const string APP_ID = "dc949460a57e4fb0990a219b799ccf13";
        public const string APP_ID = "7b13f4916a6b43e0b23958b18926d596";

        private void Awake()
        {
            mRTMManager = GetComponent<RTMManager>();
            mRTCManager = GetComponent<RTCManager>();

            TeleBuddyQuatreDeuxActivity.Init(null, mRTMManager, mRTCManager);
        }

        void Start()
        {
            mAppData = TeleBuddyQuatreDeuxData.Instance;
        }


        public static string EncodeToSHA256(string iInput)
        {
            using (System.Security.Cryptography.SHA256 lSha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(iInput);
                byte[] hashBytes = lSha256.ComputeHash(inputBytes);
                
                byte[] lBytes = lSha256.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < lBytes.Length; i++)
                {
                    sb.Append(lBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        public static string EncodeToMD5(string iInput)
        {
            using (System.Security.Cryptography.MD5 lMd5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(iInput);
                byte[] hashBytes = lMd5.ComputeHash(inputBytes);

                byte[] lBytes = lMd5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < lBytes.Length; i++)
                {
                    sb.Append(lBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

    }
}