using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BuddyApp.FreezeDance
{
    public class FadeManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject screen;

        [SerializeField]
        private Image backgroundImage;

        [SerializeField]
        private float speed;

        private Color mAlpha;
        private bool FadeOut;

        // Use this for initialization
        void Start()
        {
            FadeOut = false;
            mAlpha = new Color();
            mAlpha = backgroundImage.color;
        }

        // Update is called once per frame
        void Update()
        {
            if (FadeOut) {
                if (mAlpha.a >= 0) {
                    mAlpha.a -= speed;
                    backgroundImage.color = mAlpha;
                    if (mAlpha.a < 0) {
                        screen.SetActive(false);
                        gameObject.SetActive(false);
                    }
                }
            }
        }

        public void StartFade()
        {
            FadeOut = true;
        }
    }
}