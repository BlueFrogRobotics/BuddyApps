using UnityEngine.UI;
using UnityEngine;

using Buddy;

namespace BuddyApp.Reminder
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class ReminderBehaviour : MonoBehaviour
    {
        /*
         * Modified data from the UI interaction
         */
        [SerializeField]
        private Text text;

        /*
         * API of the robot
         */
        private TextToSpeech mTextToSpeech;

        /*
         * Data of the application. Save on disc when app quit happened
         */
        private ReminderData mAppData;

        /*
         * Init refs to API and your app data
         */
        void Start()
        {
            mTextToSpeech = BYOS.Instance.Interaction.TextToSpeech;
            mAppData = ReminderData.Instance;
        }

        /*
         * A sample of use of data (here for basic display purpose)
         */
        void Update()
        {
            text.text = "Value : " + mAppData.MyValue.ToString();
        }

        /*
        * Want to make Buddy tell something ?
        */
        public void Speak()
        {
            mTextToSpeech.Say("hello");
        } 
    }
}