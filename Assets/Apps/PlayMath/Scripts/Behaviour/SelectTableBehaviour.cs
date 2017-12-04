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

        public SelectTableBehaviour()
        {
            mTitleTop = GameObject.Find("UI/Set_Table/Top_UI/Title_Top").GetComponent<Text>();
        }

        public void OnClick(BaseEventData data)
        {
            GameObject lSelected = data.selectedObject;
            if (lSelected != null)
            {
                Text lTextComponent = lSelected.GetComponentInChildren<Text>();
                User.Instance.GameParameters.Table = int.Parse(Regex.Match(lTextComponent.text,@"\d+").Value);
                mPlayMathAnimator.SetTrigger("Play");
            }
        }

        public void OnClickGoToMenu() {
            mPlayMathAnimator.SetTrigger("BackToMenu");
        }

        public void TranslateUI() {
            mTitleTop.text = BYOS.Instance.Dictionary.GetString("settabletitle").ToUpper();
        }
    }
}

