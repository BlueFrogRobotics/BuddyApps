using UnityEngine;
using System;
using System.Collections;

namespace BuddyApp.Alarm
{
    public class Counter : MonoBehaviour
    {
        [SerializeField]
        public bool isCounterStarted;

        [SerializeField]
        public bool isCounterDone;

        [SerializeField]
        public bool isDueDateSet;

        [SerializeField]
        public string publicDate;

        [SerializeField]
        public string publicDueDate;

        private DateTime mDueDate;
        public RunAwayAlarm mAlarmFunctions;

        // TODO :
        // - generalize this in order to do a class wich keep all the alarms in a vector and trigger the right function depending of due date
        // - clean the code
        // - [see TODO 1] 
        // - remove the public from the flags

        // How To use : 
        // initCounter() or the start function by default
        // setCounter(int seconds)
        // startCounter()
        // when done callbackWhenAlarmIsCalled() is called

        // Use this for initialization
        void Start()
        {
            InitCounter();
        }

        // Update is called once per frame
        void Update()
        {
            publicDate = System.DateTime.Now.ToString();
            publicDueDate = mDueDate.ToString();

            // if the counter is not running or done alredy
            if (isCounterDone || !isCounterStarted) {
                return;
            }

            // if the dueDate is in the past, then the alarm should run
            if (System.DateTime.Compare(System.DateTime.Now, mDueDate) >= 0) {
                isCounterDone = true;
                CallbackWhenAlarmIsCalled();
            }
        }

        // here we set the given date from the timerinformation to the counter
        // best change for this function would be to input a TimeSpan
        public void SetCounter(int timerAlarmInSecond)
        {
            if (timerAlarmInSecond < 0) {
                Debug.Log("given timer isn't positive");
                return;
            }
            System.TimeSpan alarmTimer = new System.TimeSpan(0, 0, timerAlarmInSecond);
            mDueDate = System.DateTime.Now.Add(alarmTimer);
            isDueDateSet = true;
        }

        // we start the counter (set flag for the update function)
        public void StartCounter()
        {
            // if due date is not set
            if (!isDueDateSet) {
                Debug.Log("Can't start the timer, the due date has not been set");
                return;
            }
            // if due date is in the past
            // TODO 1 use the compare function to see if the dueDate is past already, then refuse to start the counter, for now, we just trigger the alarm

            isCounterStarted = true;
        }

        // when we start the counter of when we want to restart it we should call this funtion
        private void InitCounter()
        {
            isCounterStarted = false;
            isCounterDone = false;
            isDueDateSet = false;
        }

        // this function is call when the alarm is done
        private void CallbackWhenAlarmIsCalled()
        {
            mAlarmFunctions.StartAlarm();
        }

        private void SetDueDate(DateTime iDueDateProposed)
        {
            mDueDate = iDueDateProposed;
            isDueDateSet = true;
        }
    }
}