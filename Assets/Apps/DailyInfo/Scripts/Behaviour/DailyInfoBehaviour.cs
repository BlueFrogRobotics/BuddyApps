using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.DailyInfo
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class DailyInfoBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Return the actual language abreviation in lower case
        /// </summary>
        //public string Lang { get { return Buddy.Platform.Language.OutputLanguage.ISO6391Code.ToString().ToLower(); } }
        public string Lang { get { return "fr"; } } 

        /// <summary>
        /// List of infos requested
        /// </summary>
        public List<InfoData> InfosData { get; private set; }

        /// <summary>
        /// List of icons displayed in the menu
        /// </summary>
        public List<string> Icons { get; private set; }

        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Date of the request
        /// </summary>
        public DateTime RequestedDate { get; private set; }

        /// <summary>
        /// Date of the request
        /// </summary>
        public string DateStr { get; private set; }

        /*
         * Data of the application. Save on disc when app is quitted
         */
        private DailyInfoData mAppData;

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			DailyInfoActivity.Init(null);

            /*
			* Init your app data
			*/
            mAppData = DailyInfoData.Instance;

            InfosData = new List<InfoData>();
            Icons = null;
            Title = mAppData.InfosFileName;
            RequestedDate = DateTime.Today;

            string infoFileName = Buddy.Resources.AppRawDataPath + mAppData.InfosFileName + "_" + Lang + ".xml";
            InfosData allInfos = null;
            try
            {
                allInfos = Utils.UnserializeXML<InfosData>(infoFileName);

                if (allInfos.Icons != null)
                    Icons = allInfos.Icons;

                if (allInfos.Title != null)
                    Title = allInfos.Title;

            }
            catch (NullReferenceException e)
            {
                Debug.LogError("DailyInfo: error parsing file " + infoFileName + " : " + e.Message);
            }

            if (allInfos.Infos.Count > 0)
            {
                ExtractDate(mAppData.VocalRequest);

                // If request starts with "when" or "where" search for the info in the info list
                if (ContainsOneOf(mAppData.VocalRequest, Buddy.Resources.GetPhoneticStrings("when"))
                    || ContainsOneOf(mAppData.VocalRequest, Buddy.Resources.GetPhoneticStrings("where")))
                {
                    foreach (InfoData data in allInfos.Infos)
                    {
                        foreach (string item in data.Items)
                        {
                            if (mAppData.VocalRequest.ToLower().Contains(item.ToLower()))
                            {
                                InfosData.Add(data);

                                // Set Day for speech
                                int nbDeltaDays = (int)(data.Day.Date - DateTime.Now.Date).TotalDays;
                                switch (nbDeltaDays)
                                {
                                    case 0: DateStr = Buddy.Resources.GetString("today");break;
                                    case 1: DateStr = Buddy.Resources.GetString("tomorrow");break;
                                    default:DateStr = Buddy.Resources.GetString(data.Day.DayOfWeek.ToString().ToLower()); break;
                                }

                                break;
                            }
                        }
                        if (InfosData.Count > 0)
                            break;
                    }
                }

                if (InfosData.Count == 0)
                {
                    string dayPart = null;
                    foreach (InfoData data in allInfos.Infos)
                    {
                        if (data.Day == RequestedDate
                            && !string.IsNullOrEmpty(data.DayPart)
                            && mAppData.VocalRequest.Contains(data.DayPart))
                        {
                            dayPart = data.DayPart;
                            break;
                        }
                    }
                    foreach (InfoData data in allInfos.Infos)
                    {
                        if (data.Day == RequestedDate
                            && (string.IsNullOrEmpty(dayPart)
                            || (data.DayPart == dayPart)))
                            InfosData.Add(data);
                    }
                    Debug.Log("Nb info data : " + InfosData.Count);
                }
            }
        }

        /// <summary>Extracts the date from the vocal request.</summary>
        /// <param name="vocalRequest">The vocal request.</param>
        private void ExtractDate(string vocalRequest)
        {
            int nbDeltaDays = 0;
            // If no date is mentionned today's info is displayed
            DateStr = Buddy.Resources.GetString("today");

            if (vocalRequest.Contains(Buddy.Resources.GetString("tomorrow")))
            {
                DateStr = Buddy.Resources.GetString("tomorrow");
                nbDeltaDays = 1;
            }
            else if (vocalRequest.Contains(Buddy.Resources.GetString("dayaftertomorrow")))
            {
                DateStr = Buddy.Resources.GetString("dayaftertomorrow");
                nbDeltaDays = 2;
            }
            else
            {
                // Day of week in vocal request
                for (int i = 0; i < 7; i++)
                {
                    DayOfWeek d = (DayOfWeek)i;
                    if (vocalRequest.Contains(Buddy.Resources.GetString(d.ToString().ToLower())))
                    {
                        int dnow = (int)DateTime.Now.DayOfWeek;
                        nbDeltaDays = (i >= dnow) ? i - dnow : (7 + i) - dnow;
                        DateStr = Buddy.Resources.GetString(d.ToString().ToLower());
                    }
                }

            }
            RequestedDate = DateTime.Today.AddDays(nbDeltaDays);
        }

        /// <summary>Determines whether one of the string in a list is contained in a text.</summary>
        /// <param name="text">The text.</param>
        /// <param name="stringList">The list of strings.</param>
        private bool ContainsOneOf(string text, string[] stringList)
        {
            text = text.ToLower();
            for (int i = 0; i < stringList.Length; ++i)
            {
                string[] words = stringList[i].Split(' ');
                if (words.Length < 2)
                {
                    words = text.Split(' ');
                    foreach (string word in words)
                    {
                        if (word == stringList[i].ToLower())
                        {
                            return true;
                        }
                    }
                }
                else if (text.ToLower().Contains(stringList[i].ToLower()))
                    return true;
            }
            return false;
        }
    }
}