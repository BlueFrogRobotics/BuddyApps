using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.BuddyLab
{
    public class TextParameterEditor : MonoBehaviour
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
            DragAndDropCell lCell = GetComponentInParent<DragAndDropCell>();
            if (lCell != null && lCell.cellType == DragAndDropCell.CellType.Swap)
            {
                buttonValidate.onClick.AddListener(Validate);
                buttonCancel.onClick.AddListener(Cancel);
                inputField.text = GetComponent<ABLItem>().Parameter;
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