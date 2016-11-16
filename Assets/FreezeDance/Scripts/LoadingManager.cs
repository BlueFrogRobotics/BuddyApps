using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingManager : MonoBehaviour {

    [SerializeField]
    private Animator LoadingAnimator;
    [SerializeField]
    private GameObject BackgroundTricks;
    [SerializeField]
    private GameObject LoadingScreen;
    [SerializeField]
    private GameObject StartScreen;
    [SerializeField]
    private GameObject VictoryAnim;
    [SerializeField]
    private GameObject DefeatAnim;
    //[SerializeField]
    //private GameObject BackgroundStartAppli;

    // Use this for initialization
    void Start ()
    {
        StartCoroutine(LoadingScreenFunc());
	}
	
    IEnumerator LoadingScreenFunc()
    {
        //mVictoryAnim.SetActive(false);
        //mDefeatAnim.SetActive(false);
        yield return new WaitForSeconds(3f);
        StartScreen.SetActive(true);
        BackgroundTricks.SetActive(true);
        LoadingAnimator.SetBool("HasLoaded", true);
        //BackgroundStartAppli.SetActive(false);
        yield return new WaitForSeconds(1f);
        LoadingScreen.SetActive(false);
    }

}
