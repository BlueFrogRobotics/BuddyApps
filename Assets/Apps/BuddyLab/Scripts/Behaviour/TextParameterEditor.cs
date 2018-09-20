using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;

namespace BuddyApp.BuddyLab
{
    public sealed class TextParameterEditor : MonoBehaviour
    {
        [SerializeField]
        private GameObject popupField;

        [SerializeField]
        private GameObject backgroundBlack;

        [SerializeField]
        private Button buttonValidate;

        [SerializeField]
        private Button buttonCancel;

        [SerializeField]
        private InputField inputField;

        //[SerializeField]
        //private GameObject Placeholder;

        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ShowField()
        {
            DraggableItem lCell = GetComponentInParent<DraggableItem>();
            if (lCell != null && !lCell.OnlyDroppable)
            {
                //Placeholder.GetComponent<Text>().text = ""; 
                buttonValidate.onClick.AddListener(Validate);
                buttonCancel.onClick.AddListener(Cancel);
                inputField.text = GetComponent<ABLItem>().Parameter;
                inputField.GetComponentsInChildren<Text>()[0].text = Buddy.Resources.GetString("texttosay");
                popupField.GetComponent<Animator>().SetTrigger("open");
                backgroundBlack.GetComponent<Animator>().SetTrigger("open");
            }
        }

        private void Validate()
        {
            GetComponent<ABLItem>().Parameter = inputField.text;
            CloseField();

            popupField.GetComponent<Animator>().ResetTrigger("open");
            backgroundBlack.GetComponent<Animator>().ResetTrigger("open");
            //popupField.GetComponent<Animator>().ResetTrigger("close");
            //backgroundBlack.GetComponent<Animator>().ResetTrigger("close");
        }

        private void Cancel()
        {
            CloseField();
        }

        private void CloseField()
        {
            buttonValidate.onClick.RemoveListener(Validate);
            buttonCancel.onClick.RemoveListener(Cancel);
            popupField.GetComponent<Animator>().SetTrigger("close");
            backgroundBlack.GetComponent<Animator>().SetTrigger("close");
        }
    }
}