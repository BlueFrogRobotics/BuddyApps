using UnityEngine;
using System.Collections;
using BuddyOS;
using UnityEngine.UI;

namespace BuddyApp.Jukebox
{
    public class JukeboxMovement : MonoBehaviour
    {
        [SerializeField]
        private Button thermalMovement;
        [SerializeField]
        private Button balladeMovement;

        public void Update()
        {
            Debug.Log(thermalMovement.GetComponent<Companion.FollowPersonReaction>().enabled + " " + balladeMovement.GetComponent<CompanionWalk>().enabled);
        }
        public void Walk()
        {
            if (balladeMovement.GetComponent<CompanionWalk>().enabled == false)
            {
                thermalMovement.GetComponent<Companion.FollowPersonReaction>().enabled = false;
                balladeMovement.GetComponent<CompanionWalk>().enabled = true;
            }  
            else
            {
                balladeMovement.GetComponent<CompanionWalk>().enabled = false;
            }
        }

        public void ThermalFollow()
        {
            if (thermalMovement.GetComponent<Companion.FollowPersonReaction>().enabled == false)
            {
                balladeMovement.GetComponent<CompanionWalk>().enabled = false;
                thermalMovement.GetComponent<Companion.FollowPersonReaction>().enabled = true;
            }
            else
            {
                thermalMovement.GetComponent<Companion.FollowPersonReaction>().enabled = false;
            } 
        }

        public void stopMovement()
        {
            if(thermalMovement.GetComponent<Companion.FollowPersonReaction>().enabled == true)
            {
                thermalMovement.GetComponent<Companion.FollowPersonReaction>().enabled = false;
            }
            else if(balladeMovement.GetComponent<CompanionWalk>().enabled == true)
            {
                balladeMovement.GetComponent<CompanionWalk>().enabled = false;
            }
        }
    }
}
