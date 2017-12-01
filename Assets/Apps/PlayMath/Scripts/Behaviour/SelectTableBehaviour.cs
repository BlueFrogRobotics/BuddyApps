using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

using Buddy;

namespace BuddyApp.PlayMath{
    public class SelectTableBehaviour : MonoBehaviour {

        [SerializeField]
        private Animator mPlayMathAnimator;

        public SelectTableBehaviour()
        {
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
    }
}

