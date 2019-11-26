using UnityEngine;

using System.Collections.Generic;

namespace BuddyApp.BabyPhone
{
    /// <summary>
    /// Contains a list of recipients
    /// </summary>
    [SerializeField]
    public sealed class RecipientsData
    {
        public List<RecipientData> Recipients { get; set; }
    }
}