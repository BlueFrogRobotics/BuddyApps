using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Buddy;
using Buddy.UI;

namespace BuddyApp.Guardian
{
    public class DebugMovementWindow : MonoBehaviour
    {

        [SerializeField]
        private RawImage raw;

        [SerializeField]
        private Gauge gaugeSensibility;

        [SerializeField]
        private UnityEngine.UI.Button buttonBack;

        [SerializeField]
        private Image icoMouv;

        [SerializeField]
        private Text message;

        [SerializeField]
        private Text labelGauge;

        public RawImage Raw { get { return raw; } }
        public Gauge GaugeSensibility { get { return gaugeSensibility; } }
        public UnityEngine.UI.Button ButtonBack { get { return buttonBack; } }
        public Image IcoMouv { get { return icoMouv; } }

        // Use this for initialization
        void Start()
        {
            message.text = BYOS.Instance.Dictionary.GetString("selectsensibility").ToUpper();
            labelGauge.text = BYOS.Instance.Dictionary.GetString("sensibility");
            gaugeSensibility.DisplayPercentage = true;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}