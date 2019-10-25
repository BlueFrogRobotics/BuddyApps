using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.FreezeDance
{
    public class SettingsWindow : MonoBehaviour
    {
        [SerializeField]
        private GameObject settings;

        [SerializeField]
        private Button buttonMenu;

        [SerializeField]
        private Button buttonResetScores;

        [SerializeField]
        private Button buttonPlay;

        [SerializeField]
        private Dropdown dropdown;

        public Button ButtonMenu { get { return buttonMenu; } }
        public Button ButtonResetScores { get { return buttonResetScores; } }
        public Button ButtonPlay { get { return buttonPlay; } }

        public int MusicId { get { return dropdown.value; } }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ShowSettings()
        {
            settings.GetComponent<Animator>().SetTrigger("open");
        }

        public void HideSettings()
        {
            settings.GetComponent<Animator>().SetTrigger("close");
        }
    }
}