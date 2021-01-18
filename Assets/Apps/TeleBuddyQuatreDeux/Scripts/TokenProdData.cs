using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

namespace BuddyApp.TeleBuddyQuatreDeux
{
    [SerializeField]
    public sealed class TokenProdData
    {
        public string Token_Refresh { get; set; }
        public string Client_ID { get; set; }
        public string Client_Secret { get; set; }
        public bool API_Prod { get; set; }
    }
}

