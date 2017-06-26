using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BuddyApp.Reminder
{
    public class PrintState : AStateMachineBehaviour
    {
        private float mTimer = 0.0f;
        private bool mExit = false;
        private List<ReminderFilter> mListFilter;

        override public void Start()
        {
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ShowReminders();
            mTimer = 0.0f;
            mExit = false;
            mListFilter = new List<ReminderFilter>();
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTimer += Time.deltaTime;
            if (ReminderManager.ReminderContent.reminderList.Count > 0 && mTimer > 25.0f && !mExit) {
                Trigger("Print");
                mExit = true;
                ClearWindow();
                CloseWindow();
            } else if (ReminderManager.ReminderContent.reminderList.Count == 0 && !mExit) {
                Trigger("ProposeAdd");
                mExit = true;
                ClearWindow();
                CloseWindow();
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        private void ShowReminders()
        {
            ReminderManager.SortListByDate();
            DateTime lDate = new DateTime();

            for (int i = 0; i < ReminderManager.ReminderContent.reminderList.Count; i++) {
                if (lDate.ToString("D") != (ReminderManager.ReminderContent.reminderList[i].RemindDate.ToString("D"))) {
                    GameObject lDatePrefab = Instantiate(GetGameObject(StateObject.DATE_PREFAB), GetGameObject(StateObject.PANEL_LIST).transform);
                    lDatePrefab.GetComponent<DateUI>().Date = ReminderManager.ReminderContent.reminderList[i].RemindDate.ToString("D");
                    lDate = ReminderManager.ReminderContent.reminderList[i].RemindDate;
                }
                GameObject lReminderPrefab = Instantiate(GetGameObject(StateObject.REMINDER_PREFAB), GetGameObject(StateObject.PANEL_LIST).transform);
                lReminderPrefab.GetComponent<ReminderUI>().Title = ReminderManager.ReminderContent.reminderList[i].Title;
                lReminderPrefab.GetComponent<ReminderUI>().Hour = ReminderManager.ReminderContent.reminderList[i].RemindDate.TimeOfDay.ToString();
                lReminderPrefab.GetComponent<ReminderUI>().Num = i;
                lReminderPrefab.GetComponent<ReminderUI>().DeleteButton.onClick.AddListener(delegate { OnDelete(lReminderPrefab.GetComponent<ReminderUI>().Num); });
            }
            GetGameObject(StateObject.WINDOW).SetActive(true);
        }

        private void ClearWindow()
        {
            foreach (Transform child in GetGameObject(StateObject.PANEL_LIST).transform) {
                GameObject.Destroy(child.gameObject);
            }
        }

        private void CloseWindow()
        {
            GetGameObject(StateObject.WINDOW).SetActive(false);
        }

        private void OnDelete(int iNum)
        {
            ReminderManager.ReminderContent.reminderList.RemoveAt(iNum);
            ClearWindow();
            ShowReminders();
        }

    }
}