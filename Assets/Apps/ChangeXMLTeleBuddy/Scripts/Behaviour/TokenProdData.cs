﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.ChangeXMLTeleBuddy
{
    [SerializeField]
    public sealed class TokenProdData
    {
        public string Token_Refresh { get; set; }
        public string Client_ID { get; set; }
        public string Client_Secret { get; set; }
        public string URL_Request { get; set; }
    }
}
