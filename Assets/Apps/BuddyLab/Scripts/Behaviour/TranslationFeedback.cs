using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.BuddyLab
{
    public sealed class TranslationFeedback : AFeedback
    {
        [SerializeField]
        private GameObject objectToTranslate;

        [SerializeField]
        private float minTranslateValue;

        [SerializeField]
        private float maxTranslateValue;

        [SerializeField]
        private Scrollbar blueScrollbar;

        // Use this for initialization
        void Start()
        {

        }

        public override void OnNewValue(float iValue)
        {
            if(maxTranslateValue > minTranslateValue)
                objectToTranslate.GetComponent<RectTransform>().localPosition = new Vector3((maxTranslateValue - minTranslateValue) * iValue, objectToTranslate.GetComponent<RectTransform>().localPosition.y);
            else
                objectToTranslate.GetComponent<RectTransform>().localPosition = new Vector3((minTranslateValue - maxTranslateValue) * (1.0f-iValue), objectToTranslate.GetComponent<RectTransform>().localPosition.y);
            blueScrollbar.size = iValue;
        }

        

    }
}