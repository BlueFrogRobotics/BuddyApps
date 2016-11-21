using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ActivePause : MonoBehaviour {

    [SerializeField]
    private GameObject mPause;

    [SerializeField]
    private GameObject mPlay;
    // Use this for initialization

    public void PauseMusic(GameObject ButtonToDeactivate)
    {
        //ButtonToDeactivate.SetActive(false);
        //ButtonToActivate.SetActive(true);
    }
    public void PauseMusicAndIcon()
    {
        if(mPause.activeSelf)
        {
            mPause.SetActive(false);
            mPlay.SetActive(true);
        }
        else if (mPlay.activeSelf)
        {
            mPlay.SetActive(false);
            mPause.SetActive(true);
        }
    }
}
