using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.HideAndSeek
{
    public class Players : MonoBehaviour
    {

        [SerializeField]
        private InputField inputNumPlayer;

        [SerializeField]
        private Button buttonValidate;

        public Button ButtonValidate { get { return buttonValidate; } }
        public int NumPlayer { get { return mNumPlayer; } set { mNumPlayer = value; } }
        public List<string> NamesPlayers { get { return mNamesPlayers; } }

        private List<string> mNamesPlayers;
        private int mNumPlayer = 0;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ValidateNumPlayer()
        {
            int.TryParse(inputNumPlayer.text, out mNumPlayer); 
        }

        public void AddOnePlayer()
        {
            mNumPlayer++;
        }

        public void DeleteOnePlayer()
        {
            mNumPlayer--;
        }
    }
}