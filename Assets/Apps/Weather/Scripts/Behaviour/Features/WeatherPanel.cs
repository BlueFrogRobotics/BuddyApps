using BlueQuark;


using System;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;


namespace BuddyApp.Weather
{

    public sealed class WeatherPanel : MonoBehaviour
    {
        [SerializeField]
        internal GameObject PanelGO;
        [SerializeField]
        internal Text Temperature;
        [SerializeField]
        internal Text Wind;
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

        public PanelInfo BottomInfo = null;

        [SerializeField]
        private Image image;

        // Coeff to convert m/s to km/h
        private const float SPEED_CONVERSION_COEFF = 3.6f;

        void Awake()
        {
            PanelGO.GetComponent<Animator>().SetTrigger("open");
        }

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
            Temperature.text = degree;
        }

        public void SetTime(int value)
        {
            if (BottomInfo)
                BottomInfo.SetTime(value);
        }

        public void SetWind(float value)
        {
            float wind = value * SPEED_CONVERSION_COEFF;
            Wind.text = Math.Round(wind).ToString() + " Km/h";
        }

        public void SetMoment(string moment)
        {
            // Moment is not displayed to keep a lighter UI
            if (Moment)
                Moment.text = "";// moment;
        }

        public void ChangeTextColor(Color NewColor)
        {
            Temperature.color = NewColor;
        }

        public void ChangeMomentColor(Color NewColor)
        {
            //Moment.color = NewColor;
        }

        public void Open()
        {
            PanelGO.GetComponent<Animator>().SetTrigger("open");
        }

        public void blackBG()
        {
            Debug.Log("background black used : ");
            //Anciennement background black : a remettre avec le custom toast
            //Buddy.GUI.Toaster.Display<CustomToast>().With(,);
        }

        public void Cancel()
        {
            Close();

            Desactivate();

            ChangeMomentColor(Color.black);
            ChangeTextColor(Color.black);
        }

        public void Desactivate()
        {
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
        }

        public void Close()
        {
            PanelGO.GetComponent<Animator>().SetTrigger("close");
        }

        public void Morning()
        {
            image.sprite = Buddy.Resources.Get<Sprite>("Weather_BG_Morning");
        }

        public void Noon()
        {
            image.sprite = Buddy.Resources.Get<Sprite>("Weather_BG_Noon");
        }

        public void After_Noon()
        {
            image.sprite = Buddy.Resources.Get<Sprite>("Weather_BG_Afternoon");
        }

        public void Evening()
        {
            image.sprite = Buddy.Resources.Get<Sprite>("Weather_BG_Night");
        }
    }
}