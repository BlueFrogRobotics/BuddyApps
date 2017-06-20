using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Reminder
{
    public class DateUI : MonoBehaviour
    {
        [SerializeField]
        private Text dateText;

        [SerializeField]
        private Button deleteButton;

        public string Date { get { return dateText.text; } set { dateText.text = value; } }
        public Button DeleteButton { get { return deleteButton; } }

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