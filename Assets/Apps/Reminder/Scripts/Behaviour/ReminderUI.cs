using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Reminder
{
    public class ReminderUI : MonoBehaviour
    {
        [SerializeField]
        private Text titleText;

        [SerializeField]
        private Text hourText;

        [SerializeField]
        private Button deleteButton;

        public string Title { get { return titleText.text; } set { titleText.text = value; } }
        public string Hour { get { return hourText.text; } set { hourText.text = value; } }
        public Button DeleteButton { get { return deleteButton; } }
        public int Num { get; set; }

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