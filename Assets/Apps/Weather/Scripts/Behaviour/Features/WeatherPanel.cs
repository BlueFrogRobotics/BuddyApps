using Buddy;
using Buddy.UI;

using System;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;


namespace BuddyApp.Weather
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
        internal Text Moment;
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
        internal GameObject gsky;
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
        //Sprite[] allSprites;

        [SerializeField]
        private Image image;

        public void SetSun()
        {
            lsun.SetActive(true);
            bsun.SetActive(true);
           
        }

        public void SetHalo()
        {
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
            SetCloud2();
            cb.SetActive(true);
            cbb.SetActive(true);
        }

        public void SetCloud4()
        {
            SetCloud3();
            gsky.SetActive(true);
            gs.SetActive(true);
            gss.SetActive(true);
        }

        public void SetCloud5()
        {
            SetCloud4();
            gcm.SetActive(true);
            gcmm.SetActive(true);
        }

        public void SetCloud6()
        {
            SetCloud5();
            gb.SetActive(true);
            gbb.SetActive(true);
        }

        public void SetText(string degree)
        {
            text.text = degree;
        }

        public void SetMoment(string moment)
        {
            Moment.text = moment;
        }

        public void ChangeTextColor(Color NewColor)
        {
            text.color = NewColor;
        }

        public void ChangeMomentColor(Color NewColor)
        {
            Moment.color = NewColor;
        }

        public void Open()
        {
            gm.GetComponent<Animator>().SetTrigger("open");
        }

        public void blackBG()
        {
            BYOS.Instance.Toaster.Display<BackgroundToast>();
        }

        public void Cancel()
        {
            Close();
            cs.SetActive(false);
            css.SetActive(false);
            cm.SetActive(false);
            cmm.SetActive(false);
            cb.SetActive(false);
            cbb.SetActive(false);
            gsky.SetActive(false);
            gs.SetActive(false);
            gss.SetActive(false);
            gcm.SetActive(false);
            gcmm.SetActive(false);
            gb.SetActive(false);
            gbb.SetActive(false);
            lmoon.SetActive(false);
            lsun.SetActive(false);
            halo.SetActive(false);
            snow.SetActive(false);
            bsun.SetActive(false);
            bmoon.SetActive(false);
            frost.SetActive(false);
            rain.SetActive(false);

            ChangeMomentColor(Color.black);
            ChangeTextColor(Color.black);
        }

        public void Close()
        {
            gm.GetComponent<Animator>().SetTrigger("close");
        }

        public void Morning()
        {
            image.sprite = BYOS.Instance.Resources.GetSpriteFromAtlas("BG_Morning", "Atlas_Meteo");
        }

        public void Noon()
        {
            image.sprite = BYOS.Instance.Resources.GetSpriteFromAtlas("BG_Noon", "Atlas_Meteo");
        }

        public void After_Noon()
        {
            image.sprite = BYOS.Instance.Resources.GetSpriteFromAtlas("BG_Afternoon", "Atlas_Meteo");
        }

        public void Evening()
        {
            image.sprite = BYOS.Instance.Resources.GetSpriteFromAtlas("BG_Night", "Atlas_Meteo");
        }
    }
}