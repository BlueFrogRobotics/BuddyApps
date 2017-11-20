using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.FreezeDance
{
    public class ScoreManager : MonoBehaviour
    {
        public float Score { get; private set; }

        [SerializeField]
        Mask redBarMask;

        [SerializeField]
        Mask blueBarMask;

        [SerializeField]
        GameObject jauge;

        private const float MAX_SCORE =2000f;
        private const float MAX_MASK_HEIGHT = 520f;

        // Use this for initialization
        void Start()
        {
            Reset();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Reset()
        {
            Score = 1000;
            SetGauge();
        }

        public void LoseLife()
        {
            Score -= 100;
            if (Score < 0)
                Score = 0;
            SetGauge();
            jauge.GetComponent<Animator>().SetTrigger("down");
        }

        public void WinLife()
        {
            Score += 100;
            if (Score > MAX_SCORE)
                Score = MAX_SCORE;
            SetGauge();
            jauge.GetComponent<Animator>().SetTrigger("up");
        }

        private void SetGauge()
        {
            float lHeight= (MAX_MASK_HEIGHT * Score) / MAX_SCORE;
            float lWidth = redBarMask.rectTransform.rect.width;
            redBarMask.rectTransform.sizeDelta = new Vector2(lWidth, lHeight);
            blueBarMask.rectTransform.sizeDelta = new Vector2(lWidth, lHeight);
        }
    }
}