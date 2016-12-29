using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS;
using BuddyOS.UI;

namespace BuddyApp.Guardian
{
    public class DebugSoundWindow : MonoBehaviour
    {

        [SerializeField]
        private RawImage raw;

        [SerializeField]
        private Gauge gaugeSensibility;

        [SerializeField]
        private UnityEngine.UI.Button buttonBack;

        [SerializeField]
        private Image ico;

        [SerializeField]
        private Text message;

        [SerializeField]
        private Text labelGauge;

        public RawImage Raw { get { return raw; } }
        public Gauge GaugeSensibility { get { return gaugeSensibility; } }
        public UnityEngine.UI.Button ButtonBack { get { return buttonBack; } }
        public Image Ico { get { return ico; } }

        // Use this for initialization
        void Start()
        {
            message.text = BYOS.Instance.Dictionary.GetString("selectSensibility").ToUpper();
            labelGauge.text = BYOS.Instance.Dictionary.GetString("sensibility");
            gaugeSensibility.DisplayPercentage = true;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
