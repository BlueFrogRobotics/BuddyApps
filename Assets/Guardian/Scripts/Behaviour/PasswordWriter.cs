using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BuddyApp.Guardian
{
    public class PasswordWriter : MonoBehaviour
    {

        public InputField mInput;
        public string Password { get { return mInput.text; } }

        // Use this for initialization
        void Start()
        {
            mInput.text = "";

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void AddNumber(int iNumber)
        {
            if (mInput.text.Length < 4)
                mInput.text += iNumber;
        }

        public void Clear()
        {
            mInput.text = "";
        }
    }
}