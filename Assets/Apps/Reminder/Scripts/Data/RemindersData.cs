using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Buddy;

using System.IO;
using System;

namespace BuddyApp.Reminder
{
    [Serializable]
    public class RemindersData
    {
        public List<Reminderkey> Reminders { get; set; }
    }

}
