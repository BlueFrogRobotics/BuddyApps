using UnityEngine;
using UnityEngine.UI;
using BuddyOS;

namespace BuddySample
{
    /// <summary>
    /// PulseLight is used for change the body light of Buddy in SampleBuddyLight
    /// </summary>
    public class PulseLight : MonoBehaviour
    {
        [SerializeField]
        private Image feedbackColor;

        /// <summary>
        /// Reference to the Buddy LED manager.
        /// </summary>
        private LED mLED;

        /// <summary>
        /// Color use when use a pulse method
        /// </summary>
        private Color32 mColor;

        void Start()
        {
            mLED = BYOS.Instance.LED;
            RandomColor();
        }

        public void RandomColor()
        {
            byte lR = (byte)Random.Range(0, 255);
            byte lG = (byte)Random.Range(0, 255);
            byte lB = (byte)Random.Range(0, 255);
            mColor = new Color32(lR, lG, lB, 255);
            feedbackColor.color = mColor;
        }

        /// <summary>
        /// set no pulse to the buddy corelight
        /// </summary>
        public void NoPulse()
        {
            mLED.SetBodyLight(mColor);
        }

        /// <summary>
        /// set slow pulse of the buddy corelight
        /// </summary>
        public void SlowPulse()
        {
            mLED.SetBodyLight(mColor, 0.9F, 1F);
        }

        /// <summary>
        /// Set fast pulse to the buddy corelight
        /// </summary>
        public void FastPulse()
        {
            mLED.SetBodyLight(mColor, 0.5F, 4F);
        }
    }
}
