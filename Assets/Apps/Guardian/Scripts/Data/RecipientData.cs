using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.Guardian
{

    [SerializeField]
    public sealed class RecipientData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mail { get; set; }
    }
}