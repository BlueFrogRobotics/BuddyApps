using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;

namespace BuddyApp.Weather
{

    public sealed class WeatherMan : MonoBehaviour
    {

        // Use this for initialization

        [SerializeField]
        internal Text Date;

        [SerializeField]
        internal Text City;

        [SerializeField]
        internal Text Country;

        [SerializeField]
        internal Text Time;

        [SerializeField]
        internal GameObject PanelSmall;

        [SerializeField]
        internal GameObject PanelMedium;

        [SerializeField]
        internal GameObject PanelBig;

        [SerializeField]
        internal GameObject PanelFull;

        [SerializeField]
        private GameObject WeatherGrid;

        [SerializeField]
        internal GameObject InfoSmall;

        [SerializeField]
        internal GameObject InfoMedium;

        [SerializeField]
        internal GameObject InfoBig;

        [SerializeField]
        internal GameObject InfoFull;

        [SerializeField]
        internal GameObject WeatherNext;

        public List<WeatherPanel> PanelsList;

        private List<GameObject> mVisibleGameObjects;

        void Awake()
        {
            PanelsList = new List<WeatherPanel>();

            mVisibleGameObjects = new List<GameObject>();

            WeatherGrid.GetComponent<Animator>().SetTrigger("open");

            Buddy.GUI.Header.DisplayParametersButton(false);
        }

        // Gets the number of panels needed to display the requested command
        public int GetNbPanels(WeatherBehaviour.WeatherCommand commandType)
        {
            if (commandType == WeatherBehaviour.WeatherCommand.NOW)
                return 2;
            else if (commandType == WeatherBehaviour.WeatherCommand.DAY)
                return 4;
            else if (commandType == WeatherBehaviour.WeatherCommand.WHEN)
                return 1;
            return 0;
        }

        // Activate a game object and store it
        private void ActivateGameObject(GameObject go)
        {
            go.SetActive(true);
            mVisibleGameObjects.Add(go);
        }

        // Display the weather panels needed to display the requested command
        public void DisplayPanels(WeatherBehaviour.WeatherCommand commandType)
        {
            foreach (GameObject go in mVisibleGameObjects)
                go.SetActive(false);
            mVisibleGameObjects.Clear();
            PanelsList.Clear();

            if (commandType == WeatherBehaviour.WeatherCommand.NOW)
                DisplayBigMediumPanel();
            else if (commandType == WeatherBehaviour.WeatherCommand.DAY)
                DisplaySmallPanels();
            else if (commandType == WeatherBehaviour.WeatherCommand.WHEN)
                DisplayFullPanel();
        }

        // Display one full panel + bottom info
        private void DisplayFullPanel()
        {
            // Middle
            ActivateGameObject(PanelFull);
            WeatherPanel p = PanelFull.GetComponent<WeatherPanel>();
            PanelsList.Add(p);

            // Bottom
            ActivateGameObject(InfoFull);
            PanelInfo info = InfoFull.GetComponent<PanelInfo>();
            p.BottomInfo = info;
        }

        // Display one big panel and one medium + bottom info for each
        private void DisplayBigMediumPanel()
        {
            // Big panel
            ActivateGameObject(PanelBig);
            WeatherPanel p = PanelBig.GetComponent<WeatherPanel>();
            ActivateGameObject(InfoBig);
            PanelInfo info = InfoBig.GetComponent<PanelInfo>();
            p.BottomInfo = info;
            PanelsList.Add(p);

            // Medium panel
            ActivateGameObject(PanelMedium);
            p = PanelMedium.GetComponent<WeatherPanel>();
            ActivateGameObject(InfoMedium);
            info = InfoMedium.GetComponent<PanelInfo>();
            p.BottomInfo = info;
            PanelsList.Add(p);
        }

        // Display 4 small panels
        private void DisplaySmallPanels()
        {
            for (int i = 0; i < 4; i++)
            {
                // Middle
                GameObject gameObject = Instantiate(PanelSmall);
                gameObject.transform.parent = WeatherGrid.transform;
                gameObject.transform.localScale = new Vector3(1, 1, 1);
                ActivateGameObject(gameObject);
                WeatherPanel p = gameObject.GetComponent<WeatherPanel>();

                // Bottom
                gameObject = Instantiate(InfoSmall);
                gameObject.transform.parent = WeatherNext.transform;
                gameObject.transform.localScale = new Vector3(1, 1, 1);
                ActivateGameObject(gameObject);
                PanelInfo info = gameObject.GetComponent<PanelInfo>();
                p.BottomInfo = info;

                PanelsList.Add(p);
            }
        }

        // Set the date in the title
        public void SetDate(string value)
        {
            Date.text = value;
        }

        // Set the location in the title
        public void SetLocation(string city, string country)
        {
            City.text = city;
            Country.text = " - " + country;
        }

        // Set the time in the bottom
        public void SetTime(string value)
        {
            Time.text = value + "h";
        }

        // Launch close animation on panels
        public void Close()
        {
            WeatherGrid.GetComponent<Animator>().SetTrigger("close");
            WeatherNext.GetComponent<Animator>().SetTrigger("close");
        }

    }
}