using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.FreezeDance
{
    public class RankItem : MonoBehaviour
    {
        [SerializeField]
        private GameObject hightlight;

        [SerializeField]
        private GameObject userName;

        [SerializeField]
        private GameObject userScore;

        [SerializeField]
        private GameObject userPlace;

        [SerializeField]
        private int place;

        public bool HighLight
        {
            get
            {
                return hightlight.activeSelf;
            }

            set
            {
                hightlight.SetActive(value);
            }
        }

        public string Name
        {
            get
            {
                return userName.GetComponent<Text>().text;
            }

            set
            {
                userName.GetComponent<Text>().text = value;
            }
        }

        public int Score
        {
            get
            {
                int lScore = 0;
                //int lol = 5;
                int.TryParse(userScore.GetComponent<Text>().text, out lScore);
                return lScore;
            }

            set
            {
                userScore.GetComponent<Text>().text = ""+value;
            }
        }

        public int Place
        {
            get
            {
                if (userPlace != null)
                {
                    int lPlace = 0;
                    int.TryParse(userPlace.GetComponent<Text>().text, out lPlace);
                    return lPlace;
                }
                else
                    return place;
            }

            set
            {
                if (userPlace != null)
                {
                    userPlace.GetComponent<Text>().text = "" + value;
                }
            }
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}