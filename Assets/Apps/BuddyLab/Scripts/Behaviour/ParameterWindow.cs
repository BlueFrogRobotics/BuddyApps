using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.BuddyLab
{
    public class ParameterWindow : MonoBehaviour
    {
        [SerializeField]
        private GameObject backgroundBlack;


        [SerializeField]
        private InputField inputField;

        [SerializeField]
        private Slider slider;

        [SerializeField]
        private Button validateButton;

        //[SerializeField]
        //private Button eraseButton;

        [SerializeField]
        private float minValue;

        [SerializeField]
        private float maxValue;

        [SerializeField]
        private AFeedback mFeedback;

        public float MaxValue { get { return maxValue; } }

        public float MinValue { get { return minValue; } }

        public Button ValidateButton { get { return validateButton; } }

        public string Parameter { get; private set; }

        private Animator mAnimator;




        // Use this for initialization
        void Start()
        {
            mAnimator = GetComponent<Animator>();
            slider.onValueChanged.AddListener(OnSliderValueChange);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ShowWindow()
        {
            mAnimator.SetTrigger("open");
            backgroundBlack.GetComponent<Animator>().SetTrigger("open");
            //mFeedback = Instantiate(feedback);
            //mFeedback.transform.parent = transform;
            //mFeedback.transform.SetSiblingIndex(1);
        }

        public void HideWindow()
        {
            mAnimator.SetTrigger("close");
            backgroundBlack.GetComponent<Animator>().SetTrigger("close");
            //Destroy(mFeedback);
            //mFeedback = null;
        }

        public void AddNumber(int iNb)
        {
            if (inputField.text.Length < 3)
            {
                inputField.text += iNb;
                ChangeValue(inputField.text);
                
            }
        }


        public void SetValue(float iValue)
        {
            slider.value = (iValue-minValue) / (maxValue-minValue);
            inputField.text = "" + iValue;
        }

        private void ChangeValue(string iValue)
        {
            Debug.Log("le value: " + iValue);
            Parameter = iValue;
            float lNb = 0;
            float.TryParse(iValue, out lNb);
            if (lNb > maxValue)
                lNb = maxValue;
            if (lNb < minValue)
                lNb = minValue;
            slider.value = (lNb-minValue) / (maxValue-minValue);
            
        }

        public void EraseLastNumber()
        {
            if(inputField.text.Length>0)
                inputField.text = inputField.text.Remove(inputField.text.Length - 1);
            ChangeValue(inputField.text);
        }

        private void OnSliderValueChange(float iValue)
        {
            float lParam = (iValue * (maxValue-minValue)+minValue );
            inputField.text = ""+ Mathf.RoundToInt(lParam);
            Parameter = inputField.text;
            if(mFeedback!=null)
                mFeedback.OnNewValue(iValue);
            //ChangeValue(inputField.text);
            Debug.Log("value du param: " + iValue * maxValue+" lParam: "+lParam+" min: "+minValue);
        }
    }
}