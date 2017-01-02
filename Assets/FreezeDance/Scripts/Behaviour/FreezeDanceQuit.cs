using UnityEngine;
using System.Collections;
using BuddyOS.Command;

public class FreezeDanceQuit : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void QuitApplication()
    {
        Debug.Log("Quit app");
        new HomeCmd().Execute();
    }
}
