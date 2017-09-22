using System.Collections;
using System.Collections.Generic;
using System;
using System.Globalization;
using UnityEngine;
using Buddy;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace BuddyApp.Reminder
{
    public class Reminder
    {
        [XmlAttribute("addDate")]
        public DateTime AddDate { get; set; }

        [XmlAttribute("remindDate")]
        public DateTime RemindDate { get; set; }

        [XmlAttribute("title")]
        public string Title { get; set; }

        [XmlAttribute("content")]
        public string Content { get; set; }

        [XmlAttribute("receiver")]
        public string Receiver { get; set; }
    }

    public class ReminderContent
    {
        [XmlElement("reminderList")]
        public List<Reminder> reminderList { get; set; }

        public ReminderContent()
        {
            reminderList = new List<Reminder>();
        }

    }

    public class ReminderFilter
    {
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string Receiver { get; set; }
    }

    public class ReminderManager : MonoBehaviour
    {

        public string CommandText { get; set; }
        public Command Command { get; set; }
        public ReminderContent ReminderContent { get; set; }
        public List<ReminderUI> ReminderUIList { get; set; }
        public List<DateUI> DateUIList { get; set; }
        public IProcessVocal ProcessVocal { get; set; }

        void Awake()
        {
            ReminderUIList = new List<ReminderUI>();
            DateUIList = new List<DateUI>();
            ProcessVocal = new ProcessVocalManual();
        }

        // Use this for initialization
        void Start()
        {
            CommandText = "";
            UnSerializeList();
            string sdate = "2017-06-09T16:00:00.000+02:00";

            DateTime lDate = DateTime.ParseExact(sdate, "yyyy-MM-dd'T'HH:mm:ss.fffzzz", CultureInfo.InvariantCulture,
                                       DateTimeStyles.AssumeUniversal |
                                       DateTimeStyles.AdjustToUniversal);
            Debug.Log("!!!!!!!!!!!!!!!!!date de now: " + lDate.ToString());
            //ProcessVocalManual lProcess= new ProcessVocalManual();
            //lProcess.ExtractDate("va voir goronou demain");
            //lProcess.ExtractDate("va voir goronou jeudi à 13 h");
            //lProcess.ExtractDate("va voir goronou dans 5 jours");
            //lProcess.ExtractDate("va voir goronou la semaine prochaine");
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SerializeList()
        {
            string lPath = BYOS.Instance.Resources.PathToRaw("reminder_list.xml");
            Debug.Log("chemin: " + lPath);
            Utils.SerializeXML<ReminderContent>(ReminderContent, lPath);
        }

        public void UnSerializeList()
        {
            string lPath = BYOS.Instance.Resources.PathToRaw("reminder_list.xml");
			ReminderContent = Utils.UnserializeXML<ReminderContent>(lPath);
            if(ReminderContent==null)
            {
                ReminderContent = new ReminderContent();
            }
        }

        public void SortListByDate()
        {
            ReminderContent.reminderList.Sort((a, b) => a.RemindDate.CompareTo(b.RemindDate));
        }

    }
}