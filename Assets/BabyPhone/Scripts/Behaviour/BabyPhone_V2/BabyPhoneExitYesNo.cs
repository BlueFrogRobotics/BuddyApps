using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS;
using BuddyOS.App;

public class BabyPhoneExitYesNo : MonoBehaviour
{
    [SerializeField]
    private Animator babyPhoneAnimator;

    [SerializeField]
    private Text message;

    [SerializeField]
    private Text yesButton;

    [SerializeField]
    private Text noButton;

    private Dictionary mDictionary;

    private int mForwardState;
	void Start ()
    {
        mForwardState = -1;
        mDictionary = BYOS.Instance.Dictionary;
        message.text = mDictionary.GetString("quitbb");
        yesButton.text = mDictionary.GetString("yes").ToUpper() ;
        noButton.text = mDictionary.GetString("no").ToUpper();
    }
	

	void Update ()
    {
        mForwardState = babyPhoneAnimator.GetInteger("ForwardState");
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
                babyPhoneAnimator.SetTrigger("StartApp");
                break;
            case 1:
                babyPhoneAnimator.SetTrigger("SetParameters");
                break;
            case 2:
                babyPhoneAnimator.SetTrigger("HeadAdjust");
                break;
            case 3:
                babyPhoneAnimator.SetTrigger("StartFallingAssleep");
                break;
            case 4:
                babyPhoneAnimator.SetTrigger("StartListening");
                break;
            default:
                babyPhoneAnimator.SetInteger("ForwardState", -1);
                break;
        }

    }
}
