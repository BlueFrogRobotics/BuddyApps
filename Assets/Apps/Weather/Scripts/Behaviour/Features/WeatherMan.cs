using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.Weather
{

    public sealed class WeatherMan : MonoBehaviour
    {

        // Use this for initialization

        [SerializeField]
        internal GameObject WeatherPanelGO;

        [SerializeField]
        private GameObject gridGO;

        [SerializeField]
        internal GameObject LittleBlockGO;

        [SerializeField]
        private GameObject NextGO;

        private WeatherPanel WP;

        public List<WeatherPanel> block = new List<WeatherPanel>();

        void Awake()
        {

            gridGO.GetComponent<Animator>().SetTrigger("open");
            for (int i = 0; i < 4; i++)
            {
                GameObject GO = Instantiate(WeatherPanelGO);
                GO.transform.parent = gridGO.transform;
                WP = GO.GetComponent<WeatherPanel>();
                block.Add(WP);
            }

            NextGO.GetComponent<Animator>().SetTrigger("open");
            for (int i = 0; i < 4; i++)
            {
                GameObject GO = Instantiate(LittleBlockGO);
                GO.transform.parent = NextGO.transform;
                WP = GO.GetComponent<WeatherPanel>();
                block.Add(WP);
            }
        }
    
        //StartCoroutine(Example());
        //IEnumerator Example()
        //{
        //    float num = 0.3f;
        //    for (int i = 0; i < 4; i++)
        //    {
        //        WP = block[i];
        //        WP.Open();
        //        WP.SetSun();
        //        yield return new WaitForSeconds(num);
        //    }
        //}

            // Update is called once per fram
        void Update()
        {

        }

    }
}