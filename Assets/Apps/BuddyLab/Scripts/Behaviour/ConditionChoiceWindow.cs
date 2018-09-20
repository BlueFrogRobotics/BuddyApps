using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BuddyApp.BuddyLab
{
    public sealed class ConditionChoiceWindow : MonoBehaviour
    {
        public delegate void ChangeCondition(string iParameter, Sprite iIcon);
        public ChangeCondition OnChangeCondition;

        [SerializeField]
        private GameObject backgroundBlack;

        private Animator mAnimator;

        // Use this for initialization
        void Start()
        {
            mAnimator = GetComponent<Animator>();
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

        public void OnChoice(string iParam)
        {
            Debug.Log("choix: " + iParam);
            Sprite lSprite=EventSystem.current.currentSelectedGameObject.transform.GetChild(1).GetComponent<Image>().sprite;
            if(OnChangeCondition!=null)
            {
                OnChangeCondition(iParam, lSprite);
            }
            HideWindow();
            
        }
    }
}