using UnityEngine;
using UnityEngine.UI;
using BuddyOS;
using BuddyOS.App;

public class BabyPhoneParameters : AStateMachineBehaviour
{
    private GameObject mParameters;
    private Animator mBackgroundBlack;

    private Button mPlayButton;
    private Button mQuitButton;

    private bool mPlayParameters;

    public override void Init()
    {
        mParameters = GetGameObject(0);
        mBackgroundBlack = GetGameObject(9).GetComponent<Animator>();
        mPlayButton = GetGameObject(6).GetComponent<Button>();
        mQuitButton = GetGameObject(6).GetComponent<Button>();

        mPlayButton.onClick.AddListener(Play);
        mQuitButton.onClick.AddListener(Quit);

        mPlayParameters = false;
    }

    protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
    {
        mParameters.SetActive(true);
        mBackgroundBlack.SetTrigger("Open_BG");
    }

    protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
    {
        mParameters.SetActive(false);
        mBackgroundBlack.SetTrigger("Close_BG");
    }

    protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
    {
        if (mPlayParameters) iAnimator.SetBool("DoPlaySettings", true);

    }

    public void Play()
    {
        mPlayParameters = true;
    }

    public void Quit()
    {
        BYOS.Instance.AppManager.Quit();
    }
}
