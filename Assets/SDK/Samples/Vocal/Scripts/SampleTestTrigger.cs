using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS;

namespace BuddyApp.IOT
{
    public class SampleTestTrigger : MonoBehaviour
    {
        SphinxTrigger mSphinx;

        [SerializeField]
        Toggle mToggle;
        void Start()
        {
            mSphinx = BYOS.Instance.SphinxTrigger;
        }

        void Update()
        {
            mToggle.isOn = mSphinx.HasTriggered;
        }

        public void Trigger()
        {
            mSphinx.LaunchRecognition();
        }

        public void ChangeTrigger(string iTrigger)
        {
            mSphinx.SetKeyphrase(iTrigger);
        }
    }
}
