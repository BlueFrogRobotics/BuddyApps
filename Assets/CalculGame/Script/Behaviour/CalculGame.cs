using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using BuddyOS;
using UnityEngine.UI;

public class CalculGame : SpeechStateBehaviour
{

	// id the level - to know when to stop
	private int mLevelID;

	// timer - to check timeout
	private float mTimer;

	// value on the left of the operation
	private int mValueLeft;

	// operation (+, - or x)
	private string mOperation;

	// value on the right of the operation
	private int mValueRight;

	// correct result of the operation (for success/failure check)
	private int mCorrectResult;

	// operation text to display in the UI (ex: '5+2=')
	private string mTextCalcul;

	// last speech said by the user
	private string mLastSpeech;

	// bool to check if the canvas has been displayed (to hide it onExit)
	private bool mCanvasDisplayed;

	private float mMaxTimer = 20.0f;

	private string mClickedButtonValue;

	private string mCalculsDialogFileName;
	private string mCalculsUIFileName;

	// list of choices to be displayed (which one of them if the correct result)
	private List<string> mChoices;

	// list dialogs strings
	private List<string> firstQuestionWords;
	private List<string> nextQuestionWords;
	private List<string> lastQuestionWords;
	private List<string> howMuchIsWords;
	private List<string> notUnderstoodWords;
	private List<string> errorAudioWords;
	private List<string> errorNetworkWords;
	private List<string> errorRecognizerWords;
	private List<string> errorSpeechTimeoutWords;

	// ui texts
	private List<string> mIHeardWords;
	private List<string> mHowMuchIsUIWords;

	public GameObject mProgressBar;

	private Canvas mCanvasBackGround;
	private Canvas mCanvasCalcul;

	private AnimManager mAnimationManager;
	private SoundManager mSoundManager;

	public override void Init()
	{
		mAnimationManager = GetComponentInGameObject<AnimManager>(0);
		mSoundManager = GetComponentInGameObject<SoundManager>(1);
		mCanvasBackGround = GetComponentInGameObject<Canvas>(2);
		mCanvasCalcul = GetComponentInGameObject<Canvas>(3);
		mProgressBar = GetGameObject(4);

		if (BYOS.Instance.VocalActivation.CurrentLanguage == Language.FRA) {
			mCalculsDialogFileName = "calculs_dialogs_fr.xml";
			mCalculsUIFileName = "calculs_ui_fr.xml";
		} else {
			mCalculsDialogFileName = "calculs_dialogs_en.xml";
			mCalculsUIFileName = "calculs_ui_en.xml";
		}
	}

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	protected override void OnEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{

		// reset face to neutral
		mMood.Set(MoodType.NEUTRAL);

		// value set when button clicked (select answer) && on speech reco
		mClickedButtonValue = "";

		// init level
		mLevelID = animator.GetInteger("levelID");
		mLevelID += 1;
		animator.SetInteger("levelID", mLevelID);

		// init last speech vlaue
		mLastSpeech = "";

		// canvas is not displayed by default
		mCanvasDisplayed = false;

		// check if the level id is greater than the number of levels we want to play - if so exit
		if (mLevelID > CommonIntegers["nbLevels"]) {
			Debug.Log("End of the game. Give the results");
			animator.SetTrigger("EndGame");
		} else {

			// -- Common --

			mSynonymesFile = Resources.Load<TextAsset>(mCalculsDialogFileName).text;

			// init lists
			firstQuestionWords = new List<string>();
			nextQuestionWords = new List<string>();
			lastQuestionWords = new List<string>();
			howMuchIsWords = new List<string>();
			notUnderstoodWords = new List<string>();
			errorAudioWords = new List<string>();
			errorNetworkWords = new List<string>();
			errorRecognizerWords = new List<string>();
			errorSpeechTimeoutWords = new List<string>();

			// fill lists from xml file
			FillListSyn("FirstQuestion", firstQuestionWords);
			FillListSyn("NextQuestion", nextQuestionWords);
			FillListSyn("LastQuestion", lastQuestionWords);
			FillListSyn("HowMuchIs", howMuchIsWords);
			FillListSyn("NotUnderstood", notUnderstoodWords);
			FillListSyn("ErrorAudio", errorAudioWords);
			FillListSyn("ErrorNetwork", errorNetworkWords);
			FillListSyn("ErrorRecognizerBusy", errorRecognizerWords);
			FillListSyn("ErrorSpeechTimeout", errorSpeechTimeoutWords);

			// -- UI -- 

			mSynonymesFile = Resources.Load<TextAsset>(mCalculsUIFileName).text;

			mIHeardWords = new List<string>();
			mHowMuchIsUIWords = new List<string>();
			FillListSyn("IHeard", mIHeardWords);
			FillListSyn("HowMuchIs", mHowMuchIsUIWords);

			// init STT callbacks
			mSTT.OnBestRecognition.Add(OnSpeechRecognition);
			mSTT.OnPartial.Add(OnPartialRecognition);
			mSTT.OnErrorEnum.Add(ErrorSTT);

			// init level stuff 
			Debug.Log("Start level : " + mLevelID);
			mTimer = 0.0f;
			mTextCalcul = "XX";
			mChoices = new List<string>();

			SetLevelCalcul();

			// move the robot
			mAnimationManager.Blink();

			// say something according to the levelID
			if (mLevelID.Equals(1)) {
				// first level sentence
				mTTS.Say(RdmStr(firstQuestionWords) + RdmStr(howMuchIsWords) + SayLevelOperation(), true);
			} else if (mLevelID.Equals(CommonIntegers["nbLevels"])) {
				// last level sentence
				mTTS.Say(RdmStr(lastQuestionWords) + RdmStr(howMuchIsWords) + SayLevelOperation(), true);
			} else {
				// in between sentence
				mTTS.Say(RdmStr(nextQuestionWords) + RdmStr(howMuchIsWords) + SayLevelOperation(), true);
			}
		}
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	protected override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		// main loop waits for the tts to be over
		if (mTTS.HasFinishedTalking) {

			// if STT is done and lastSpeech empty - means we need to restart the STT
			if (mSTT.HasFinished && string.IsNullOrEmpty(mLastSpeech)) {
				Debug.Log("Start STT");
				// set listening face & led
				mMood.Set(MoodType.LISTENING);
				// start STT request
				mSTT.Request();
			}

			// update timer when the robot is not talking - (stops the game timer if the robot is talking, not fair otherwise)
			mTimer += Time.deltaTime;

			// display the canvas only once, once buddy is done saying the question
			if (!mCanvasDisplayed) {
				mCanvasDisplayed = true;
				// display canvas calcul
				DisplayCanvasCalcul(RdmStr(mHowMuchIsUIWords), mTextCalcul, mChoices);
			}

			// update UI timer
			SetCanvasCalculTimer(mTimer);

			// check is the user did not answer on time
			if (mTimer > mMaxTimer) {
				Debug.Log("TimeOut Game.");
				animator.SetTrigger("Timeout");
				mMood.Set(MoodType.SAD);
			}

			// check if user has clicked on a button or said something
			if (!string.IsNullOrEmpty( mClickedButtonValue) ) {

				Debug.Log("Check answer");

				// small animation on user said/clicked an answer
				mAnimationManager.Blink();

				// check if user said "quitter" or "retour"
				if (mClickedButtonValue.Contains("quitter") || mClickedButtonValue.Contains("retour") || mClickedButtonValue.Contains("stop")) {
					// quit game
					Debug.Log("Quit Game");
					animator.SetTrigger("Quit");
				} else if (mClickedButtonValue.Equals(mCorrectResult.ToString())) {
					// Check if the user said the correct answer
					Debug.Log("The answer is correct !");
					animator.SetTrigger("Success");
					// set face & led
					mMood.Set(MoodType.HAPPY);
				} else if (mChoices.Contains(mClickedButtonValue)) {
					// if wait the user isn't the right answer, but is still in the list of choices displayed - means user said the wrong answer
					Debug.Log("The answer is wrong !!! ");
					animator.SetTrigger("Fail");
					// set face & led
					mMood.Set(MoodType.SAD);
				} else {
					// otherwise user said something completely different - say not understood and restart listening (by setting mLastSpeech to "" && clickedButtonValue to null)
					Debug.Log("not understood, value not in the list");
					mTTS.Say(RdmStr(notUnderstoodWords));
					mLastSpeech = "";
					mClickedButtonValue = "";
				}
			}
		}
	}

	// speech reco callback
	private void OnSpeechRecognition(string iVoiceInput)
	{
		Debug.Log("OnSpeechReco");

		// set face, led & animation
		mMood.Set(MoodType.NEUTRAL);
		mAnimationManager.Blink();

		Debug.Log("[Heard] : " + iVoiceInput);

		// replace the words heard by number if necessary ('un' -> '1', 'moins' -> '-' etc..)
		iVoiceInput = ReplaceWordsNumber(iVoiceInput);

		// set active Answer in Dialog
		mLastSpeech = iVoiceInput;
		mClickedButtonValue = iVoiceInput.Replace(" ", "");
	}


	private void OnPartialRecognition(string iVoiceInput)
	{
		Debug.Log("OnPartialReco");
		Debug.Log("[Partial Reco] : " + iVoiceInput);
		
		// set face, led and animation
		mMood.Set(MoodType.NEUTRAL);
		mAnimationManager.Blink();

		// replace the words heard by number if necessary ('un' -> '1', 'moins' -> '-' etc...)
		iVoiceInput = ReplaceWordsNumber(iVoiceInput);

		// set active Answer in Dialog
		mLastSpeech = iVoiceInput;
		mClickedButtonValue = iVoiceInput;
	}

	private string ReplaceWordsNumber(string input)
	{
		// check if the word said is a number "un" , "deux" ..
		string replacedInput = input;

		replacedInput = replacedInput.Replace("un", "1");
		replacedInput = replacedInput.Replace("de", "2");

		// check if input contains the "moins" word
		if (input.Contains("moins")) {
			replacedInput = replacedInput.Replace("moins ", "-");
		} else if (input.Contains("moi")) {
			replacedInput = replacedInput.Replace("moi ", "-");
		} else if (input.Contains("moin")) {
			replacedInput = replacedInput.Replace("moin ", "-");
		}

		// replace spaces by empty strings
		return replacedInput.Replace(" ", "");
	}

	// Error STT callback
	void ErrorSTT(STTError iError)
	{
		Debug.Log("[question error] : " + iError);

		// timer to avoid repeated errors and robots saying stuff before the user could say anything
		if (mTimer > 2.0f) {

			// set face, led and animation
			mMood.Set(MoodType.NEUTRAL);
			mAnimationManager.Sigh();

			string lSentence = RdmStr(notUnderstoodWords);

			switch (iError) {
				case STTError.ERROR_AUDIO:
					lSentence = RdmStr(errorAudioWords);
					break;
				case STTError.ERROR_NETWORK:
					lSentence = RdmStr(errorNetworkWords);
					break;
				case STTError.ERROR_RECOGNIZER_BUSY:
					lSentence = RdmStr(errorRecognizerWords);
					break;
				case STTError.ERROR_SPEECH_TIMEOUT:
					lSentence = RdmStr(errorSpeechTimeoutWords);
					break;
			}

			if (UnityEngine.Random.value > 0.8) {
				mMood.Set(MoodType.GRUMPY);
				mTTS.Say(lSentence);
			}

			// reset lastSpeech and clickedButtonValue to force restart stt
			mLastSpeech = "";
			//mClickedButtonValue = "";
		} else {
			mTTS.Silence(1000, true);
			mLastSpeech = "";
			//mClickedButtonValue = "";
		}

	}

	void SetLevelCalcul()
	{
		// set level infos according to levelID
		Debug.Log("Set Level : " + mLevelID);

		// random differs according to the level
		// difficulty increases
		if (mLevelID.Equals(1)) {
			mValueLeft = UnityEngine.Random.Range(2, 5);
			mValueRight = UnityEngine.Random.Range(2, 5);
		} else if (mLevelID.Equals(2)) {
			mValueLeft = UnityEngine.Random.Range(2, 8);
			mValueRight = UnityEngine.Random.Range(2, 8);
		} else if (mLevelID.Equals(2)) {
			mValueLeft = UnityEngine.Random.Range(2, 10);
			mValueRight = UnityEngine.Random.Range(2, 10);
		} else if (mLevelID.Equals(3)) {
			mValueLeft = UnityEngine.Random.Range(5, 10);
			mValueRight = UnityEngine.Random.Range(5, 10);
		} else if (mLevelID.Equals(4)) {
			mValueLeft = UnityEngine.Random.Range(2, 4);
			mValueRight = UnityEngine.Random.Range(10, 20);
		} else {
			mValueLeft = UnityEngine.Random.Range(2, 4);
			mValueRight = UnityEngine.Random.Range(2, 30);
		}

		// select a random operation add, subtract or multiply
		SetRandomOperation();

		// set correctResult in the main script for further use in other states
		CommonIntegers["correctResult"] = mCorrectResult;

		// text displayed in the canvas
		mTextCalcul = mValueLeft + mOperation + mValueRight + " =";
	}

	// select a random operation between add '+' , subtract '-' or multiply 'x'
	void SetRandomOperation()
	{

		int rangeMin = 0;
		int rangeMax = 1;

		if (mLevelID > 2) {
			// increase difficulty from level 3 - more chance to get the multiplication
			rangeMin = 1;
			rangeMax = 3;
		} else if (mLevelID > 3) {
			rangeMin = 0;
			rangeMax = 3;
		}

		switch (UnityEngine.Random.Range(rangeMin, rangeMax)) {
			case 0:
				// addition
				mOperation = " + ";
				mCorrectResult = mValueLeft + mValueRight;
				break;
			case 1:
				// sub
				mOperation = " - ";
				mCorrectResult = mValueLeft - mValueRight;
				break;
			case 2:
				// multiply
				mOperation = " x ";
				mCorrectResult = mValueLeft * mValueRight;
				break;
			case 3:
				// multiply again - more chance to get it on higher levels
				mOperation = " x ";
				mCorrectResult = mValueLeft * mValueRight;
				break;
			default:
				// add
				mOperation = " + ";
				mCorrectResult = mValueLeft + mValueRight;
				break;
		}

		// set list of choices - different from the correct answer
		mChoices.Add(mCorrectResult.ToString());
		mChoices.Add(GenerateAlternativeAnswer().ToString());
		mChoices.Add(GenerateAlternativeAnswer().ToString());
		mChoices.Add(GenerateAlternativeAnswer().ToString());

		// shuffle list
		for (int i = 0; i < mChoices.Count; i++) {
			string tmp = mChoices[i];
			int randomIndex = UnityEngine.Random.Range(i, mChoices.Count);
			mChoices[i] = mChoices[randomIndex];
			mChoices[randomIndex] = tmp;
		}
	}

	// generate alternative answers (wrong answers)
	// and check if they are not similar to the correct answer
	int GenerateAlternativeAnswer()
	{
		bool done = false;
		int answer = 0;
		while (!done) {
			switch (UnityEngine.Random.Range(0, 3)) {
				case 0:
					answer = mCorrectResult + UnityEngine.Random.Range(1, 5);
					break;
				case 1:
					answer = mCorrectResult - UnityEngine.Random.Range(1, 5);
					break;
				case 2:
					answer = mCorrectResult - UnityEngine.Random.Range(1, 5) + UnityEngine.Random.Range(1, 5);
					break;
				default:
					answer = mCorrectResult - 1;
					break;
			}

			// answer should not already be in the choices list and should be different from the correct answer
			if (!answer.Equals(mCorrectResult) && !mChoices.Contains(answer.ToString())) {
				done = true;
			} else {
				Debug.Log("re generate alternative answer");
			}
		}
		return answer;
	}

	// generate sentence string that will be said by Buddy to ask the question
	public string SayLevelOperation()
	{
		string toSay = mValueLeft.ToString();
		if (mOperation.Equals(" x ")) {
			toSay += " fois ";
		} else if (mOperation.Equals(" - ")) {
			toSay += " moins ";
		} else {
			toSay += " " + mOperation.ToString() + " ";
		}
		toSay += mValueRight.ToString();
		return toSay;
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	protected override void OnExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		// hide the canvas
		HideCanvasCalcul();

		// reset face & led to neutral
		mMood.Set(MoodType.NEUTRAL);

		// remove STT callbacks
		mSTT.OnBestRecognition.Remove(OnSpeechRecognition);
		mSTT.OnPartial.Remove(OnPartialRecognition);
		mSTT.OnErrorEnum.Remove(ErrorSTT);
	}



	/////////////////// CANVAS /////////////////

	// Display the canvas that will ask the question and display the answers (4 answers)
	public void DisplayCanvasCalcul(string title, string calcul, List<string> choices)
	{

		// trigger the animators to display the canvas
		mCanvasBackGround.GetComponent<Animator>().SetTrigger("Open_BG");
		mCanvasCalcul.GetComponent<Animator>().SetTrigger("Open_WMath4");

		// fill the texts of the canvas
		Text[] textObjects = mCanvasCalcul.GetComponentsInChildren<Text>();
		foreach (Text t in textObjects) {
			// check if it's the title
			if (t.gameObject.name.Equals("Math_Text")) {
				t.text = calcul;
			}
			if (t.gameObject.name.Equals("Message")) {
				t.text = title;
			}
		}

		// fill the buttons texts
		textObjects[1].text = choices[0];
		textObjects[2].text = choices[1];
		textObjects[3].text = choices[2];
		textObjects[4].text = choices[3];

		// init the click callbacks to send the right value
		InitCallbacks();

		// init canvas displayed timer
		SetCanvasCalculTimer(0.0f);
	}

	// set the buttons listeners (callbacks) of the canvasCalcul
	void InitCallbacks()
	{
		Button[] buttons = mCanvasCalcul.GetComponentsInChildren<Button>();
		foreach (Button b in buttons) {
			string value = b.gameObject.GetComponentInChildren<Text>().text;
			b.onClick.AddListener(() => ProcessClickButton(value));
		}
	}

	// reset the buttons listeners of the canvasCalcul
	void ResetCallback()
	{
		mClickedButtonValue = "";
		Button[] buttons = mCanvasCalcul.GetComponentsInChildren<Button>();
		foreach (Button b in buttons) {
			b.onClick.RemoveAllListeners();
		}
	}

	// called when clicking on a button
	public void ProcessClickButton(string value)
	{
		mSoundManager.PlaySound(SoundType.BEEP2);
		Debug.Log("Clicked button with value : " + value);
		mTTS.Silence(5,false);
		mClickedButtonValue = value;
	}

	// hide the canvas calcul (question + list answers) & reset the buttons callbacks
	public void HideCanvasCalcul()
	{
		mCanvasBackGround.GetComponent<Animator>().SetTrigger("Close_BG");
		mCanvasCalcul.GetComponent<Animator>().SetTrigger("Close_WMath4");
		ResetCallback();
		mClickedButtonValue = "";
	}

	// update the canvas calcul timer, called on each frame within the game
	public void SetCanvasCalculTimer(float time)
	{
		// set canvas active to false if time is 0.0f, because otherwise there is an offset in the display
		if (time.Equals(0.0f)) {
			mProgressBar.SetActive(false);
		} else {
			mProgressBar.SetActive(true);
			mCanvasCalcul.GetComponentInChildren<Scrollbar>().size = time / mMaxTimer;
		}
	}







}
