using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.BuddyLab
{
    public sealed class LabUIEditorManager : MonoBehaviour
    {

        [SerializeField]
        private GameObject background;

        [SerializeField]
        private GameObject bottomUI;

        [SerializeField]
        private GameObject playUI;

        [SerializeField]
        private GameObject lineProgram;

        [SerializeField]
        private GameObject trashArea;

        [SerializeField]
        private GameObject popupMovement;

        [SerializeField]
        private GameObject popupExpression;

        [SerializeField]
        private GameObject popupSounds;

        [SerializeField]
        private GameObject popupSensors;

        [SerializeField]
        private GameObject popupVisions;

        [SerializeField]
        private GameObject popupLoops;

        [SerializeField]
        private Button playButton;

        [SerializeField]
        private Button stopButton;

        [SerializeField]
        private Button replayButton;

        [SerializeField]
        private Button redoButton;

        [SerializeField]
        private Button undoButton;

        [SerializeField]
        private Button folderButton;

        [SerializeField]
        private Button saveButton;

        [SerializeField]
        private Button backButton;

        public Button PlayButton { get { return playButton; } }

        public Button StopButton { get { return stopButton; } }

        public Button ReplayButton { get { return replayButton; } }

        public Button RedoButton { get { return redoButton; } }

        public Button UndoButton { get { return undoButton; } }

        public Button FolderButton { get { return folderButton; } }

        public Button SaveButton { get { return saveButton; } }

        public Button BackButton { get { return backButton; } }

        // Use this for initialization
        void Start()
        {
            //OpenBottomUI();
            //OpenLineProgram();
            //OpenTrashArea();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetBackground(bool iActive)
        {
            background.SetActive(iActive);
        }

        public void OpenBottomUI()
        {
            bottomUI.GetComponent<Animator>().ResetTrigger("close");
            bottomUI.GetComponent<Animator>().ResetTrigger("open");
            bottomUI.GetComponent<Animator>().SetTrigger("open");
        }

        public void CloseBottomUI()
        {
            bottomUI.GetComponent<Animator>().ResetTrigger("close");
            bottomUI.GetComponent<Animator>().ResetTrigger("open");
            bottomUI.GetComponent<Animator>().SetTrigger("close");
            //bottomUI.GetComponent<Animator>().ResetTrigger("close");
        }

        public void OpenPlayUI()
        {
            playUI.GetComponent<Animator>().SetTrigger("open");
        }

        public void ClosePlayUI()
        {
            playUI.GetComponent<Animator>().SetTrigger("close");
        }

        public void OpenLineProgram()
        {
            lineProgram.GetComponent<Animator>().SetTrigger("open");
        }

        public void CloseLineProgram()
        {
            lineProgram.GetComponent<Animator>().SetTrigger("close");
        }

        public void OpenTrashArea()
        {
            trashArea.GetComponent<Animator>().SetTrigger("open");
        }

        public void CloseTrashArea()
        {
            trashArea.GetComponent<Animator>().SetTrigger("close");
        }

        public void OpenMenu()
        {
            CloseWindows();
            if (bottomUI.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Bottom_UI_Idle"))
                bottomUI.GetComponent<Animator>().SetTrigger("open_menu");
        }

        public void CloseMenu()
        {
            //A voir l'animation quand on play, s'il y a un probleme d'animation faire close_menu puis un close
            if (bottomUI.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Bottom_UI_MenuIdle"))
                bottomUI.GetComponent<Animator>().SetTrigger("close_menu");
            //bottomUI.GetComponent<Animator>().Play("Bottom_UI_Off");
        }

        public void OpenPopupMovement()
        {
            CloseWindows();
            if (popupMovement.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Popup_Off"))
            {
                popupMovement.GetComponent<Animator>().SetTrigger("open");
                popupMovement.GetComponent<Animator>().ResetTrigger("close");
                //popupMovement.GetComponent<Animator>().ResetTrigger("open");
            }

        }

        public void ClosePopupMovement()
        {
            if (popupMovement.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Popup_Idle"))
            {
                popupMovement.GetComponent<Animator>().SetTrigger("close");
                //popupMovement.GetComponent<Animator>().ResetTrigger("close");
                popupMovement.GetComponent<Animator>().ResetTrigger("open");
            }
                
        }

        public void OpenPopupExpression()
        {
            CloseWindows();
            if (popupExpression.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Popup_Off"))
            {
                popupExpression.GetComponent<Animator>().SetTrigger("open");
                popupExpression.GetComponent<Animator>().ResetTrigger("close");
            }
                
               
        }

        public void ClosePopupExpression()
        {
            if (popupExpression.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Popup_Idle"))
            {
                popupExpression.GetComponent<Animator>().SetTrigger("close");
                popupExpression.GetComponent<Animator>().ResetTrigger("open");
            }
                
        }

        public void OpenPopupSounds()
        {
            CloseWindows();
            if (popupSounds.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Popup_Off"))
            {
                popupSounds.GetComponent<Animator>().SetTrigger("open");
                popupSounds.GetComponent<Animator>().ResetTrigger("close");
            }
                
           
        }

        public void ClosePopupSounds()
        {
            if (popupSounds.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Popup_Idle"))
            {
                popupSounds.GetComponent<Animator>().SetTrigger("close");
                popupSounds.GetComponent<Animator>().ResetTrigger("open");
            }
                
        }

        public void OpenPopupSensors()
        {
            CloseWindows();
            if (popupSensors.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Popup_Off"))
            {
                popupSensors.GetComponent<Animator>().SetTrigger("open");
                popupSensors.GetComponent<Animator>().ResetTrigger("close");
            }
                
            
        }

        public void ClosePopupSensors()
        {
            if (popupSensors.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Popup_Idle"))
            {
                popupSensors.GetComponent<Animator>().SetTrigger("close");
                popupSensors.GetComponent<Animator>().ResetTrigger("open");
            }
                
        }

        public void OpenPopupVisions()
        {
            CloseWindows();
            if (popupVisions.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Popup_Off"))
            {
                popupVisions.GetComponent<Animator>().SetTrigger("open");
                popupVisions.GetComponent<Animator>().ResetTrigger("close");
            }
                
            
        }

        public void ClosePopupVisions()
        {
            if (popupVisions.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Popup_Idle"))
            {
                popupVisions.GetComponent<Animator>().SetTrigger("close");
                popupVisions.GetComponent<Animator>().ResetTrigger("open");
            }
                
        }

        public void OpenPopupLoops()
        {
            CloseWindows();
            if (popupLoops.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Popup_Off"))
            {
                popupLoops.GetComponent<Animator>().SetTrigger("open");
                popupLoops.GetComponent<Animator>().ResetTrigger("close");
            }
                
            
        }

        public void ClosePopupLoops()
        {
            if (popupLoops.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Popup_Idle"))
            {
                popupLoops.GetComponent<Animator>().SetTrigger("close");
                popupLoops.GetComponent<Animator>().ResetTrigger("open");
            }
                
        }

        public void ClosePopups()
        {
            if (popupLoops.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Popup_Idle"))
                popupLoops.GetComponent<Animator>().SetTrigger("close");
            if (popupVisions.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Popup_Idle"))
                popupVisions.GetComponent<Animator>().SetTrigger("close");
            if (popupSensors.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Popup_Idle"))
                popupSensors.GetComponent<Animator>().SetTrigger("close");
            if (popupSounds.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Popup_Idle"))
                popupSounds.GetComponent<Animator>().SetTrigger("close");
            if (popupExpression.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Popup_Idle"))
                popupExpression.GetComponent<Animator>().SetTrigger("close");
            if (popupMovement.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Popup_Idle"))
                popupMovement.GetComponent<Animator>().SetTrigger("close");

            bottomUI.GetComponent<Animator>().Play("Bottom_UI_Off");

        }

        public void CloseWindows()
        {
            CloseMenu();
            ClosePopupExpression();
            ClosePopupMovement();
            ClosePopupSounds();
            ClosePopupVisions();
            ClosePopupSensors();
            ClosePopupLoops();
        }

        public void PrintOsef()
        {
            Debug.Log("osef");
        }
    }
}