using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BuddyApp.CoursTelepresence
{
    public class CallRequest
    {

        public string userName;
        public string channelId;

        public CallRequest(string iChannelId, string iUserName = "")
        {
            userName = iUserName;
            channelId = iChannelId;
        }


    }
}