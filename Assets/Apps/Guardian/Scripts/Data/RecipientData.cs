using UnityEngine;

namespace BuddyApp.Guardian
{
    /// <summary>
    /// Contains all the informations of a recipient
    /// </summary>
    [SerializeField]
    public sealed class RecipientData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mail { get; set; }
    }
}