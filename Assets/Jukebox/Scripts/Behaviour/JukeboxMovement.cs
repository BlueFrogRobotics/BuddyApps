﻿using UnityEngine;
using System.Collections;
using Buddy;
using UnityEngine.UI;

namespace BuddyApp.Jukebox
{
    public class JukeboxMovement : MonoBehaviour
    {
        //[SerializeField]
        //private Button thermalMovement;
        //[SerializeField]
        //private Button balladeMovement;

        //private YesHinge mYesHinge;

        public void Start()
        {
            //mYesHinge = BYOS.Instance.Primitive.Motors.YesHinge;
        }

        public void Update()
        {
            //Debug.Log(thermalMovement.GetComponent<Companion.FollowPersonReaction>().enabled + " " + balladeMovement.GetComponent<CompanionWalk>().enabled);
        }
        public void Walk()
        {
            Debug.Log("start mouv");
            BYOS.Instance.Navigation.Roomba.enabled = true;
            //if (balladeMovement.GetComponent<CompanionWalk>().enabled == false)
            //{
            //    //thermalMovement.GetComponent<Companion.FollowPersonReaction>().enabled = false;
            //    balladeMovement.GetComponent<CompanionWalk>().enabled = true;
            //}  
            //else
            //{
            //    balladeMovement.GetComponent<CompanionWalk>().enabled = false;
            //}
        }

        public void ThermalFollow()
        {
            //if (thermalMovement.GetComponent<Companion.FollowPersonReaction>().enabled == false)
            //{
            //    balladeMovement.GetComponent<CompanionWalk>().enabled = false;
            //    mYesHinge.Locked = true;
            //    thermalMovement.GetComponent<Companion.FollowPersonReaction>().enabled = true;
            //}
            //else
            //{
            //    thermalMovement.GetComponent<Companion.FollowPersonReaction>().enabled = false;
            //    mYesHinge.Locked = false;
            //} 
        }

        public void stopMovement()
        {
            Debug.Log("stop mouv");
            BYOS.Instance.Navigation.Roomba.enabled = false;
            BYOS.Instance.Primitive.Motors.Wheels.StopWheels();
            //if(thermalMovement.GetComponent<Companion.FollowPersonReaction>().enabled == true)
            //{
            //    thermalMovement.GetComponent<Companion.FollowPersonReaction>().enabled = false;
            //}
            //else if(balladeMovement.GetComponent<CompanionWalk>().enabled == true)
            //{
            //    balladeMovement.GetComponent<CompanionWalk>().enabled = false;
            //}
        }
    }
}
