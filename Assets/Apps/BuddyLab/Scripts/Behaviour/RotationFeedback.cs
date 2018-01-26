using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        // Use this for initialization
        void Start()
        {

        }

        public override void OnNewValue(int iValue)
        {
            objectToRotate.GetComponent<RectTransform>().rotation = new Quaternion(0, 0, 90, 0);
        }

       
    }
}