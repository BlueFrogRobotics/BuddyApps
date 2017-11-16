using Buddy;
using Buddy.UI;

using System;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;


namespace BuddyApp.Weather2
{

    public class WeatherPanel : MonoBehaviour
    {

        private Image myImage;
        internal Animator anim;
        [SerializeField]
        internal GameObject gm;
        [SerializeField]
        internal Text text;
        [SerializeField]
        internal GameObject cs;
        [SerializeField]
        internal GameObject css;
        [SerializeField]
        internal GameObject cm;
        [SerializeField]
        internal GameObject cmm;
        [SerializeField]
        internal GameObject cb;
        [SerializeField]
        internal GameObject cbb;
        [SerializeField]
        internal GameObject gs;
        [SerializeField]
        internal GameObject gss;
        [SerializeField]
        internal GameObject gcm;
        [SerializeField]
        internal GameObject gcmm;
        [SerializeField]
        internal GameObject gb;
        [SerializeField]
        internal GameObject gbb;
        [SerializeField]
        internal GameObject rain;
        [SerializeField]
        internal GameObject snow;
        [SerializeField]
        internal GameObject frost;
        [SerializeField]
        internal GameObject halo;
        [SerializeField]
        internal GameObject lsun;
        [SerializeField]
        internal GameObject bsun;
        [SerializeField]
        internal GameObject lmoon;
        [SerializeField]
        internal GameObject bmoon;
        Sprite[] allSprites;

        public void SetSun()
        {
            lsun.SetActive(true);
            bsun.SetActive(true);
            halo.SetActive(true);
        }

        public void SetMoon()
        {
            lmoon.SetActive(true);
            bmoon.SetActive(true);
        }

        public void SetFrost()
        {
            frost.SetActive(true);
        }

        public void SetRain()
        {
            rain.SetActive(true);
        }

        public void SetSnow()
        {
            snow.SetActive(true);
        }

        public void SetCloud1()
        {
            cs.SetActive(true);
            css.SetActive(true);

        }

        public void SetCloud2()
        {
            SetCloud1();
            cm.SetActive(true);
            cmm.SetActive(true);

        }

        public void SetCloud3()
        {
            SetCloud1();
            SetCloud2();
            cb.SetActive(true);
            cbb.SetActive(true);
        }

        public void SetCloud4()
        {
            SetCloud1();
            SetCloud2();
            SetCloud3();
            gs.SetActive(true);
            gss.SetActive(true);
        }

        public void SetCloud5()
        {
            SetCloud1();
            SetCloud2();
            SetCloud3();
            SetCloud4();
            gm.SetActive(true);
            gcmm.SetActive(true);


        }

        public void SetCloud6()
        {
            SetCloud1();
            SetCloud2();
            SetCloud3();
            SetCloud4();
            SetCloud5();
            gb.SetActive(true);
            gbb.SetActive(true);
        }

        public void SetText(string degree)
        {
            text.text = degree;
        }
        
        public void Open()
        {
            gm.GetComponent<Animator>().SetTrigger("open");
        }

        public void Morning()
        {


            //myImage = GetComponent<Image>();
            //myImage.sprite = Morning;
        }

        private void Start()
        {
           
        }
    // Update is called once per fram
    void Update()
        {

        }
    }
}