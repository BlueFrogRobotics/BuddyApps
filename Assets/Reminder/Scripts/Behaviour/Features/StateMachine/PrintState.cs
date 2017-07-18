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
            //if (ReminderManager.Command.StartDate.Equals(DateTime.MinValue))
            //    ShowReminders();
            //else
            
            mTimer = 0.0f;
            mExit = false;
            mListFilter = new List<ReminderFilter>();
            AddFilter(ReminderManager.Command);
            ShowRemindersFiltered();
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
                    lDatePrefab.GetComponent<DateUI>().DeleteButton.onClick.AddListener(delegate { OnDeleteDate(ReminderManager.ReminderContent.reminderList[i].RemindDate.ToString("D")); });
                    lDate = ReminderManager.ReminderContent.reminderList[i].RemindDate;
                }
                GameObject lReminderPrefab = Instantiate(GetGameObject(StateObject.REMINDER_PREFAB), GetGameObject(StateObject.PANEL_LIST).transform);
                lReminderPrefab.GetComponent<ReminderUI>().Title = ReminderManager.ReminderContent.reminderList[i].Title;
                lReminderPrefab.GetComponent<ReminderUI>().Hour = ReminderManager.ReminderContent.reminderList[i].RemindDate.TimeOfDay.ToString();
                lReminderPrefab.GetComponent<ReminderUI>().Num = i;
                lReminderPrefab.GetComponent<ReminderUI>().DeleteButton.onClick.AddListener(delegate { OnDeleteReminder(lReminderPrefab.GetComponent<ReminderUI>().Num); });
            }
            GetGameObject(StateObject.WINDOW).SetActive(true);
        }

        private void ShowRemindersFiltered()
        {
            ReminderManager.SortListByDate();
            DateTime lDate = new DateTime();
            int lNbDate=0;

            for (int i = 0; i < ReminderManager.ReminderContent.reminderList.Count; i++)
            {
                if (DoesRespectConditions(ReminderManager.ReminderContent.reminderList[i]))
                {
                    if (lDate.ToString("D") != (ReminderManager.ReminderContent.reminderList[i].RemindDate.ToString("D")))
                    {
                        GameObject lDatePrefab = Instantiate(GetGameObject(StateObject.DATE_PREFAB), GetGameObject(StateObject.PANEL_LIST).transform);
                        lDatePrefab.GetComponent<DateUI>().Date = ReminderManager.ReminderContent.reminderList[i].RemindDate.ToString("D");
                        lDate = ReminderManager.ReminderContent.reminderList[i].RemindDate;
                        lDatePrefab.GetComponent<DateUI>().DeleteButton.onClick.AddListener(delegate { OnDeleteDate(lDatePrefab.GetComponent<DateUI>().Date); });
                    }
                    GameObject lReminderPrefab = Instantiate(GetGameObject(StateObject.REMINDER_PREFAB), GetGameObject(StateObject.PANEL_LIST).transform);
                    lReminderPrefab.GetComponent<ReminderUI>().Title = ReminderManager.ReminderContent.reminderList[i].Title;
                    lReminderPrefab.GetComponent<ReminderUI>().Hour = ReminderManager.ReminderContent.reminderList[i].RemindDate.TimeOfDay.ToString();
                    lReminderPrefab.GetComponent<ReminderUI>().Num = i;
                    lReminderPrefab.GetComponent<ReminderUI>().DeleteButton.onClick.AddListener(delegate { OnDeleteReminder(lReminderPrefab.GetComponent<ReminderUI>().Num); });
                }
            }
            GetGameObject(StateObject.WINDOW).SetActive(true);
        }

        private void AddFilter(Command iCommand)
        {
            if (iCommand == null)
                return;

            bool lHasFilter = false;
            ReminderFilter lFilter = new ReminderFilter();
            if(!iCommand.StartDate.Equals(DateTime.MinValue) && !iCommand.EndDate.Equals(DateTime.MinValue))
            {
                lHasFilter = true;
                lFilter.DateStart = iCommand.StartDate;
                lFilter.DateEnd = iCommand.EndDate;
            }

            if(iCommand.Receiver!=null && iCommand.Receiver !="")
            {
                lHasFilter = true;
                lFilter.Receiver = iCommand.Receiver;
            }

            if (lHasFilter)
                mListFilter.Add(lFilter);
        }

        private bool DoesRespectConditions(Reminder iReminder)
        {
            foreach (ReminderFilter filter in mListFilter)
            {
                if (!filter.DateStart.Equals(DateTime.MinValue) && (iReminder.RemindDate < filter.DateStart || iReminder.RemindDate > filter.DateEnd))
                    return false;
                if (filter.Receiver != null && filter.Receiver != "" && filter.Receiver != iReminder.Receiver)
                    return false;
            }
            return true;
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

        private void OnDeleteReminder(int iNum)
        {
            ReminderManager.ReminderContent.reminderList.RemoveAt(iNum);
            ClearWindow();
            ShowRemindersFiltered();
        }

        private void OnDeleteDate(string iDate)
        {
            Debug.Log("date a virer: " + iDate);
            ReminderManager.ReminderContent.reminderList.RemoveAll(item => item.RemindDate.ToString("D") == iDate);

            ClearWindow();
            ShowRemindersFiltered();
        }

    }
}