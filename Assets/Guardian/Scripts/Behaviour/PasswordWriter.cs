using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS;

namespace BuddyApp.Guardian
{
    public class PasswordWriter : MonoBehaviour
    {
        [SerializeField]
        private InputField input;

        public string Password { get { return input.text; } }

        // Use this for initialization
        void Start()
        {
            input.text = "";

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void AddNumber(int iNumber)
        {
            if (input.text.Length < 4)
                input.text += iNumber;
            BYOS.Instance.Speaker.FX.Play(FXSound.BEEP_1);
        }

        public void Clear()
        {
            input.text = "";
        }
    }
}