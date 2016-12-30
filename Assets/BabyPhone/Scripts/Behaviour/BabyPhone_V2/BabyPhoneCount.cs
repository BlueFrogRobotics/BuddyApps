using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS;

namespace BuddyApp.BabyPhone
{
    public class BabyPhoneCount : MonoBehaviour
    {
        [SerializeField]
        private Text label;

        [SerializeField]
        private Text counter;

        [SerializeField]
        private Animator babyPhoneAnimator;

        //private BabyPhoneData mBabyPhoneData;
        private Dictionary mDictionary;

        void OnEnable()
        {
            StopCoroutine(StartCount());
            mDictionary = BYOS.Instance.Dictionary;
            label.text = mDictionary.GetString("bbstart");
            //StartCoroutine(StartCount());
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
            if ((babyPhoneAnimator.GetCurrentAnimatorStateInfo(0).IsName("CountBeforStart"))
                && (babyPhoneAnimator.GetBool("DoStartCount")))
            {
                counter.text = "5";
                StartCoroutine(StartCount());
                babyPhoneAnimator.SetBool("DoStartCount", false);
            }
            else
                StopCoroutine(StartCount());
        }

        private IEnumerator StartCount()
        {
            yield return new WaitForSeconds(1F);
            counter.text = "4";
            yield return new WaitForSeconds(1F);
            counter.text = "3";
            yield return new WaitForSeconds(1F);
            counter.text = "2";
            yield return new WaitForSeconds(1F);
            counter.text = "1";
            yield return new WaitForSeconds(1F);
            counter.text = "0";
            yield return new WaitForSeconds(1F);
            if(babyPhoneAnimator.GetCurrentAnimatorStateInfo(0).IsName("CountBeforStart"))
                babyPhoneAnimator.SetTrigger("StartFallingAssleep");
        }
    }
}
