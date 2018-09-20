using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.BuddyLab
{
    public sealed class LoopConditionManager : MonoBehaviour
    {
        [SerializeField]
        private ConditionChoiceWindow conditionChoiceWindow;

        [SerializeField]
        private Image icon;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ShowWindow()
        {
            Debug.Log("le show button");
            DraggableItem lCell = GetComponentInParent<DraggableItem>();
            if (lCell != null && !lCell.OnlyDroppable)
            {
                conditionChoiceWindow.OnChangeCondition += OnConditionChoice;
                conditionChoiceWindow.ShowWindow();
            }
        }

        public void ChangeIcon(Sprite iSprite)
        {
            icon.sprite = iSprite;
        }

        private void OnConditionChoice(string iParam, Sprite iSprite)
        {
            icon.sprite = iSprite;
            GetComponent<ABLItem>().Parameter = iParam;
            conditionChoiceWindow.OnChangeCondition -= OnConditionChoice;
        }
    }
}