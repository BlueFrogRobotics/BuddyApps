using UnityEngine;
using System.Collections;

public class LoadingBabyphone : MonoBehaviour {

    [SerializeField]
    private GameObject mLoadingScreen;
    [SerializeField]
    private GameObject mStartScreen;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(LoadingScreen());
    }

    IEnumerator LoadingScreen()
    {
        yield return new WaitForSeconds(3f);
        mStartScreen.SetActive(true);

        yield return new WaitForSeconds(1f);
        mLoadingScreen.SetActive(false);
    }
}
