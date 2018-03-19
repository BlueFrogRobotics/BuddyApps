using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Buddy;

using System.IO;
using System;

namespace BuddyApp.Reminder
{
    [Serializable]
    public class Reminderkey
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public string Date { get; set; }

        public string Hour { get; set; }

        public int ID { get; set; }
    }

}

