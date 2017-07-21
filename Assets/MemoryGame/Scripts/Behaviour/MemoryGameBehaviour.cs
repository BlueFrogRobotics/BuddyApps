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

		private MemoryGameStateMachineManager mGameStateManager;

		private int mLastDifficulty;
		private bool mLastFullBody;

		/*
         * Init refs to API and your app data
         */
		void Start()
        {
            mTextToSpeech = BYOS.Instance.Interaction.TextToSpeech;
            mAppData = MemoryGameData.Instance;
			mGameStateManager = gameObject.GetComponent<MemoryGameStateMachineManager>();
            //mAppData.Difficulty = 1;
			//mAppData.FullBody = true;
			mLastDifficulty = mAppData.Difficulty;
			mLastFullBody = mAppData.FullBody;
        }

        /*
         * A sample of use of data (here for basic display purpose)
         */
        void Update()
        {
			if(mAppData.Difficulty != mLastDifficulty) {
				mLastDifficulty = mAppData.Difficulty;
				//mGameStateManager.mCommonObjects["gameLevels"] = new MemoryGameRandomLevel(mAppData.Difficulty);
                mGameStateManager.mAnimator.Play("DifficultyChanged");
			}else if(mAppData.FullBody != mLastFullBody) {
				mLastFullBody = mAppData.FullBody;
				mGameStateManager.mAnimator.Play("DifficultyChanged");
			}
        }
    }
}