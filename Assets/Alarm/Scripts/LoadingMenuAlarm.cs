using UnityEngine;
using System.Collections;

public class LoadingMenuAlarm : MonoBehaviour {

    [SerializeField]
    private GameObject mLoadingScreen;
    [SerializeField]
    private GameObject mStartScreen;
    [SerializeField]
    private GameObject mGlobalIA;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(LoadingScreen());
    }

    IEnumerator LoadingScreen()
    {
        yield return new WaitForSeconds(3f);
        mStartScreen.SetActive(true);
        mGlobalIA.SetActive(true);

        yield return new WaitForSeconds(1f);
        mLoadingScreen.SetActive(false);


    }
}
