using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.FreezeDance
{
    public class PlayerScore 
    {
        public string Name { get; set; }

        public int Score { get; set; }
    }

    public class PlayerList
    {
        public List<PlayerScore> List { get; set; }

        public PlayerList()
        {
            List = new List<PlayerScore>();
        }
    }
}