using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.TakePhoto
{
    public enum Publish : int
    {
        TWITTER,
        MAIL,
        BOTH
    };

    [SerializeField]
    public sealed class XMLData
    {
        public Publish WhereToPublish { get; set; }
        public string Token { get; set; }
        public string TokenSecret { get; set; }
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public bool UseKey { get; set; }
        public string TwitterText { get; set; }
        public string TwitterHashtag { get; set; }
        public string AdressMailReceiver { get; set; }
        public string AdressMailSender { get; set; }
        public string PasswordMail { get; set; }
        public string SubjectMail { get; set; }
        public string BodyMail { get; set; }

    }
}

