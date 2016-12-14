using UnityEngine.UI;
using UnityEngine;
using BuddyOS;

namespace BuddyApp.Basic
{
    /* A basic monobehaviour as "AI" behaviour for our app */
    internal class BasicAppBehaviour : MonoBehaviour
    {
        /*
         * Modified data from the UI interaction
         */
        [SerializeField]
        private Text textOne;

        [SerializeField]
        private Text textTwo;

        [SerializeField]
        private Text textOneActive;

        /*
         * API of the robot
         */
        private TextToSpeech mTextToSpeech;
        private Motors mMotors;
        private Face mFace;

        /*
         * Access to the language dictionary. Each app may has its own dictionary.
         */ 
        private Dictionary mDictionary;

        /*
         * Data of the application. Save on disc when app quit happened
         */
        private BasicAppData mAppData;

        /*
         * Init refs to API and your app data
         */
        void Start()
        {
            mMotors = BYOS.Instance.Motors;
            mTextToSpeech = BYOS.Instance.TextToSpeech;
            mFace = BYOS.Instance.Face;
            mDictionary = BYOS.Instance.Dictionary;
            mAppData = BasicAppData.Instance;
        }

        /*
         * A sample of use of data (here for basic display purpose)
         */
        void Update()
        {
            textOneActive.text = mAppData.OneIsActive ? "First val is active" : "First val not active";
            textOne.text = mAppData.OneIsActive ? ("First value : " + mAppData.One.ToString()) : string.Empty;
            textTwo.text = "Second value : " + mAppData.Two;
        }

        /*
        * Want to make Buddy tell something ?
        */
        public void Speak()
        {
            mTextToSpeech.Say(mDictionary.GetString("hello"));
        }

        /*
        * Basic forward order with a medium speed (degrees / sec) for 2 secs.
        */
        public void Forward()
        {
            mMotors.Wheels.SetWheelsSpeed(150F, 150F, 2000);
        }

        /*
        * Basic backward order with a medium speed (degrees / sec) for 2 secs.
        */
        public void Backward()
        {
            mMotors.Wheels.SetWheelsSpeed(-150F, -150F, 2000);
        }

        /*
        * Change the mood of the robot randomly
        */
        public void RandomMood()
        {
            MoodType lMood = (MoodType)Random.Range(0, 10);
            mFace.SetExpression(lMood);
        }
    }
}