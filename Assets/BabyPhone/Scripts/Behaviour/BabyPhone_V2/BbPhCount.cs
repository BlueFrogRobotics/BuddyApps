using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BuddyApp.BabyPhone
{
    public class BbPhCount : MonoBehaviour
    {
        [SerializeField]
        private Text mCounter;

        [SerializeField]
        private Animator mBabyPhoneAnimator;

        void OnEnable()
        {
            StartCoroutine(StartCount());
        }

        void OnDisable()
        {
            StopCoroutine(StartCount());
        }
        void Start()
        {

        }

        void Update()
        {
            if ((mBabyPhoneAnimator.GetCurrentAnimatorStateInfo(0).IsName("CountBeforStart"))
                && (mBabyPhoneAnimator.GetBool("DoStartCount")))
            {
                mCounter.text = "5";
                StartCoroutine(StartCount());
                mBabyPhoneAnimator.SetBool("DoStartCount", false);
            }
        }

        private IEnumerator StartCount()
        {
            yield return new WaitForSeconds(1F);
            mCounter.text = "4";
            yield return new WaitForSeconds(1F);
            mCounter.text = "3";
            yield return new WaitForSeconds(1F);
            mCounter.text = "2";
            yield return new WaitForSeconds(1F);
            mCounter.text = "1";
            yield return new WaitForSeconds(1F);
            mCounter.text = "0";
            yield return new WaitForSeconds(1F);
            if(mBabyPhoneAnimator.GetCurrentAnimatorStateInfo(0).IsName("CountBeforStart"))
                mBabyPhoneAnimator.SetTrigger("StartFallingAssleep");
        }
    }
}
