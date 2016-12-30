using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS;
using BuddyOS.UI;

namespace BuddyApp.Guardian
{
    public class AskPasswordWindow : MonoBehaviour
    {

        [SerializeField]
        private Text textField;

        // Use this for initialization
        void Start()
        {
            textField.text = BYOS.Instance.Dictionary.GetString("enterPassword");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}