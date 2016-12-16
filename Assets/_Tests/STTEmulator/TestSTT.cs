using UnityEngine;
using System.Collections;

public class TestSTT : MonoBehaviour {
	
	public BuddyAPI.SpeechToText stt;

	// Use this for initialization
	void Start () {
		transform.GetChild (0).gameObject.SetActive (false);
	}

	public void RecoString(string msg){
		stt.RecognitionCallback (msg);
	}


	// Update is called once per frame
	void Update () {
		#if UNITY_EDITOR
		transform.GetChild (0).gameObject.SetActive (!stt.HasFinished);
		#endif
	}

}
