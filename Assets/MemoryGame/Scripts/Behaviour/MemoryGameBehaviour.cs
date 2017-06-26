using UnityEngine.UI;
using UnityEngine;

using Buddy;

namespace BuddyApp.MemoryGame
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class MemoryGameBehaviour : MonoBehaviour
    {
        /*
         * API of the robot
         */
        private TextToSpeech mTextToSpeech;

        /*
         * Data of the application. Save on disc when app quit happened
         */
        private MemoryGameData mAppData;

        /*
         * Init refs to API and your app data
         */
        void Start()
        {
            mTextToSpeech = BYOS.Instance.Interaction.TextToSpeech;
            mAppData = MemoryGameData.Instance;
        }

        /*
         * A sample of use of data (here for basic display purpose)
         */
        void Update()
        {
        }
    }
}