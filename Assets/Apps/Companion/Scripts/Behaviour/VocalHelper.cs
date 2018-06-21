using Buddy.UI;
using Buddy;
using UnityEngine;

using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace BuddyApp.Companion
{
	enum RequestType
	{
		DEFINITION,
		NEWS,
		WEATHER
	}

	/// <summary>
	/// Use this function in order to get the Type of question found
	/// </summary>
	/// <remarks>
	/// The value is a string and can be a lot of different things like Babyphone, VolumeUp
	/// </remarks>
	/// <param name="iType"></param>
	public delegate void QuestionAnalysed(string iType);

	/// <summary>
	/// This provides a standard vocal feature for basic question answering through pre-answered questions
	/// wolfram alpha queries and/or cleverbot answering
	/// </summary>
	public class VocalHelper : MonoBehaviour
	{

		/// <summary>
		///  Authorizes to know if an answer is pending
		/// </summary>
		public bool BuildingAnswer { get { return mBuildingAnswer; } }

		private bool mBuildingAnswer;
		
		
		public string Answer { get; private set; }

		/// <summary>
		/// Use this callback to know the Question type found (as string) 
		/// </summary>
		public QuestionAnalysed OnQuestionTypeFound { get; set; }

		void Start()
		{
			mBuildingAnswer = false;
		}
		
		
		public bool SpecialRequest(string iSpeech)
		{
			//Search for specific keywords and send the type through the delegate QuestionAnalyzed
			string lType = "";

            /////////////////////////
            //Apps
            ////////////////////////////


            if (ContainsOneOf(iSpeech, "alarm"))
                lType = "Alarm";
            //else if (ContainsOneOf(iSpeech, mQuizzSpeech))
            //	lType = "Quizz";
            else if (ContainsOneOf(iSpeech, "calculationgame"))
                lType = "Calcul";
            else if (ContainsOneOf(iSpeech, "memorygame"))
                lType = "Memory";
            //else if (ContainsOneOf(iSpeech, "babyphone"))
            //	lType = "Babyphone";
            else if (ContainsOneOf(iSpeech, "freezedance"))
                lType = "FreezeDance";
            else if (ContainsOneOf(iSpeech, "guardian"))
                lType = "Guardian";
            else if (ContainsOneOf(iSpeech, "iot"))
                lType = "IOT";
            else if (ContainsOneOf(iSpeech, "jukebox"))
                lType = "Jukebox";
            //else if (ContainsOneOf(iSpeech, mRecipeSpeech))
            //	lType = "Recipe";
            else if (ContainsOneOf(iSpeech, "radio"))
                lType = "Radio";
            else if (ContainsOneOf(iSpeech, "rlgl"))
                lType = "RLGL";
            else if (ContainsOneOf(iSpeech, "reminder"))
                lType = "Reminder";
            //else if (ContainsOneOf(iSpeech, mHideSeekSpeech))
            //	lType = "HideSeek";
            else if (ContainsOneOf(iSpeech, "pose"))
                lType = "Pose";
            else if (ContainsOneOf(iSpeech, "photo"))
                lType = "Photo";
            else if (ContainsOneOf(iSpeech, "weather"))
                lType = "Weather";

            //else if (ContainsOneOf(iSpeech, "story"))
            //	lType = "Story";

            /////////////////////////
            //BML
            ////////////////////////////
            else if (ContainsOneOf(iSpeech, "demoshort"))
                lType = "DemoShort";
            else if (ContainsOneOf(iSpeech, "demofull"))
                lType = "DemoFull";
            else if (ContainsOneOf(iSpeech, "dance"))
                lType = "Dance";
            else if (ContainsOneOf(iSpeech, "iloveyou") || ContainsOneOf(iSpeech, "kissme"))
                lType = "UserLove";
            else if (ContainsOneOf(iSpeech, "ihateyou"))
                lType = "UserHate";
            else if (ContainsOneOf(iSpeech, "welcome"))
                lType = "Welcome";
            //else if (ContainsOneOf(iSpeech, mJokeSpeech) || ContainsOneOf(iSpeech, "knockknock"))
            //{
            //    Debug.Log("Vocal helper joke");
            //    if (iSpeech.ToLower().Contains(BYOS.Instance.Dictionary.GetString("i")) || ContainsOneOf(iSpeech, "knockknock"))
            //        lType = "ListenJoke";
            //    else
            //        lType = "Joke";








            //}
            else if (ContainsOneOf(iSpeech, "accept"))
                lType = "Accept";
            //else if (ContainsOneOf(iSpeech, mQuitSpeech))
            //    lType = "Quit";
            //else if (ContainsOneOf(iSpeech, mRepeatAfterMeSpeech))
            //{
            //    int lKeywordsIndex = WordIndexOfOneOf(iSpeech, mRepeatAfterMeSpeech);
            //    string[] lWords = iSpeech.Split(' ');
            //    string lSentenceToRepeat = "";

            //    if (lKeywordsIndex != -1 && lKeywordsIndex != lWords.Length)
            //    {
            //        for (int j = lKeywordsIndex + 1; j < lWords.Length; j++)
            //            lSentenceToRepeat += lWords[j] + " ";
            //    }
            //    lType = "Answer";
            //    Answer = lSentenceToRepeat;
            //    //TTSProcessAndSay(lSentenceToRepeat, true);
            //    //mAnswerGiven = true;
            //}
            //else if (ContainsOneOf(iSpeech, mRepeatPlzSpeech))
            //{
            //    //TTSProcessAndSay(mPreviousAnswer, true);
            //    //mAnswerGiven = true;
            //    lType = "Repeat";
            //}
            else if (ContainsOneOf(iSpeech, "mood"))
            {
                Debug.Log("vocalHelper mood");
                lType = "Mood";

            }
            //else if (ContainsOneOf(iSpeech, "definition"))
            //{
            //    lType = "Definition";
            //    //We search for the location of the weather request
            //    int lKeywordIndex = WordIndexOfOneOf(iSpeech, mDefinitionSpeech);
            //    string[] lWords = iSpeech.Split(' ');
            //    string lDefinitionWord = "";

            //    if (lKeywordIndex != -1 && lKeywordIndex != lWords.Length)
            //    {
            //        for (int j = lKeywordIndex + 1; j < lWords.Length; j++)
            //            lDefinitionWord += lWords[j] + " ";
            //    }
            //    StartCoroutine(BuildDefinitionAnswer(lDefinitionWord));
            //}
            //else if (ContainsOneOf(iSpeech, mWhoIs))
            //{
            //    Debug.Log("Contains who is");
            //    //We search for the location of the weather request
            //    int lKeywordIndex = WordIndexOfOneOf(iSpeech, mWhoIs);
            //    string[] lWords = iSpeech.Split(' ');
            //    if (lKeywordIndex + 1 < lWords.Length)
            //    {
            //        if (char.IsUpper(lWords[lKeywordIndex + 1][0]))
            //        {
            //            Debug.Log("next is upper: " + lWords[lKeywordIndex + 1]);
            //            lType = "Definition";
            //            string lDefinitionWord = "";

            //            if (lKeywordIndex != -1 && lKeywordIndex != lWords.Length)
            //            {
            //                for (int j = lKeywordIndex + 1; j < lWords.Length; j++)
            //                    lDefinitionWord += lWords[j] + " ";
            //            }
            //            StartCoroutine(BuildDefinitionAnswer(lDefinitionWord));
            //        }
            //        else
            //        {

            //            Debug.Log("next is not upper: " + lWords[lKeywordIndex + 1]);
            //        }
            //    }

            //    // General answer if not common name
            //    if (lType != "Definition")
            //    {
            //        lType = "Answer";
            //        Answer = BuildGeneralAnswer(iSpeech.ToLower());
            //    }

            //}
            else if (ContainsOneOf(iSpeech, "buddylab"))
                lType = "BuddyLab";
            else if (ContainsOneOf(iSpeech, "timer"))
                lType = "Timer";
            //else if (ContainsOneOf(iSpeech, mFollowMeSpeech))
            //    lType = "FollowMe";
            //else if (ContainsOneOf(iSpeech, mLookAtMeSpeech))
            //    lType = "LookAtMe";
            else if (ContainsOneOf(iSpeech, "heat"))
                lType = "Heat";
            else if (ContainsOneOf(iSpeech, "colourseen"))
                lType = "ColourSeen";
            else if (ContainsOneOf(iSpeech, "detectobject"))
                lType = "DetectObject";
            else if (ContainsOneOf(iSpeech, "mirror"))
                lType = "Mirror";
            else if (ContainsOneOf(iSpeech, "nap"))
                lType = "Nap";
            else if (ContainsOneOf(iSpeech, "copy"))
                lType = "Copy";
            else if (ContainsOneOf(iSpeech, "connection"))
                lType = "Connection";
            //else if (ContainsOneOf(iSpeech, mWanderSpeech))
            //{
            //    Answer = FindMood(iSpeech.ToLower());
            //    lType = "Wander";
            //}
            else if (ContainsOneOf(iSpeech, "canmove"))
                lType = "CanMove";
            //else if (ContainsOneOf(iSpeech, mDontMoveSpeech))
            //    lType = "DontMove";
            //else if (ContainsOneOf(iSpeech, mHeadDownSpeech))
            //{
            //    Answer = GetNextNumber(iSpeech);
            //    Debug.Log("Vocal helper answer: " + Answer);
            //    lType = "HeadDown";
            //}
            //else if (ContainsOneOf(iSpeech, mHeadLeftSpeech))
            //{
            //    Answer = GetNextNumber(iSpeech);
            //    Debug.Log("Vocal helper answer: " + Answer);
            //    lType = "HeadLeft";
            //}
            //else if (ContainsOneOf(iSpeech, mHeadRightSpeech))
            //{
            //    Answer = GetNextNumber(iSpeech);
            //    Debug.Log("Vocal helper answer: " + Answer);
            //    lType = "HeadRight";
            //}
            //else if (ContainsOneOf(iSpeech, mHeadUpSpeech))
            //{
            //    Answer = GetNextNumber(iSpeech);
            //    Debug.Log("Vocal helper answer: " + Answer);
            //    lType = "HeadUp";
            //}
            //else if (ContainsOneOf(iSpeech, mTurnSpeech))
            //{
            //    Answer = GetNextNumber(iSpeech);
            //    Debug.Log("Vocal helper answer: " + Answer);
            //    if (iSpeech.ToLower().Contains(BYOS.Instance.Dictionary.GetString("head")))
            //        lType = "Head";
            //    else
            //        lType = "Move";

            //    if (ContainsOneOf(iSpeech, mLeftSpeech))
            //        lType += "Left";
            //    else
            //        lType += "Right";
            //}
            //else if (ContainsOneOf(iSpeech, mMoveBackwardSpeech))
            //{
            //    Answer = GetNextNumber(iSpeech);
            //    Debug.Log("Vocal helper answer: " + Answer);
            //    lType = "MoveBackward";
            //}
            //else if (ContainsOneOf(iSpeech, mMoveForwardSpeech))
            //{
            //    Answer = GetNextNumber(iSpeech);
            //    Debug.Log("Vocal helper answer: " + Answer);
            //    lType = "MoveForward";
            //}
            //else if (ContainsOneOf(iSpeech, mMoveLeftSpeech))
            //{
            //    Answer = GetNextNumber(iSpeech);
            //    Debug.Log("Vocal helper answer: " + Answer);
            //    lType = "MoveLeft";
            //}
            //else if (ContainsOneOf(iSpeech, mMoveRightSpeech))
            //{
            //    Answer = GetNextNumber(iSpeech);
            //    Debug.Log("Vocal helper answer: " + Answer);
            //    lType = "MoveRight";
            //}
            //else if (ContainsOneOf(iSpeech, "battery"))
            //{
            //    lType = "Battery";
            //}
            //else if (ContainsOneOf(iSpeech, mVolumeSpeech))
            //{
            //    Answer = "" + BYOS.Instance.Primitive.Speaker.GetVolume();
            //    Debug.Log("Vocal helper answer: " + Answer);
            //    lType = "Volume";
            //}
            //else if (ContainsOneOf(iSpeech, mVolumeDownSpeech))
            //{
            //    Answer = GetNextNumber(iSpeech);
            //    Debug.Log("Vocal helper answer: " + Answer);
            //    lType = "VolumeDown";
            //}
            //else if (ContainsOneOf(iSpeech, mVolumeUpSpeech))
            //{
            //    Answer = GetNextNumber(iSpeech);
            //    Debug.Log("Vocal helper answer: " + Answer);
            //    lType = "VolumeUp";
            //    //} else if (ContainsOneOf(iSpeech, "switchlanguage")) {
            //    //	Answer = "";
            //    //	if (ContainsOneOf(iSpeech, "english"))
            //    //		Answer = "English";
            //    //	else if (ContainsOneOf(iSpeech, "french"))
            //    //		Answer = "French";
            //    //	else if (ContainsOneOf(iSpeech, "Italian"))
            //    //		Answer = "Italian";

            //    //	lType = "SwitchLanguage";
            //}
            //else if (ContainsOneOf(iSpeech, mVolumeSpeech))
            //{
            //    Answer = "" + BYOS.Instance.Primitive.Speaker.GetVolume();
            //    Debug.Log("Vocal helper answer: " + Answer);
            //    lType = "Volume";
            //}
            //else if (ContainsOneOf(iSpeech, mVolumeDownSpeech))
            //{
            //    Answer = GetNextNumber(iSpeech);
            //    Debug.Log("Vocal helper answer: " + Answer);
            //    lType = "VolumeDown";
            //}
            //else if (ContainsOneOf(iSpeech, mVolumeUpSpeech))
            //{
            //    Answer = GetNextNumber(iSpeech);
            //    Debug.Log("Vocal helper answer: " + Answer);
            //    lType = "VolumeUp";
            //}
            //else if (ContainsOneOf(iSpeech, mThanksSpeech))
            //{
            //    //TTSProcessAndSay(RandomString(mURWelcomeSpeech));
            //    lType = "Answer";
            //    Answer = RandomString(mURWelcomeSpeech);
            //}
            //else if (ContainsOneOf(iSpeech, "date"))
            //{
            //    lType = "Date";
            //    //TTSProcessAndSay("Today, we are the  " + DateTime.Now.Day +
            //    //" " + DateTime.Now.Month +
            //    //		" " + DateTime.Now.Year, true);
            //}
            //else if (ContainsOneOf(iSpeech, mHourSpeech))
            //{
            //    lType = "Hour";

            //}
            //else if (ContainsOneOf(iSpeech, mPlaySpeech))
            //    lType = "Play";
            //else if (ContainsOneOf(iSpeech, "tellsomething"))
            //    lType = "TellSomething";
            //else if (ContainsOneOf(iSpeech, mDoSomethingSpeech))
            //    lType = "DoSomething";
            else if (iSpeech.ToLower().Contains("propose"))
                //lType = Suggest();
                lType = "propose";
            //else if (iSpeech.Contains("cleverbot")) {
            //	lType = "Cleverbot";

            //	if (mChatBotRequested) {
            //		TTSProcessAndSay("Cleverbot disabled", true);
            //		mChatBotRequested = false;
            //	} else {
            //		TTSProcessAndSay("Cleverbot enabled", true);
            //		mChatBotRequested = true;
            //		InitChatBot();
            //	}
            else if (!iSpeech.Any(c => char.IsDigit(c)))
            {
                lType = "Answer";
                Answer = BuildGeneralAnswer(iSpeech.ToLower());
            }
            else
            {

                string lSpeech = iSpeech.Trim();
                //lSpeech = lSpeech.Replace("x", "*");
                //lSpeech = lSpeech.Replace("÷", "/");


                //if (lSpeech.Contains("√")) {
                //	lSpeech = lSpeech.Replace("√", "sqrt");
                //	lSpeech = Regex.Replace(lSpeech, @"\d", "($0)").Replace("sqrt ", "sqrt");
                //}

                Debug.Log("pre: " + lSpeech);
                string pattern = @"(\s?)(\d+\.?((?<=\.)\d+)?)";
                Regex rgx = new Regex(pattern);
                lSpeech = rgx.Replace(lSpeech, "($2)");

                //lSpeech = Regex.Replace(lSpeech, @"\d+\.\d+", "($0)");
                var parser = new ExpressionParser();

                Debug.Log("post: " + lSpeech);




                try
                {
                    Expression exp = parser.EvaluateExpression(lSpeech);
                    Debug.Log("Operation " + iSpeech);
                    lType = "Operation";
                }
                catch
                {
                    lType = "Answer";
                    Answer = BuildGeneralAnswer(iSpeech.ToLower());
                }
                //if () {
                //	Debug.Log("Operation? " + iSpeech);
                //	lType = "Operation";
                //} else {
                //	lType = "Answer";
                //	Answer = BuildGeneralAnswer(iSpeech.ToLower());
                //}
            }

			OnQuestionTypeFound(lType);
			return true;
		}

		private string FindMood(string iSpeech)
		{
			if (iSpeech.Contains(BYOS.Instance.Dictionary.GetString("sad")))
				return MoodType.SAD.ToString();
			else if (iSpeech.Contains(BYOS.Instance.Dictionary.GetString("happy")))
				return MoodType.HAPPY.ToString();
			else if (iSpeech.Contains(BYOS.Instance.Dictionary.GetString("lovely")))
				return MoodType.LOVE.ToString();
			else if (iSpeech.Contains(BYOS.Instance.Dictionary.GetString("sick")))
				return MoodType.SICK.ToString();
			else if (iSpeech.Contains(BYOS.Instance.Dictionary.GetString("tired")))
				return MoodType.TIRED.ToString();
			else if (iSpeech.Contains(BYOS.Instance.Dictionary.GetString("angry")))
				return MoodType.ANGRY.ToString();
			else if (iSpeech.Contains(BYOS.Instance.Dictionary.GetString("grumpy")))
				return MoodType.GRUMPY.ToString();
			else if (iSpeech.Contains(BYOS.Instance.Dictionary.GetString("scared")))
				return MoodType.SCARED.ToString();
			else
				return MoodType.NEUTRAL.ToString(); ;
		}

		

		private string GetNextNumber(string iText)
		{
			if (!string.IsNullOrEmpty(iText)) {
				string[] lWords = iText.Split(' ');
				for(int i=0; i<lWords.Length; ++i) {
					try {
						float.Parse(lWords[i]);
						return lWords[i];
					}

					catch {
						continue;
					}
				}
			}
			return "";
		}

		private string BuildGeneralAnswer(string iData)
		{
			//In case there is no special keyword, look through the pre-answered questions from the question file

			string lFormatedData = Regex.Replace(iData, @"[^\w\s]", " ");
			//Debug.Log("BuildGeneralAnswer - ponctu " + lFormatedData);
			string lAnswer = "";

			string[] lWords = lFormatedData.Split(' ');
			//Debug.Log("Looking for input " + lFormatedData);

			//using (XmlReader lReader = XmlReader.Create(new StringReader(mQuestionsFile))) {
			//	while (lReader.ReadToFollowing("QA")) {
			//		lReader.ReadToFollowing("question");
			//		//Remove ponctuation
			//		string lContentQ = Regex.Replace(lReader.ReadElementContentAsString().ToLower(), @"[^\w\s]", " ");

			//		if (lContentQ.Contains(lFormatedData)) {
			//			Debug.Log("Found Content Question : " + lContentQ);
			//			bool lFoundInput = true;

			//			//We need to check for single input words for some reason, otherwise it doesn't work ...
			//			if (lWords.Length < 2) {
			//				//Debug.Log("Input corresponds to only one word");
			//				lFoundInput = false;
			//				string[] lQuestions = lContentQ.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			//				for (int i = 0; i < lQuestions.Length && !lFoundInput; ++i) {
			//					if (lQuestions[i] == lFormatedData) {
			//						lFoundInput = true;
			//					}
			//				}
			//			}

			//			if (lFoundInput) {
			//				lReader.ReadToFollowing("answer");
			//				string lContentA = lReader.ReadElementContentAsString();
			//				string[] lAnswers = lContentA.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

			//				if (lAnswers.Length == 1)
			//					lAnswer = lAnswers[0];
			//				else {
			//					System.Random lRnd = new System.Random();
			//					lAnswer = lAnswers[lRnd.Next(0, lAnswers.Length)];
			//				}
			//				Debug.Log("Found  Content Answer : " + lAnswer);
			//			}
			//			break;
			//		}
			//	}
			//}

			//if (string.IsNullOrEmpty(lAnswer)) {
			//	if (!mChatBotRequested || mCleverbotSession == null) {
			//		lAnswer = RandomString(mSorrySpeech) + " " +
			//			RandomString(mICouldntSpeech) + " " +
			//			RandomString(mGetSpeech) + " " +
			//			RandomString(mAnswersSpeech);
			//		OnQuestionTypeFound("");
			//		Debug.Log("CleverBot : Not activated with answer : " + lAnswer);
			//	} else {
			//		lAnswer = mCleverbotSession.Think(iData);
			//		Debug.Log("CleverBot answer : " + lAnswer);
			//	}
			//}

			return lAnswer;
		}

		//private IEnumerator BuildDefinitionAnswer(string iData)
		//{
		//	mBuildingAnswer = true;
		//	string lXmlData = "";
		//	yield return StartCoroutine(MakeRequest(RequestType.DEFINITION, iData, null, value => lXmlData = value));
		//	//Get only the first sentence of wikipedia introduction as definition. And this is bad in a lot of cases
		//	//Starting from here, we have the result of the page with the intro of selected page, categories and outlinks
		//	string lAnswer = "";

		//	//Check if the result is a multiple answer page
		//	if (lXmlData.Contains("Catégorie:Homonymie") || lXmlData.Contains("Category:Disambiguation")) {
		//		Debug.Log("Definition contains multiple answers.");
		//		string lOutput = "";

		//		using (XmlReader lReader = XmlReader.Create(new StringReader(lXmlData))) {
		//			lReader.ReadToFollowing("links");

		//			while (lReader.ReadToFollowing("pl")) {
		//				lReader.MoveToAttribute("title");
		//				string lValue = lReader.Value;
		//				if (!lValue.Contains("Liste"))
		//					lOutput += lValue + ". ";
		//			}
		//			lReader.Close();
		//		}
		//		lAnswer = RandomString(mSeveralResSpeech) + " " + lOutput.ToString();
		//	} else {
		//		//We found the article of the desired research. We get the first sentence of the introduction text
		//		string lOutput = "";

		//		using (XmlReader lReader = XmlReader.Create(new StringReader(lXmlData))) {
		//			if (lReader.ReadToFollowing("extract")) {
		//				lOutput += lReader.ReadElementContentAsString();

		//				if (string.IsNullOrEmpty(lOutput.ToString())) {
		//					//lAnswer = RandomString(mICouldntSpeech) + " " +
		//					//    RandomString(mGetSpeech) + " " +
		//					//    RandomString(mAnswersSpeech);
		//					lReader.MoveToContent();
		//					//Debug.Log("After MoveToContent : " + lReader.ReadElementContentAsString());
		//					if (lReader.ReadToFollowing("pl")) {
		//						lReader.MoveToAttribute("title");
		//						string lValue = lReader.Value;
		//						//Debug.Log("Searching other definition : " + lValue);
		//						yield return BuildDefinitionAnswer(lValue);
		//						yield break;
		//					}
		//				} else {
		//					string lOutputString = lOutput.ToString();

		//					if (lOutputString.Length <= 100)
		//						lAnswer = lOutputString;
		//					else {
		//						int lIndex = lOutputString.IndexOf('.', 100);
		//						lAnswer = lOutputString.Substring(0, lIndex);
		//					}
		//				}
		//			} else
		//				lAnswer = RandomString(mICouldntSpeech) + " " +
		//					RandomString(mGetSpeech) + " " +
		//					BYOS.Instance.Dictionary.GetRandomString("answers");

		//			lReader.Close();
		//		}
		//	}

		//	mBuildingAnswer = false;

		//	Answer = lAnswer;
		//}

		//private IEnumerator MakeRequest(RequestType iType, string iKeyword, Dictionary<string, string> iHeader, Action<string> ioResult)
		//{
		//	WWW lWww = new WWW(mWebsiteHash[iType] + Uri.EscapeUriString(iKeyword), null, iHeader);

		//	float lElapsedTime = 0.0f;
		//	while (!lWww.isDone) {
		//		lElapsedTime += Time.deltaTime;
		//		if (lElapsedTime >= 5f) break;
		//		yield return null;
		//	}
		//	if (!lWww.isDone || !string.IsNullOrEmpty(lWww.error)) {
		//		Debug.Log("Request error: " + lWww.error);
		//		ioResult(null);
		//		yield break;
		//	}
		//	ioResult(lWww.text);
		//}

		private bool ContainsOneOf(string iSpeech, List<string> iListSpeech)
		{
			for (int i = 0; i < iListSpeech.Count; ++i) {
				string[] words = iListSpeech[i].Split(' ');
				if (words.Length < 2) {
					words = iSpeech.Split(' ');
					foreach (string word in words) {
						if (word.ToLower() == iListSpeech[i].ToLower().Trim()) {
							return true;
						}
					}
				} else if (iSpeech.ToLower().Contains(iListSpeech[i].ToLower().Trim()))
					return true;
			}
			return false;
		}


		private bool ContainsOneOf(string iSpeech, string iKeySpeech)
		{
			string[] iListSpeech = BYOS.Instance.Dictionary.GetPhoneticStrings(iKeySpeech);


			for (int i = 0; i < iListSpeech.Length; ++i) {

				if (string.IsNullOrEmpty(iListSpeech[i]))
					continue;

				string[] words = iSpeech.Split(' ');
				if (words.Length < 2 && !string.IsNullOrEmpty(words[0])) {
					if (words[0].ToLower() == iListSpeech[i].ToLower().Trim()) {
						return true;
					}
				} else if (iSpeech.ToLower().Contains(iListSpeech[i].ToLower().Trim()))
					return true;
			}
			return false;
		}

		private int WordIndexOfOneOf(string iSpeech, List<string> iListSpeech)
		{
			//Returns the index of one of the words of iListSpeech keywords in the speech iSpeech
			for (int i = 0; i < iListSpeech.Count; ++i) {
				string[] lWords = iListSpeech[i].Split(' ');
				string lKeyword = lWords[lWords.Length - 1];
				string[] lSpeechWords = iSpeech.Split(' ');
				for (int j = 0; j < lSpeechWords.Length; j++) {
					if (lSpeechWords[j] == lKeyword)
						return j;
				}
			}
			return -1;
		}
	}
}
