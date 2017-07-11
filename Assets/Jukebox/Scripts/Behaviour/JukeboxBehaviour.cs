using UnityEngine.UI;
using UnityEngine;

using Buddy;

namespace BuddyApp.Jukebox
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class JukeboxBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private JukeboxData mAppData;

        /*
         * Init refs to API and your app data
         */
        void Start()
        {
            mAppData = JukeboxData.Instance;
        }
    }
}