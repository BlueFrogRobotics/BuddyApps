using System.Text.RegularExpressions;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

using Buddy;

namespace BuddyApp.PlayMath{
    public class SelectTableBehaviour : AnimationSyncBehaviour {

        [SerializeField]
        private Animator mPlayMathAnimator;

        private Text mTitleTop;

        private bool mTriggerOnce;

        void Start()
        {
            mTitleTop = GameObject.Find("UI/Set_Table/Top_UI/Title_Top").GetComponent<Text>();
        }

        public void InitState()
        {
            TranslateUI();
            mTriggerOnce = true;
        }

        public void OnClick(BaseEventData data)
        {
            GameObject lSelected = data.selectedObject;
            if (lSelected != null && mTriggerOnce)
            {
                Text lTextComponent = lSelected.GetComponentInChildren<Text>();
                User.Instance.GameParameters.Table = int.Parse(Regex.Match(lTextComponent.text,@"\d+").Value);
                mTriggerOnce = false;
                mPlayMathAnimator.SetTrigger("Play");
            }
        }

        public void OnClickGoToMenu() {
            if (mTriggerOnce)
            {
                Debug.Log("OnClickGoToMenu()");
                mTriggerOnce = false;
                mPlayMathAnimator.SetTrigger("BackToMenu");
            }
        }

        public void TranslateUI() {
            mTitleTop.text = BYOS.Instance.Dictionary.GetString("settabletitle").ToUpper();
        }
    }
}

