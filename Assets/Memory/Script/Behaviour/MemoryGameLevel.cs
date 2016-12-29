using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class MemoryGameLevel
{
	public float speed;
	public string introSentence;
	public List<int> events;
	public string successSentence;
	public string failureSentence;
}

[System.Serializable]
public class MemoryGameLevels
{
	public string intro;
	public List<string> yourTurn;
	public List<MemoryGameLevel> levels;

	public static MemoryGameLevels Load(string pathFile){
		string filePath = pathFile.Replace (".json", "");

		TextAsset targetFile = Resources.Load<TextAsset>(filePath);

		return JsonUtility.FromJson<MemoryGameLevels> (@targetFile.text);
	}

	public string GetRandomYourTurn(){
		// get a random question in the list
		return yourTurn[UnityEngine.Random.Range(0, (int) Mathf.Round(yourTurn.Count - 1))];
	}
}

