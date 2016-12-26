using UnityEngine;
using System.Collections;
using BuddyOS;

public class BabyPhoneExitYesNo : MonoBehaviour
{
    [SerializeField]
    private Animator mBabyPhoneAnimator;

    private int mForwardState;
	void Start ()
    {
        mForwardState = -1;
    }
	

	void Update ()
    {
        mForwardState = mBabyPhoneAnimator.GetInteger("ForwardState");
	}

    public void ExitBabyPhone()
    {
        BYOS.Instance.AppManager.Quit();
    }

    public void ReturnLastState()
    {
        Debug.Log(mForwardState);
        switch (mForwardState)
        {
            case 0:
                mBabyPhoneAnimator.SetTrigger("StartApp");
                break;
            case 1:
                mBabyPhoneAnimator.SetTrigger("SetParameters");
                break;
            case 2:
                mBabyPhoneAnimator.SetTrigger("HeadAdjust");
                break;
            case 3:
                mBabyPhoneAnimator.SetTrigger("StartFallingAssleep");
                break;
            case 4:
                mBabyPhoneAnimator.SetTrigger("StartListening");
                break;
            default:
                mBabyPhoneAnimator.SetInteger("ForwardState", -1);
                break;
        }

    }
}
