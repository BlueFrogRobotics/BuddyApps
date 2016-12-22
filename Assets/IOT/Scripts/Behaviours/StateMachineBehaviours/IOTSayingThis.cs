using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BuddyApp.IOT
{
    public class IOTSayingThis : MonoBehaviour
    {
        [SerializeField]
        private Transform UIText;
        [SerializeField]
        private float speed = 0.5F;

        private string mSayingThis;
        public string SayingThis { get { return mSayingThis; } set { mSayingThis = value; } }

        private Transform mMoving;
        private Vector3 mStartPosition;
        private Vector3 mEndPosition;
        private float mTime = 0F;

        private BuddyOS.TextToSpeech mTTS;

        void Start()
        {
            mTTS = BuddyOS.BYOS.Instance.TextToSpeech;
        }
        // Use this for initialization
        void OnEnable()
        {
            UIText.GetChild(0).GetChild(1).GetComponent<Text>().text = mSayingThis;
            mMoving = UIText.GetChild(0);

            mStartPosition = mMoving.localPosition;
            mEndPosition = mMoving.localPosition - new Vector3(0F, mMoving.GetComponent<RectTransform>().rect.height, 0F);

            mTime = 0F;
        }

        // Update is called once per frame
        void Update()
        {
            if (mTime < 1F && !mTTS.HasFinishedTalking) {
                mTime += Time.deltaTime * speed;
                mMoving.localPosition = Vector3.Lerp(mStartPosition, mEndPosition, mTime);
            } else if (mTime > 1F && !mTTS.HasFinishedTalking)
                mMoving.localPosition = mEndPosition;

            if (mTTS.HasFinishedTalking) {
                mTime -= Time.deltaTime * speed;
                mMoving.localPosition = Vector3.Lerp(mStartPosition, mEndPosition, mTime);
                if (mTime < 0F) {
                    mMoving.localPosition = mStartPosition;
                    gameObject.SetActive(false);
                }
            }
        }

        public void showMessage(string lMsg)
        {
            mSayingThis = lMsg;
            gameObject.SetActive(true);
        }
    }
}
