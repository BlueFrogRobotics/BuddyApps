using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BuddyApp.IOT
{
    public class IOTListeningThis : MonoBehaviour
    {
        [SerializeField]
        private Transform UIText;
        [SerializeField]
        private float speed = 0.5F;

        private string mListenedThis;
        public string ListenedThis { get { return mListenedThis; } set { mListenedThis = value; } }

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
            UIText.GetChild(0).GetChild(1).GetComponent<Text>().text = mListenedThis;
            mMoving = UIText.GetChild(0);

            mStartPosition = mMoving.localPosition;
            mEndPosition = mMoving.localPosition - new Vector3(0F, mMoving.GetComponent<RectTransform>().rect.height, 0F);

            mTime = 0F;
        }

        // Update is called once per frame
        void Update()
        {
            mTime += Time.deltaTime * speed;
            if (mTime < 1F)
            {
                mMoving.localPosition = Vector3.Lerp(mStartPosition, mEndPosition, mTime);
            }
            else if (mTime > 5F)
            {
                mMoving.localPosition = Vector3.Lerp(mEndPosition, mStartPosition, mTime - 5F);
                if (mTime > 6F)
                {
                    mMoving.localPosition = mStartPosition;
                    gameObject.SetActive(false);
                }
            }
            else if (mTime > 1F)
                mMoving.localPosition = mEndPosition;
        }

        public void showMessage(string lMsg)
        {
            mListenedThis = lMsg;
            gameObject.SetActive(true);
        }
    }
}
