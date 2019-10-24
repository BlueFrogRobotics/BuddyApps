using System;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Calcul{
    public class Certificate : MonoBehaviour {

        public GameParameters GameParams{ get; set; }

        public DateTime TimeStamp{ get; set; }

        public Texture2D UserPic{ get; set; }

        public Certificate()
        {
        }
    }
}

