using UnityEngine;
using UnityEngine.UI;
using BuddyOS.UI;
using BuddyOS;
using System.Collections.Generic;
using OpenCVUnity;

namespace BuddyApp.BabyPhone
{
    [RequireComponent(typeof(InputMicro))]
    [RequireComponent(typeof(AudioSource))]
    public class DebugSoundDetection : MonoBehaviour
    {
        [SerializeField]
        private RawImage soundViewer;

        [SerializeField]
        private Gauge microSensitivity;

        [SerializeField]
        private Text labelSound;

        private Animator debugSoundAnimator;
        private Dictionary mDictionary;

        private InputMicro mInputMicro;
        private float mSound;
        private Mat mImage;

        void OnEnable()
        {
            mDictionary = BYOS.Instance.Dictionary;
            debugSoundAnimator = GetComponent<Animator>();
            debugSoundAnimator.SetTrigger("Open_WDebugs");
            mImage = new Mat();
        }

        void OnDisable()
        {
            debugSoundAnimator.SetTrigger("Close_WDebugs");
        }

        void Start()
        {
            Init();
            Labelize();
        }

        public void Labelize()
        {
            microSensitivity.Label.text = mDictionary.GetString("soundetect");
            labelSound.text = mDictionary.GetString("microsens");
        }

        public void Init()
        {
            ////sound detection
            microSensitivity.DisplayPercentage = true;
            microSensitivity.Slider.minValue = 5;
            microSensitivity.Slider.maxValue = 20;
            microSensitivity.Slider.value = BabyPhoneData.Instance.MicrophoneSensitivity;
            microSensitivity.UpdateCommands.Add(new SetMicroSensCmd());
        }


    }
}
