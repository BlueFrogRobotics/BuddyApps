using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.BuddyLab
{
    public sealed class BMLParameterManager : MonoBehaviour
    {
        [SerializeField]
        private ParameterWindow parameterWindow;

        [SerializeField]
        private Text textBli;

        [SerializeField]
        private float divisionCoeff =1;


        // Use this for initialization
        void Start()
        {
            //textBli.text = GetComponent<BMLItem>().Parameter;
            float lNumParameter = 0;
            float.TryParse(GetComponent<ABLItem>().Parameter, out lNumParameter);
            lNumParameter *= divisionCoeff;
            textBli.text = "" + lNumParameter;
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ShowWindow()
        {
            Debug.Log("le show button");
            DraggableItem lItem = GetComponentInParent<DraggableItem>();
            if(lItem!=null && !lItem.OnlyDroppable)
            {
                float lNumParameter = 0;
                float.TryParse(GetComponent<ABLItem>().Parameter, out lNumParameter);
                lNumParameter *= divisionCoeff;
                parameterWindow.SetValue(lNumParameter);
                parameterWindow.ShowWindow();
                parameterWindow.ValidateButton.onClick.AddListener(OnValidation);
            }
        }

        private void OnValidation()
        {
            Debug.Log("validation");
            if (divisionCoeff == 0)
                divisionCoeff = 1;
            float lNumParameter = 0;
            bool lHasParse=float.TryParse(parameterWindow.Parameter, out lNumParameter);
            if (!lHasParse)
                lNumParameter = 0;
            if (lNumParameter > parameterWindow.MaxValue)
                lNumParameter = parameterWindow.MaxValue;
            if (lNumParameter < parameterWindow.MinValue)
                lNumParameter = parameterWindow.MinValue;
            textBli.text = ""+lNumParameter;

            lNumParameter /= divisionCoeff;
            GetComponent<ABLItem>().Parameter = "" + lNumParameter;
            
            parameterWindow.HideWindow();
            parameterWindow.ValidateButton.onClick.RemoveListener(OnValidation);
        }
    }
}