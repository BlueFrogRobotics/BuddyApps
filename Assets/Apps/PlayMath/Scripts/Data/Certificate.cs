using System;
using UnityEngine;
using UnityEngine.UI;

//using System.IO;
//using Buddy;

namespace BuddyApp.PlayMath{
    public class Certificate : MonoBehaviour {

        public GameParameters GameParams{ get; set; }

        public DateTime TimeStamp{ get; set; }

        public Texture2D UserPic{ get; set; }

        public Certificate()
        {
        }

        //public void SavePicToFile()
        //{
            // optional quality (int) param, may also encode to png
            //byte[] bytes = UserPic.EncodeToJPG(); 
            //string path = BYOS.Instance.Resources.GetPathToRaw("userpic", LoadContext.APP);
            //File.WriteAllBytes(path, bytes);
        //}
    }
}

