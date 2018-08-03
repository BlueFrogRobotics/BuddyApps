using UnityEngine.UI;
using UnityEngine;
using System.Collections;

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

        private Animator mAnimator;

		private int mLastDifficulty;
		private bool mLastFullBody;
        private bool mMustCloseToaster;

        private void Awake()
        {
            mAnimator = GetComponent<Animator>();
        }

        /*
         * Init refs to API and your app data
         */
        void Start()
        {
            mTextToSpeech = BYOS.Instance.Interaction.TextToSpeech;
            mAppData = MemoryGameData.Instance;
			mGameStateManager = gameObject.GetComponent<MemoryGameStateMachineManager>();
            MemoryGameActivity.Init(mAnimator, this);
            //mAppData.Difficulty = 1;
			//mAppData.FullBody = true;
			mLastDifficulty = mAppData.Difficulty;
			mLastFullBody = mAppData.FullBody;
            BYOS.Instance.Header.OnHideParameters(OnHideParameters);
            mMustCloseToaster = false;
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
            //if(mMustCloseToaster && BYOS.Instance.Toaster.IsDisplayed)
            //{
            //    BYOS.Instance.Toaster.Hide();
            //    mMustCloseToaster = false;
            //}
        }

        private void OnDisable()
        {
            BYOS.Instance.Header.RemoveOnHideParameters(OnHideParameters);
        }

        private void OnHideParameters()
        {
            Debug.Log("on hide parameter");
            //BYOS.Instance.Toaster.Hide();
            //mMustCloseToaster = true;
            StartCoroutine(HideToaster());
        }

        private IEnumerator HideToaster()
        {
            Debug.Log("hide toaster");
            yield return new WaitForSeconds(2.3F);
            BYOS.Instance.Toaster.Hide();
        }
    }
}