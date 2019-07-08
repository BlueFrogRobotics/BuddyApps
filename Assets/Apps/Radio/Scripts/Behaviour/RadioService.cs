using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Radio
{
    /* 
     * RadioService manages the radio webservice
     */
    public class RadioService: MonoBehaviour
    {
        private string mPermalink;
        private string mStreamLink;
        private bool mProcess;

        private float mTime;

        void Awake()
        {
            mProcess = false;
            mTime = 0.0F;
            mPermalink = "";
        }

        private void Start()
        {
            // Set header font
            Font lHeaderFont = Buddy.Resources.Get<Font>("os_awesome");
            lHeaderFont.material.color = Color.black;
            Buddy.GUI.Header.SetCustomLightTitle(lHeaderFont);
        }

        /* 
         * Start the radio
         * */
        public void PlayRadio()
        {
            if (!string.IsNullOrEmpty(RadioData.Instance.DefaultRadio))
            {
                StartCoroutine(Play(RadioData.Instance.DefaultRadio));
            }
        }

        /* 
         * Stop the radio
         * */
        public void StopRadio()
        {
            Buddy.WebServices.Radio.Stop();
        }

        /* 
         * Update the displayed title according to the current radio and program
         */
        void Update()
        {
            // Update title
            mTime += Time.deltaTime;
            if (mTime > 20.0F && !string.IsNullOrEmpty(mPermalink))
            {
                Buddy.WebServices.Radio.GetCurrentProgram(mPermalink, ShowProgram, OnError);
                mTime = 0.0F;
            }
        }

        /*
         * Display station's name in the title + the name of the song if available, or the name of the program
         */
        private void ShowProgram(RadioStationProgram iProgram)
        {
            string subtitle = "";

            if (!string.IsNullOrEmpty(iProgram.Singer))
                subtitle = iProgram.Singer;

            if (!string.IsNullOrEmpty(iProgram.Song))
            {
                if (!string.IsNullOrEmpty(subtitle))
                    subtitle += " - ";
                subtitle += iProgram.Song;
            }

            if (string.IsNullOrEmpty(subtitle)
                && !string.IsNullOrEmpty(iProgram.Name))
                subtitle = iProgram.Name;

            string title = RadioData.Instance.DefaultRadio.ToUpper();
            if (!string.IsNullOrEmpty(subtitle))
                title += "\n" + subtitle;

            Buddy.GUI.Header.DisplayLightTitle(title);
        }

        /*
         * Launch the radio
         */
        private IEnumerator Play(string iName)
        {
            mProcess = true;
            mPermalink = "";

            Buddy.WebServices.Radio.SearchStations(iName, GetFirstLink, OnError);
            while (string.IsNullOrEmpty(mPermalink))
                yield return null;

            if (mPermalink != null)
            {
                mStreamLink = null;
                Buddy.WebServices.Radio.GetStreams(mPermalink, GetFirstStream, OnError);
                while (string.IsNullOrEmpty(mStreamLink))
                    yield return null;

                Buddy.WebServices.Radio.Play(mStreamLink);
                // Display title
                Buddy.WebServices.Radio.GetCurrentProgram(mPermalink, ShowProgram, OnError);
            }
            mProcess = false;
        }

        /*
         * Show error in log
         */
        private void OnError(RadioError iError)
        {
            Debug.LogError("Error connecting to radio API " + iError);
        }

        /*
         * Get first link from radio api results
         */
        private void GetFirstLink(RadioStation[] iStations)
        {
            if (iStations != null && iStations.Length > 0)
            {
                mPermalink = iStations[0].Permalink;
            }
            else
                mPermalink = null;
        }


        /*
         * Get first stream from radio api results
         */
        private void GetFirstStream(RadioStationStream[] iStreams)
        {
            if (iStreams != null && iStreams.Length > 0)
                mStreamLink = iStreams[0].Url;
        }
    }
}