﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using System.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BuddyApp.BuddyLab
{
    //TODO : when the file name is too long, just put 4-5 first letter then 3 ... and then 3 last letters


    public class BlOpen : AStateMachineBehaviour
    {
        private List<string> mProject;
        private int mNumberProject;
        private string mDirectoryPath;
        private DirectoryInfo mDirectoryInfo;
        private FileInfo[] mFileInfo;
        private Button mButton;
        private Transform mListButton;
        private Button mTrashButton;

        private BuddyLabBehaviour mBLBehaviour;

        private float mTimer;
        private bool mAnimDone;

        public override void Start()
        {
            mBLBehaviour = GetComponentInGameObject<BuddyLabBehaviour>(3);
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetGameObject(6).GetComponent<Animator>().SetTrigger("open");
            mTimer = 0F;
            mAnimDone = false;
            Debug.Log(BYOS.Instance.Resources.GetPathToRaw("Projects", LoadContext.APP));
            mButton = GetGameObject(0).GetComponent<Button>();
            mListButton = GetGameObject(1).GetComponent<Transform>();
            ///Load all files from folder projects 
            mProject = new List<string>();
            mDirectoryInfo = new DirectoryInfo(BYOS.Instance.Resources.GetPathToRaw("Projects", LoadContext.APP));
            
            if(mDirectoryInfo.GetFiles().Length > 0)
            {
                mFileInfo = new FileInfo[mDirectoryInfo.GetFiles().Length];

                mFileInfo = mDirectoryInfo.GetFiles();

                Debug.Log(mFileInfo.Length);
                foreach (FileInfo file in mFileInfo)
                {
                    if (file.Name.EndsWith("xml"))
                    {
                        mProject.Add(file.Name.Remove(file.Name.Length - 4));
                    }

                }
                for (int i = 0; i < mProject.Count; ++i)
                    Debug.Log("project " + i + " : " + mProject[i]);
                //End load projects

                //load all button + name
                for (int i = 0; i < mProject.Count; ++i)
                {
                    mButton.transform.GetChild(1).GetComponent<Text>().text = mProject[i].ToString();
                    mTrashButton = Instantiate(mButton, mListButton);
                    mTrashButton.onClick.AddListener(OpenProject);
                    mTrashButton.transform.GetChild(3).gameObject.GetComponent<Button>().onClick.AddListener(DeleteProject);
                }
            }

        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTimer += Time.deltaTime;
            if(mTimer > 0.4F && !mAnimDone)
            {
                GetGameObject(2).GetComponent<Animator>().SetTrigger("open");
                mAnimDone = true;
            }

            //Debug.Log(mDirectoryInfo.GetFiles("*.xml", SearchOption.AllDirectories).Length);
            //Debug.Log(mBLBehaviour.NameOpenProject);

            //write a msg like "you have no project saved for the moment"
            if (mDirectoryInfo.GetFiles().Length == 0 )
            {
                
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
        }

        private void DeleteProject()
        {
            Debug.Log(EventSystem.current.currentSelectedGameObject.transform.parent.GetComponentInChildren<Text>().text);
            Debug.Log(BYOS.Instance.Resources.GetPathToRaw("Projects", LoadContext.APP) + "/" +
                EventSystem.current.currentSelectedGameObject.transform.parent.GetComponentInChildren<Text>().text + ".xml");

            File.Delete(BYOS.Instance.Resources.GetPathToRaw("Projects", LoadContext.APP) + "/" +
                EventSystem.current.currentSelectedGameObject.transform.parent.GetComponentInChildren<Text>().text + ".xml");
            Destroy(EventSystem.current.currentSelectedGameObject.transform.parent.gameObject);
        }

        private void OpenProject()
        {
            mBLBehaviour.NameOpenProject = "";
            Debug.Log(EventSystem.current.currentSelectedGameObject.transform.GetComponentInChildren<Text>().text);
            mBLBehaviour.NameOpenProject = EventSystem.current.currentSelectedGameObject.transform.GetComponentInChildren<Text>().text;
            GetGameObject(2).GetComponent<Animator>().SetTrigger("close");
            Debug.Log(mBLBehaviour.NameOpenProject);
            Trigger("Scene");

        }


    }
}
