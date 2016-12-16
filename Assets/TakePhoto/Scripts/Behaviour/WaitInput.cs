using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS.App;

public class WaitInput : SpeechStateBehaviour
{

	bool mAccepted;
	bool mRefused;
	int clickValidationId;
	float mTimer;
	float mTimeLimit;

	private Canvas mCanvasBackGround;
	private Canvas mCanvasMail;

	private AudioSource mButtonSound;

	public override void Init()
	{
		mCanvasMail = GetComponentInGameObject<Canvas>(3);
		mCanvasBackGround = GetComponentInGameObject<Canvas>(8);
		mButtonSound = GetComponentInGameObject<AudioSource>(9);
	}

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	protected override void OnEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		//// If we already have a mail, propose it
		//if (CommonStrings.Count == 2) {

		//	mCanvasMail.GetComponentInChildren<InputField>().GetComponentsInChildren<Text>()[1].text = CommonStrings[1];

		//}

		DisplayCanvasMail();

		mTimer = 0.0f;
		mTimeLimit = 30.0f;

		mAccepted = false;
		mRefused = false;

		clickValidationId = 0;

		Button[] buttons = mCanvasMail.GetComponentsInChildren<Button>();
		buttons[1].onClick.AddListener(ClickValidation);
		buttons[2].onClick.AddListener(ClickRefuse);

		// callback when user type something in input, reset timeout timer
		mCanvasMail.GetComponentInChildren<InputField>().onValueChanged.AddListener(OnInputValueChanged);

		SayInLang("enterMail");
	}

	public void OnInputValueChanged(string value)
	{
		Debug.Log("input value changed");
		mTimer = 0.0f;
	}

	public void ClickValidation()
	{
		mTimer = 0.0f;
		mButtonSound.Play();
		Debug.Log("Clicked validation");
		// check input
		InputField lInput = mCanvasMail.GetComponentInChildren<InputField>();
		string lMailInput = lInput.GetComponentsInChildren<Text>()[1].text;
		Debug.Log("mail input : " + lMailInput);
		if (lMailInput.Equals("")) {
			switch (clickValidationId) {
				case 0:
					SayInLang("needMail");
					break;
				case 1:
					SayInLang("emptyMail");
					break;
				case 2:
					mMood.Set(MoodType.GRUMPY);
					SayInLang("insist");
					//link.animationManager.Scream ();
					break;
				case 3:
					mMood.Set(MoodType.SAD);
					SayInLang("emptyMail");
					//link.animationManager.Sigh ();
					break;
				default:
					mMood.Set(MoodType.NEUTRAL);
					SayInLang("enterMail");
					break;
			}
			clickValidationId += 1;
		} else {
			//link.animationManager.Smile ();
			CommonStrings["mailTo"] = lMailInput;
			mAccepted = true;
		}
	}

	public void ClickRefuse()
	{
		mButtonSound.Play();
		Debug.Log("clicked refuse");
		mRefused = true;
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	protected override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		mTimer += Time.deltaTime;

		if (mAccepted) {
			// send mail
			Debug.Log("Sending mail accepted");
			SayInLang("validate");
			animator.SetTrigger("InputDone");
		} else if (mRefused) {
			Debug.Log("sending mail refused");
			SayInLang("okNP");
			animator.SetTrigger("Exit");
		} else {
			if (mTimer > mTimeLimit) {
				// timeout, exit
				Debug.Log("timeout exit.");
				SayInLang("nothingHappen");
				animator.SetTrigger("Exit");
			}
		}

	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	protected override void OnExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		HideCanvasMail();
		mMood.Set(MoodType.NEUTRAL);
	}

	/********************* Input Mail *********************/

	public void DisplayCanvasMail()
	{


		Text[] textObjects = mCanvasMail.GetComponentsInChildren<Text>();

		textObjects[0].text = mDictionary.GetString("enterMailShort").ToUpper();
		textObjects[2].text = mDictionary.GetString("mailInput");
		textObjects[4].text = mDictionary.GetString("validate").ToUpper();
		textObjects[5].text = mDictionary.GetString("noThanks").ToUpper();

		Debug.Log("Display canvas mail");
		mCanvasBackGround.GetComponent<Animator>().SetTrigger("Open_BG");
		mCanvasMail.GetComponent<Animator>().SetTrigger("Open_WMenu3");
	}

	public void HideCanvasMail()
	{
		Debug.Log("Hide canvas mail");
		mCanvasBackGround.GetComponent<Animator>().SetTrigger("Close_BG");
		mCanvasMail.GetComponent<Animator>().SetTrigger("Close_WMenu3");
	}
}
