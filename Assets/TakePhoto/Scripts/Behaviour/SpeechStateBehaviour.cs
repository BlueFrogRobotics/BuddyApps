using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using BuddyOS.App;

public abstract class SpeechStateBehaviour : AStateMachineBehaviour
{
	public string mSynonymesFile;

	// To replace [silence] with 1s of silence
	public void SayWithSilence(string iSpeech)
	{
		string[] sentences = iSpeech.Split(new string[] { "[silence]" }, StringSplitOptions.None);
		foreach (string sentence in sentences) {
			mTTS.Say(sentence, true);
			Debug.Log("Saying " + sentence);
			mTTS.Silence(1000, true);
		}
	}

	// Return true if iSpeech contains one of the string in iListSpeech
	public bool ContainsOneOf(string iSpeech, List<string> iListSpeech)
	{
		for (int i = 0; i < iListSpeech.Count; ++i) {
			//if iListSpeech has a single word, check for the word
			string[] words = iListSpeech[i].Split(' ');
			if (words.Length < 2) {
				words = iSpeech.Split(' ');
				foreach (string word in words) {
					if (word == iListSpeech[i].ToLower()) {
						return true;
					}
				}
			} else if (iSpeech.ToLower().Contains(iListSpeech[i].ToLower()))
				return true;
		}
		return false;
	}

	// Take randomly one of the string in the list
	public string RdmStr(List<string> iListStr)
	{
		if (iListStr.Count == 0) {
			Debug.Log("the following list is empty!!! " + iListStr.ToString());
		}
		System.Random lRnd = new System.Random();
		return iListStr[lRnd.Next(0, iListStr.Count)];
	}

	public void SayInLang(string iKey, bool iQueue = false)
	{
		mTTS.Say(mDictionary.GetString(iKey), iQueue);
	}


	// Fill the list with strings from the balise iXmlCode
	public void FillListSyn(string iXmlCode, List<string> iSynList)
	{
		using (XmlReader lReader = XmlReader.Create(new StringReader(mSynonymesFile))) {
			if (lReader.ReadToFollowing(iXmlCode)) {
				String lContent = lReader.ReadElementContentAsString();
				String[] lSynonymes = lContent.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < lSynonymes.Length; ++i)
					iSynList.Add(lSynonymes[i]);
			}
		}
	}

}
