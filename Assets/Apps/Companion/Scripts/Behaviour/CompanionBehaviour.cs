using UnityEngine.UI;
using UnityEngine;

using Buddy;

namespace BuddyApp.Companion
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class CompanionBehaviour : MonoBehaviour
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
        private CompanionData mAppData;

        /*
         * Init refs to API and your app data
         */
        void Start()
        {
            mTextToSpeech = BYOS.Instance.Interaction.TextToSpeech;
            mAppData = CompanionData.Instance;
        }

        /*
         * A sample of use of data (here for basic display purpose)
         */
        void Update()
        {
			// ensure motors stay in same state as config

			//BYOS.Instance.Primitive.Motors.Wheels.Locked = !CompanionData.Instance.CanMoveBody;
			//BYOS.Instance.Primitive.Motors.YesHinge.Locked = !CompanionData.Instance.CanMoveHead;
			//BYOS.Instance.Primitive.Motors.NoHinge.Locked = !CompanionData.Instance.CanMoveHead;
		}
	}
}