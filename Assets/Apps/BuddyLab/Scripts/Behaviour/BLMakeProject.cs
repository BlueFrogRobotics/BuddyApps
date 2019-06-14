using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using BlueQuark;

namespace BuddyApp.BuddyLab
{
	public sealed class BLMakeProject : AStateMachineBehaviour
	{
		private GameObject mYesNoButton;
		private float mTimer;
		private bool mAnimDone;
		private InputField mInputField;

		private List<string> mProject;
		private int mNumberProject;
		private string mDirectoryPath;
		private DirectoryInfo mDirectoryInfo;
		private FileInfo[] mFileInfo;
		private InputField.SubmitEvent mSe;
		private GameObject mPlaceholder;

		private BuddyLabBehaviour mBLBehaviour;

		public override void Start()
		{
			mBLBehaviour = GetComponentInGameObject<BuddyLabBehaviour>(3);
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

			GetGameObject(6).GetComponent<Animator>().SetTrigger("open");
			mProject = new List<string>();
			mDirectoryInfo = new DirectoryInfo(Buddy.Platform.Application.PersistentDataPath + "Projects");
			mTimer = 0F;
			mAnimDone = false;
			mYesNoButton = GetGameObject(5).transform.GetChild(0).gameObject;
			mInputField = mYesNoButton.transform.GetChild(1).GetComponent<InputField>();
			mInputField.GetComponentsInChildren<Text>()[0].text = Buddy.Resources.GetString("projectname");//placeholder
            

            //mPlaceholder = GetGameObject(9);
            //mPlaceholder.GetComponent<InputField>().text = "";
        }

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mTimer += Time.deltaTime;
			if (mTimer > 0.2F && !mAnimDone) {
				GetGameObject(5).GetComponent<Animator>().SetTrigger("open");
				mYesNoButton.transform.GetChild(0).GetComponentInChildren<Text>().text = Buddy.Resources.GetString("makeprojectcancel");
				mYesNoButton.transform.GetChild(2).GetComponentInChildren<Text>().text = Buddy.Resources.GetString("makeprojectvalidate");
				mInputField.onEndEdit.AddListener(FillStringWithInputField);
				mYesNoButton.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(Cancel);
				mYesNoButton.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(Validate);
				mAnimDone = true;
			}
		}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
            GetGameObject(12).SetActive(false);
            mInputField.onEndEdit.RemoveListener(FillStringWithInputField);
			mYesNoButton.transform.GetChild(0).GetComponent<Button>().onClick.RemoveListener(Cancel);
			mYesNoButton.transform.GetChild(2).GetComponent<Button>().onClick.RemoveListener(Validate);
		}

		private void Validate()
		{
			//verifier que le nom rentré n'existe pas deja et que taille > 0
			if (mBLBehaviour.NameOpenProject.Length == 0) {
                GetGameObject(12).GetComponent<Text>().text = Buddy.Resources.GetString("nameempty");
                GetGameObject(12).SetActive(true);
                mBLBehaviour.NameOpenProject = ""; 

                return;
			} else if (CheckIfNameAlreadyExist(mBLBehaviour.NameOpenProject)) {
                GetGameObject(12).GetComponent<Text>().text = Buddy.Resources.GetString("projectexist");
                GetGameObject(12).SetActive(true);
                mBLBehaviour.NameOpenProject = ""; 

            } else {

				using (var file = File.Create(Buddy.Platform.Application.PersistentDataPath + "Projects" + "/" + mBLBehaviour.NameOpenProject + ".xml")) {

				}
				GetGameObject(5).GetComponent<Animator>().SetTrigger("close");
				Trigger("Scene");
			}

		}

		private void Cancel()
		{
			GetGameObject(6).GetComponent<Animator>().SetTrigger("close");
			GetGameObject(5).GetComponent<Animator>().SetTrigger("close");
			Trigger("ProjectToMenu");

		}

		private void FillStringWithInputField(string iArg)
		{
			mBLBehaviour.NameOpenProject = iArg;
		}

		private bool CheckIfNameAlreadyExist(string iName)
		{
			//Mettre des debug partout pour les null ref et dans validate aussi

			if (mDirectoryInfo.GetFiles().Length > 0) {
				mFileInfo = new FileInfo[mDirectoryInfo.GetFiles().Length];

				mFileInfo = mDirectoryInfo.GetFiles();

				foreach (FileInfo file in mFileInfo) {
					if (file.Name.EndsWith("xml")) {
						mProject.Add(file.Name.Remove(file.Name.Length - 4));
					}

				}

				foreach (string str in mProject) {
					if (string.CompareOrdinal(iName, str) == 0) {
						Debug.Log("Nom existant");
						return true;
					}

				}
			}
			return false;
		}
	}
}

