using UnityEngine;
using UnityEngine.UI;
using BuddyOS;
using System.Collections;

namespace BuddyApp.Guardian
{
    public class QuestionWindow : MonoBehaviour
    {

        [SerializeField]
        private Text validateLabel;

        [SerializeField]
        private Text cancelLabel;

        // Use this for initialization
        void Start()
        {
            validateLabel.text = BYOS.Instance.Dictionary.GetString("validate").ToUpper();
            cancelLabel.text = BYOS.Instance.Dictionary.GetString("cancel").ToUpper();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}