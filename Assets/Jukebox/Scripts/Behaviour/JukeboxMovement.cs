using UnityEngine;
using System.Collections;
using BuddyOS;
using BuddyFeature.Navigation;

namespace BuddyApp.Jukebox
{
    public class JukeboxMovement : MonoBehaviour
    {
        public void Walk()
        {
            if (GetComponent<RoombaNavigation>().enabled == false)
                GetComponent<RoombaNavigation>().enabled = true;
            else
                GetComponent<RoombaNavigation>().enabled = false;
        }
    }
}
