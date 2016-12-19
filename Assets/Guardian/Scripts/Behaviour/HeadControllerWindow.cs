using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS.UI;

namespace BuddyApp.Guardian
{
    public class HeadControllerWindow : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.Button buttonLeft;

        [SerializeField]
        private UnityEngine.UI.Button buttonRight;

        [SerializeField]
        private UnityEngine.UI.Button buttonUp;

        [SerializeField]
        private UnityEngine.UI.Button buttonDown;

        [SerializeField]
        private UnityEngine.UI.Button buttonBack;

        [SerializeField]
        private RawImage rawCamImage;

        public UnityEngine.UI.Button ButtonLeft { get { return buttonLeft; } }
        public UnityEngine.UI.Button ButtonRight { get { return buttonRight; } }
        public UnityEngine.UI.Button ButtonUp { get { return buttonUp; } }
        public UnityEngine.UI.Button ButtonDown { get { return buttonDown; } }
        public UnityEngine.UI.Button ButtonBack { get { return buttonBack; } }
        public RawImage RawCamImage { get { return rawCamImage; } }

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