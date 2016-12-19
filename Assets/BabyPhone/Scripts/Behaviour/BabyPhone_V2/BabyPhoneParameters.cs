using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS.UI;

namespace BuddyApp.BabyPhone
{
    public class BabyPhoneParameters : MonoBehaviour
    {
        [SerializeField]
        private BuddyOS.UI.Dropdown ContactSelection;

        [SerializeField]
        private Slider Volume;
        [SerializeField]
        private GaugeOnOff VolumeOnOff;
        //[SerializeField]
        //private Button LullabyOnOff;
        [SerializeField]
        private BuddyOS.UI.Dropdown LullabySelection;

        [SerializeField]
        private Slider AnimationLight;
        [SerializeField]
        private GaugeOnOff AnimationOnOff;
        //[SerializeField]
        //private Button AnimationPlayPause;
        [SerializeField]
        private BuddyOS.UI.Dropdown AnimationSelection;

        [SerializeField]
        private Slider Microphone;

        [SerializeField]
        private Slider TimeBeforContact;

        [SerializeField]
        private BuddyOS.UI.Dropdown IfBabyCry;

        [SerializeField]
        private Slider ScreenSaverTime;
        [SerializeField]
        private GaugeOnOff ScreenSaverOnOff;

        [SerializeField]
        private GaugeOnOff MobileRobotOnOff;

        [SerializeField]
        private GaugeOnOff SaveSettings;

        private BabyPhoneData mBabyPhoneData;
        void Start()
        {

        }


        void Update()
        {
           // mBabyPhoneData.Volume = 

        }
    }
}
