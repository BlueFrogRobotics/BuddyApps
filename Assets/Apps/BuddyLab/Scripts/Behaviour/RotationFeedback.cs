using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.BuddyLab
{
    public class RotationFeedback : AFeedback
    {

        [SerializeField]
        private GameObject objectToRotate;

        [SerializeField]
        private int minRotationValue;

        [SerializeField]
        private int maxRotationValue;

        [SerializeField]
        private Image blueArea;

        [SerializeField]
        private float maxFill;

        // Use this for initialization
        void Start()
        {

        }

        public override void OnNewValue(float iValue)
        {
            Debug.Log("value feedback: " + maxRotationValue * iValue);
            objectToRotate.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, maxRotationValue * iValue);
            blueArea.fillAmount = maxFill * iValue;
        }

       
    }
}