using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Buddy;

namespace BuddyApp.RedLightGreenLightGame
{
    public class LifeManager : MonoBehaviour
    {
        public int Life { get; private set; }

        [SerializeField]
        private GameObject topLifes;

        [SerializeField]
        private GameObject[] lifes;

        [SerializeField]
        private GameObject lifesLeft;

        [SerializeField]
        private GameObject[] hearts;

        [SerializeField]
        private Sprite fullHeart;

        [SerializeField]
        private Sprite emptyHeart;

        private float mTimer=0F;
        private bool mIsLoosingLife;

        // Use this for initialization
        void Start()
        {
            Life = 3;
            mIsLoosingLife = false;
        }

        // Update is called once per frame
        void Update()
        {
            mTimer += Time.deltaTime;
            if(mIsLoosingLife && mTimer>2.0f)
            {
                lifesLeft.GetComponent<Animator>().SetTrigger("close");
                lifesLeft.SetActive(false);
                mIsLoosingLife = false;
            }
        }

        public void Reset()
        {
            Life = 3;
            foreach(GameObject life in lifes)
            {
                life.SetActive(true);
                life.GetComponent<Image>().sprite = fullHeart;
            }

            foreach (GameObject heart in hearts)
            {
                heart.SetActive(false);
                heart.SetActive(true);
                heart.GetComponent<Animator>().Play("BigHeart_Full");
            }
        }

        public void LoseLife()
        {
            if(Life>0)
            {
                //lifes[Life - 1].SetActive(false);
                lifes[Life - 1].GetComponent<Image>().sprite = emptyHeart;
                lifesLeft.SetActive(true);
                lifesLeft.GetComponent<Animator>().SetTrigger("open");
                hearts[Life - 1].GetComponent<Animator>().SetTrigger("lost");
                mTimer = 0.0F;
                mIsLoosingLife = true;
            }
            Life--;
        }

        public void ShowLifesLeft()
        {
            topLifes.SetActive(true);
            topLifes.GetComponent<Animator>().SetTrigger("open");
        }

        public void HideLifesLeft()
        {
            topLifes.GetComponent<Animator>().SetTrigger("close");
        }
    }
}