using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BuddyApp.Reminder
{
    public enum Intent : int
    {
        NONE = 0,
        ADD = 1,
        REMOVE = 2,
        PRINT = 3,
        FILTER = 4
    }

    public class Command
    {
        public Intent Intent { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime RemindDate { get; set; }
        public DateTime StartDate { get; set; }//for the print
        public DateTime EndDate { get; set; }//for the print
        public string Title { get; set; }
        public string Content { get; set; }
        public string Receiver { get; set; }

    }

    public interface IProcessVocal
    {
        Command ExtractParameters(string iCommand); 
    }
}