using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BlueQuark;


namespace BuddyApp.Guardian
{
    /// <summary>
    /// Class that contains the references to the different elements of the movement test window
    /// </summary>
    public sealed class DebugMovementWindow : MonoBehaviour
    {

        [SerializeField]
        private RawImage raw;

        [SerializeField]
        private TSlider gaugeSensibility;

        [SerializeField]
        private UnityEngine.UI.Button buttonBack;

        [SerializeField]
        private Image icoMouv;

        [SerializeField]
        private Text message;

        [SerializeField]
        private Text labelGauge;

        [SerializeField]
        private RawImage rawCamImage;

        public RawImage Raw { get { return raw; } }
        public TSlider GaugeSensibility { get { return gaugeSensibility; } }
        public UnityEngine.UI.Button ButtonBack { get { return buttonBack; } }
        public Image IcoMouv { get { return icoMouv; } }
        public RawImage RawCamImage { get { return rawCamImage; } }

        // Use this for initialization
        void Start()
        {
            message.text = Buddy.Resources.GetString("selectsensibility").ToUpper();
            labelGauge.text = Buddy.Resources.GetString("sensibility");
            //gaugeSensibility..DisplayPercentage = true;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}