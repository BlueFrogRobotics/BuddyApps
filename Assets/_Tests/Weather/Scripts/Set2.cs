using Buddy;
using Buddy.UI;

using System;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;


namespace BuddyApp.Weather2
{

    public class Set2 : MonoBehaviour
    {

        private WeatherPanel Behav;


        void Start()
        {
            Debug.Log("Coucou Hiboux");

            StartCoroutine(Example());
            //get 

        }

        IEnumerator Example()
        {
            float num = 0.3f;

            Behav = GetComponent<WeatherPanel>();
            Behav.text.text = "42˚C";
            Behav.css.SetActive(true);
            Behav.cs.SetActive(true);
            Behav.cm.SetActive(true);
            Behav.cmm.SetActive(true);
            Behav.cb.SetActive(true);
            Behav.cbb.SetActive(true);
            Behav.lsun.SetActive(true);
            Behav.rain.SetActive(true);

            Behav.gm.GetComponent<Animator>().SetTrigger("open");
            yield return new WaitForSeconds(num);
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}